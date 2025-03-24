using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IRoleServices
    {
        Task<Result> DeleteRoleAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Result> CreateRoleAsync(RoleDTO roleCreateDto);
        Task<Result> UpdateRoleAsync(int id, RoleDTO roleUpdateDto);

    }
    public class RoleServices : IRoleServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public RoleServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            // Lấy tất cả người dùng từ cơ sở dữ liệu
            return await _repositoryWrapper.Role.FindAll().ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            // Tìm người dùng hiện có trong DbContext dựa trên ID
            return await _repositoryWrapper.Role.FindByCondition(u => u.IdRoles == id).FirstOrDefaultAsync();
        }

        public async Task<Result> CreateRoleAsync(RoleDTO roleCreateDto)
        {
            // Kiểm tra vai trò đã tồn tại chưa
            Role? existingRole = await _repositoryWrapper.Role
                .FindByCondition(r => r.Name == roleCreateDto.Name)
                .SingleOrDefaultAsync();

            if (existingRole != null)
            {
                return Result.Failure("Vai trò đã tồn tại.");
            }

            // Tạo mới vai trò
            Role role = new()
            {
                Name = roleCreateDto.Name
            };

            _repositoryWrapper.Role.Create(role);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
        public async Task<Result> UpdateRoleAsync(int id, RoleDTO roleUpdateDto)
        {
            // Tìm vai trò hiện có
            Role? existingRole = await _repositoryWrapper.Role
                .FindByCondition(r => r.IdRoles == id)
                .SingleOrDefaultAsync();

            if (existingRole == null)
            {
                return Result.Failure("Vai trò không tồn tại.");
            }

            // Cập nhật thông tin vai trò
            existingRole.Name = roleUpdateDto.Name;

            _repositoryWrapper.Role.Update(existingRole);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
        public async Task<Result> DeleteRoleAsync(int id)
        {
            Role? role = await _repositoryWrapper.Role
                .FindByCondition(r => r.IdRoles == id)
                .SingleOrDefaultAsync();

            if (role == null)
            {
                return Result.Failure("Vai trò không tồn tại.");
            }

            // Xóa vai trò
            _repositoryWrapper.Role.Delete(role);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
    }
}
