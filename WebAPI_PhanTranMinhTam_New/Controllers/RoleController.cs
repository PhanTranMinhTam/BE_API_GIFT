using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleServices _roleServices;

        public RoleController(IRoleServices roleServices)
        {
            _roleServices = roleServices;
        }
        [Permisstion("Read")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            Data.Role role = await _roleServices.GetRoleByIdAsync(id);
            if (role == null)
            {
                return BadRequest("Vai trò không tồn tại.");
            }

            return Ok(role);
        }
        [Permisstion("Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            IEnumerable<Data.Role> roles = await _roleServices.GetAllRolesAsync();
            return Ok(roles);
        }
        [Permisstion("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO roleCreateDto)
        {
            if (roleCreateDto == null)
            {
                return BadRequest("Thông tin vai trò không hợp lệ.");
            }

            Result result = await _roleServices.CreateRoleAsync(roleCreateDto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetRoleById), new { id = result.Data }, result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
        [Permisstion("Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDTO roleUpdateDto)
        {
            if (roleUpdateDto == null)
            {
                return BadRequest("Thông tin vai trò không hợp lệ.");
            }

            Result result = await _roleServices.UpdateRoleAsync(id, roleUpdateDto);

            if (result.IsSuccess)
            {
                return Ok("Vai trò đã được cập nhật.");
            }

            return BadRequest(result.ErrorMessage);
        }
        [Permisstion("Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            Result result = await _roleServices.DeleteRoleAsync(id);

            if (result.IsSuccess)
            {
                return Ok("Vai trò đã được xóa thành công.");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
