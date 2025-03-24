using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IPermisstionServices
    {
        Task<IEnumerable<Permission>> GetAllPermisstionAsync();
        Task<Permission> GetPermisstionByIdAsync(int id);
        Task<Result> CreatePermisstionAsync(PermisstionDTO permisstionCreateDto);
        Task<Result> UpdatePermisstionAsync(int id, PermisstionDTO permisstionUpdateDto);
        Task<Result> DeletePermisstionAsync(int id);


    }
    public class PermistionServices : IPermisstionServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public PermistionServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<IEnumerable<Permission>> GetAllPermisstionAsync()
        {
            // Lấy tất cả người dùng từ cơ sở dữ liệu
            return await _repositoryWrapper.Permisstion.FindAll().ToListAsync();
        }
        public async Task<Permission> GetPermisstionByIdAsync(int id)
        {
            // Tìm người dùng hiện có trong DbContext dựa trên ID
            return await _repositoryWrapper.Permisstion.FindByCondition(u => u.IdPermission == id).FirstOrDefaultAsync();
        }
        public async Task<Result> CreatePermisstionAsync(PermisstionDTO permisstionCreateDto)
        {
            // Kiểm tra vai trò đã tồn tại chưa
            Permission? existingPermisstion = await _repositoryWrapper.Permisstion
                .FindByCondition(r => r.Name == permisstionCreateDto.Name)
                .SingleOrDefaultAsync();

            if (existingPermisstion != null)
            {
                return Result.Failure("Permisstion đã tồn tại.");
            }

            // Tạo mới vai trò
            Permission permisstion = new()
            {
                Name = permisstionCreateDto.Name
            };

            _repositoryWrapper.Permisstion.Create(permisstion);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }

        public async Task<Result> UpdatePermisstionAsync(int id, PermisstionDTO permisstionUpdateDto)
        {
            Permission? existingPermisstion = await _repositoryWrapper.Permisstion
                .FindByCondition(r => r.IdPermission == id)
                .SingleOrDefaultAsync();

            if (existingPermisstion == null)
            {
                return Result.Failure("Permisstion không tồn tại.");
            }

            // Cập nhật thông tin vai trò
            existingPermisstion.Name = permisstionUpdateDto.Name;

            _repositoryWrapper.Permisstion.Update(existingPermisstion);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
        public async Task<Result> DeletePermisstionAsync(int id)
        {
            Permission? permission = await _repositoryWrapper.Permisstion
                .FindByCondition(r => r.IdPermission == id)
                .SingleOrDefaultAsync();

            if (permission == null)
            {
                return Result.Failure("Permisstion không tồn tại.");
            }

            // Xóa vai trò
            _repositoryWrapper.Permisstion.Delete(permission);

            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
    }
}
