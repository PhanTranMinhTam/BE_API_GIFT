using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermisstionRoleController : ControllerBase
    {
        private readonly IPermisstionRolwServices _permisstionRolwServices;

        public PermisstionRoleController(IPermisstionRolwServices permisstionRolwServices)
        {
            _permisstionRolwServices = permisstionRolwServices;
        }
        [Permisstion("Read")]
        [HttpGet("GetRolePermissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            IEnumerable<PermisstionDTO> permissions = await _permisstionRolwServices.GetRolePermissionsAsync(roleId);

            if (permissions == null || !permissions.Any())
            {
                return BadRequest("Không tìm thấy quyền cho Role này.");
            }

            return Ok(permissions);
        }
        [Permisstion("Create")]
        [HttpPost("AddPermissionsToRole")]
        public async Task<IActionResult> AddPermissionsToRole([FromBody] PermisstionRoleDTO addDto)
        {
            await _permisstionRolwServices.AddPermissionsToRoleAsync(addDto);
            return Ok("Đã thêm quyền thành công.");
        }
        [Permisstion("Update")]
        [HttpPut("UpdateRolePermissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] PermisstionRoleDTO updateDto)
        {
            await _permisstionRolwServices.UpdateRolePermissionsAsync(updateDto);
            return Ok("Cập nhật quyền thành công.");
        }
        [Permisstion("Delete")]
        [HttpDelete("DeleteRolePermission")]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
        {
            await _permisstionRolwServices.DeleteRolePermissionAsync(roleId, permissionId);
            return Ok("Đã xóa quyền thành công.");
        }
    }
}
