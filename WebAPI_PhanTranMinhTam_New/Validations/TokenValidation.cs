using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Validations
{
    public class TokenValidation : AbstractValidator<TokenDTO>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public TokenValidation(IRepositoryWrapper repositoryWrapper, JwtSecurityTokenHandler tokenHandler, TokenValidationParameters tokenValidationParameters)
        {
            _repositoryWrapper = repositoryWrapper;
            _tokenHandler = tokenHandler;
            _tokenValidationParameters = tokenValidationParameters;



            // trả về false mới báo lỗi
            RuleFor(x => x.AccessToken)
               //.NotEmpty().WithMessage("Access Token is required.")
               .Custom((accessToken, context) =>
               {
                   if (!BeValidAccessToken(accessToken))
                   {
                       context.AddFailure("Access Token wrong format.");
                   }
                   else if (!BeValidAlgAccessToken(accessToken))
                   {
                       context.AddFailure("Invalid token algorithm.");
                   }
                   else if (!BeValidExpiredAccessToken(accessToken))
                   {
                       context.AddFailure("Access Token has not yet expired.");
                   }
               });

            RuleFor(x => x.RefeshToken)
                //.NotEmpty().WithMessage("Refresh Token is required.")
                .Custom((refreshToken, context) =>
                {
                    if (!BeValidExistRefreshToken(refreshToken))
                    {
                        context.AddFailure("Refresh Token does not exist.");
                    }
                    else if (!BeRevokeRefreshToken(refreshToken))
                    {
                        context.AddFailure("Refresh Token has been revoked.");
                    }
                    //else if (!BeMatchRefreshToken())
                    //{
                    //    context.AddFailure("Refresh Token does not match the provided Access Token.");
                    //}
                });
        }

        private bool BeValidAccessToken(string accessToken)
        {
            try
            {
                System.Security.Claims.ClaimsPrincipal tokenInVerification = _tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken? validatedToken);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private bool BeValidAlgAccessToken(string accessToken)
        {
            System.Security.Claims.ClaimsPrincipal tokenInVerification = _tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken? validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                bool result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                return result;
            }

            return false;
        }

        private bool BeValidExpiredAccessToken(string accessToken)
        {
            System.Security.Claims.ClaimsPrincipal tokenInVerification = _tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken? validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                long utcExpireDate = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(x =>
                    x.Type == JwtRegisteredClaimNames.Exp)?.Value ?? "0");

                DateTime expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                return expireDate <= DateTime.UtcNow;
            }

            return true;
        }

        private bool BeValidExistRefreshToken(string refreshToken)
        {
            Data.RefreshToken storedToken = _repositoryWrapper.RefreshToken
                .FirstOrDefaultAsync(x => x.Token == refreshToken).Result;

            return storedToken != null;
        }

        private bool BeRevokeRefreshToken(string refreshToken)
        {
            Data.RefreshToken storedToken = _repositoryWrapper.RefreshToken
                .FirstOrDefaultAsync(x => x.Token == refreshToken).Result;

            return storedToken != null && !storedToken.IsRevoked;
        }

        private bool BeMatchRefreshToken(string accessToken)
        {
            // Validate the access token
            System.Security.Claims.ClaimsPrincipal tokenInVerification = _tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken? validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                string? jti = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(jti))
                    return false;

                // Check if the refresh token matches the JWT ID
                Data.RefreshToken storedToken = _repositoryWrapper.RefreshToken
                    .FirstOrDefaultAsync(x => x.JwtId == jti).Result;

                return storedToken != null;
            }

            return false;
        }


        private DateTime ConvertUnixTimeToDateTime(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
        }
    }
}
