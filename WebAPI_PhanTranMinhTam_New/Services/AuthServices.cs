using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;
using WebAPI_PhanTranMinhTam_New.Validations;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IAuthService
    {
        Task<TokenDTO> AuthenticateAsync(string name, string password);
        Task<(bool Success, string? Message, TokenDTO? Token)> RenewTokenAsync(TokenDTO model);
        Task LogoutAsync(string refreshToken);
        Task<(bool Success, string? Message)> RegisterAsync(RegisterDTO registerDTO);
    }
    public class AuthServices : IAuthService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IConfiguration _configuration;
        private readonly TokenValidation _tokenValidator;

        public AuthServices(IRepositoryWrapper repositoryWrapper, IConfiguration configuration, TokenValidation tokenValidator)
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _tokenValidator = tokenValidator;
        }

        public async Task<TokenDTO> AuthenticateAsync(string name, string password)
        {
            User user = await _repositoryWrapper.User
                .SingleOrDefaultAsync(u => u.Name == name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            TokenDTO tokenDTO = await GenerateToken(user);
            return tokenDTO;
        }

        public async Task<TokenDTO> GenerateToken(User user)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("AppSettings");
            string keyString = jwtSettings["SecretKey"];
            byte[] key = Encoding.UTF8.GetBytes(keyString);

            // Lấy các quyền hạn của user
            List<string> userPermissions = await _repositoryWrapper.RolePermisstion
                .FindByCondition(rp => rp.IdRole == user.RoleId)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            // Tạo các claims
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()) // Add Role claim
            };

            // Thêm quyền hạn (permissions) vào claims
            claims.AddRange(userPermissions.Select(permission => new Claim("Permission", permission)));

            // Tạo token
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string accessToken = tokenHandler.WriteToken(token);

            string refreshToken = GenerateRefeshToken();
            RefreshToken refreshTokenEntity = new()
            {
                IdRefreshToken = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.IdUser,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            _repositoryWrapper.RefreshToken.Create(refreshTokenEntity);
            await _repositoryWrapper.SaveAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefeshToken = refreshToken,
            };
        }



        public async Task<(bool Success, string? Message, TokenDTO? Token)> RenewTokenAsync(TokenDTO model)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };

            TokenValidation tokenValidator = new(_repositoryWrapper, tokenHandler, tokenValidationParameters);
            FluentValidation.Results.ValidationResult validationResult = tokenValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errors, null);
            }

            RefreshToken storedToken = await _repositoryWrapper.RefreshToken
                .FirstOrDefaultAsync(x => x.Token == model.RefeshToken);

            storedToken.IsRevoked = true;
            storedToken.IsUsed = true;

            _repositoryWrapper.RefreshToken.Update(storedToken);
            await _repositoryWrapper.SaveAsync();

            User user = await _repositoryWrapper.User.SingleOrDefaultAsync(t => t.IdUser == storedToken.UserId);
            TokenDTO token = await GenerateToken(user);

            return (true, null, token);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            RefreshToken storedToken = await _repositoryWrapper.RefreshToken.SingleOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                _repositoryWrapper.RefreshToken.Update(storedToken);
                await _repositoryWrapper.SaveAsync();
            }
        }
        public async Task<(bool Success, string? Message)> RegisterAsync(RegisterDTO registerDTO)
        {
            User existingUser = await _repositoryWrapper.User
                .SingleOrDefaultAsync(u => u.Email == registerDTO.Email || u.Name == registerDTO.Name);
            if (existingUser != null)
            {
                return (false, "User with this email or username already exists");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            User newUser = new()
            {
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = registerDTO.PhoneNumber,
            };

            _repositoryWrapper.User.Create(newUser);
            _repositoryWrapper.Save();

            return (true, "User registered successfully");
        }
        private string GenerateRefeshToken()
        {
            byte[] random = new Byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
    }
}
