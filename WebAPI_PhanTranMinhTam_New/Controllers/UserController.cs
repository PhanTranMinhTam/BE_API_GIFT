using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly MyDbContext _dbContext;
        public UserController(IUserServices userServices, MyDbContext dbContext)
        {
            _userServices = userServices;
            _dbContext = dbContext;

        }
        [Permisstion("Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {

            IEnumerable<User> users = await _userServices.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "No users found."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Data = users
            });
        }
        [Permisstion("Read")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {

            User user = await _userServices.GetUserByIdAsync(id);

            if (user == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = $"User with ID {id} not found."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Data = user
            });
        }
        [Permisstion("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] CreateDTO userDto)
        {

            if (ModelState.IsValid)
            {
                await _userServices.AddUser(userDto);
                return Ok();
            }

            return BadRequest(ModelState);
        }
        [Permisstion("Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsersAsync(int id, [FromForm] UpdateUserDTO userDto)
        {

            bool updateResult = await _userServices.UpdateUserAsync(id, userDto);

            if (!updateResult)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = $"User with ID {id} was not found or failed to update."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = $"User with ID {id} has been successfully updated."
            });
        }
        [Permisstion("Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {

            bool deleteResult = await _userServices.DeleteUserAsync(id);

            if (!deleteResult)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = $"User with ID {id} was not found or could not be deleted."
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = $"User with ID {id} has been successfully deleted."
            });
        }
    }
}
