using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService loginService)
        {
            this._authService = loginService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthDTO loginDTO)
        {
            TokenDTO token = await _authService.AuthenticateAsync(loginDTO.Name, loginDTO.PasswordHash);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            return Ok(new
            {
                Success = true,
                Message = "Authenticate success",
                Data = token
            });
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] TokenDTO tokenDTO)
        {
            await _authService.LogoutAsync(tokenDTO.RefeshToken);

            return Ok(new
            {
                Success = true,
                Message = "Logout successful"
            });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            (bool Success, string? Message) result = await _authService.RegisterAsync(registerDTO);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new
            {
                Success = true,
                Message = result.Message
            });
        }
        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenDTO model)
        {
            (bool success, string message, TokenDTO token) = await _authService.RenewTokenAsync(model);
            if (!success)
            {
                return BadRequest(new { Success = false, Message = message });
            }

            return Ok(new { Success = true, Data = token });
        }
    }
}
