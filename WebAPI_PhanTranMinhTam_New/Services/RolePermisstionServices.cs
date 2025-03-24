using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IPermisstionRolwServices
    {
        Task AddPermissionsToRoleAsync(PermisstionRoleDTO addDto);
        Task UpdateRolePermissionsAsync(PermisstionRoleDTO updateDto);
        Task DeleteRolePermissionAsync(int roleId, int permissionId);
        Task<IEnumerable<PermisstionDTO>> GetRolePermissionsAsync(int roleId);
    }
    public class RolePermisstionServices : IPermisstionRolwServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public RolePermisstionServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        public async Task<IEnumerable<PermisstionDTO>> GetRolePermissionsAsync(int roleId)
        {
            Role? role = await _repositoryWrapper.Role.FindByCondition(r => r.IdRoles == roleId)
                                                    .Include(r => r.RolePermissions)
                                                        .ThenInclude(rp => rp.Permission)
                                                    .FirstOrDefaultAsync();

            if (role == null)
            {
                return Enumerable.Empty<PermisstionDTO>();
            }

            return role.RolePermissions.Select(rp => new PermisstionDTO
            {
                IdPermission = rp.Permission.IdPermission,
                Name = rp.Permission.Name
            });
        }

        public async Task AddPermissionsToRoleAsync(PermisstionRoleDTO addDto)
        {
            // Lấy thông tin Role từ database
            Role? role = await _repositoryWrapper.Role.FindByCondition(r => r.IdRoles == addDto.RoleId)
                                                    .Include(r => r.RolePermissions)
                                                    .FirstOrDefaultAsync();

            if (role == null)
            {
                throw new Exception("Role không tồn tại.");
            }

            // Thêm các Permission mới vào Role nếu chưa tồn tại
            foreach (int permissionId in addDto.PermissionIds)
            {
                if (!role.RolePermissions.Any(rp => rp.IdPermission == permissionId))
                {
                    role.RolePermissions.Add(new Rolepermission
                    {
                        IdRole = addDto.RoleId,
                        IdPermission = permissionId
                    });
                }
            }

            // Lưu thay đổi vào database
            _repositoryWrapper.Role.Update(role);
            await _repositoryWrapper.SaveAsync();
        }
        public async Task UpdateRolePermissionsAsync(PermisstionRoleDTO updateDto)
        {
            // Lấy thông tin Role từ database
            Role? role = await _repositoryWrapper.Role.FindByCondition(r => r.IdRoles == updateDto.RoleId)
                                                    .Include(r => r.RolePermissions)
                                                    .FirstOrDefaultAsync();

            if (role == null)
            {
                throw new Exception("Role không tồn tại.");
            }

            // Lấy danh sách các IdPermission hiện có trong Role
            List<int> currentPermissionIds = role.RolePermissions.Select(rp => rp.IdPermission).ToList();

            // Tìm các IdPermission cần xóa
            List<int> permissionsToRemove = currentPermissionIds.Except(updateDto.PermissionIds).ToList();

            // Tìm các IdPermission cần thêm mới
            List<int> permissionsToAdd = updateDto.PermissionIds.Except(currentPermissionIds).ToList();

            // Xóa các RolePermission cần xóa
            foreach (int permissionId in permissionsToRemove)
            {
                Rolepermission? rolePermissionToRemove = role.RolePermissions.FirstOrDefault(rp => rp.IdPermission == permissionId);
                if (rolePermissionToRemove != null)
                {
                    role.RolePermissions.Remove(rolePermissionToRemove);
                }
            }

            // Thêm các RolePermission mới
            foreach (int permissionId in permissionsToAdd)
            {
                role.RolePermissions.Add(new Rolepermission
                {
                    IdRole = updateDto.RoleId,
                    IdPermission = permissionId
                });
            }

            // Lưu thay đổi vào database
            _repositoryWrapper.Role.Update(role);
            await _repositoryWrapper.SaveAsync();
        }
        public async Task DeleteRolePermissionAsync(int roleId, int permissionId)
        {
            Rolepermission? rolePermission = await _repositoryWrapper.RolePermisstion.FindByCondition(rp => rp.IdRole == roleId && rp.IdPermission == permissionId)
                                                                          .FirstOrDefaultAsync();

            if (rolePermission == null)
            {
                throw new Exception("RolePermission không tồn tại.");
            }

            _repositoryWrapper.RolePermisstion.Delete(rolePermission);
            await _repositoryWrapper.SaveAsync();
        }
    }
}
