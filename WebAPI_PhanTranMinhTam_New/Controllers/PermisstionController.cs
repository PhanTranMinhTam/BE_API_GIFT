using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermisstionController : ControllerBase
    {
        private readonly IPermisstionServices _permisstionServices;

        public PermisstionController(IPermisstionServices permisstionServices)
        {
            _permisstionServices = permisstionServices;
        }
        [Permisstion("Read")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermisstionById(int id)
        {
            Data.Permission permission = await _permisstionServices.GetPermisstionByIdAsync(id);
            if (permission == null)
            {
                return BadRequest("Permission không tồn tại.");
            }
            return Ok(permission);
        }
        [Permisstion("Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            IEnumerable<Data.Permission> roles = await _permisstionServices.GetAllPermisstionAsync();
            return Ok(roles);
        }
        [Permisstion("Create")]
        [HttpPost]
        public async Task<IActionResult> CreatePermisstion([FromBody] PermisstionDTO permisstionCreateDto)
        {
            if (permisstionCreateDto == null)
            {
                return BadRequest("Permisstion vai trò không hợp lệ.");
            }

            Result result = await _permisstionServices.CreatePermisstionAsync(permisstionCreateDto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetPermisstionById), new { id = result.Data }, result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
        [Permisstion("Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermisstion(int id, [FromBody] PermisstionDTO roleUpdateDto)
        {
            if (roleUpdateDto == null)
            {
                return BadRequest("Thông tin Permisstion không hợp lệ.");
            }

            Result result = await _permisstionServices.UpdatePermisstionAsync(id, roleUpdateDto);

            if (result.IsSuccess)
            {
                return Ok("Permisstion đã được cập nhật.");
            }
            return BadRequest(result.ErrorMessage);
        }
        [Permisstion("Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermisstion(int id)
        {
            Result result = await _permisstionServices.DeletePermisstionAsync(id);

            if (result.IsSuccess)
            {
                return Ok("Permisstion đã được xóa thành công.");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
