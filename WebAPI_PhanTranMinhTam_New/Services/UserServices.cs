using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IUserServices
    {
        Task AddUser(CreateDTO userDto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
    }
    public class UserServices : IUserServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        public UserServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Lấy tất cả người dùng từ cơ sở dữ liệu
            return await _repositoryWrapper.User.FindAll().ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            // Tìm người dùng hiện có trong DbContext dựa trên ID
            return await _repositoryWrapper.User.FindByCondition(u => u.IdUser == id).FirstOrDefaultAsync();
        }
        public async Task AddUser(CreateDTO userDto)
        {
            User user = _mapper.Map<User>(userDto); // Ánh xạ từ CreateGiftDTO sang Gift

            // Hash mật khẩu
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            if (userDto.Image != null && userDto.Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + userDto.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Lưu ảnh vào thư mục
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    await userDto.Image.CopyToAsync(fileStream);
                }

                user.ProfileImage = uniqueFileName; // Lưu tên tệp ảnh vào thuộc tính Image của Gift
            }

            _repositoryWrapper.User.Create(user);  // Lưu Gift vào repository
            await _repositoryWrapper.SaveAsync();   // Lưu thay đổi vào cơ sở dữ liệu
        }
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto)
        {
            // Tìm người dùng hiện có trong DbContext dựa trên ID
            User? existingUser = await _repositoryWrapper.User.FindByCondition(u => u.IdUser == id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return false; // Không tìm thấy người dùng để cập nhật
            }

            // Ánh xạ các thuộc tính từ DTO vào đối tượng đã tìm được
            _mapper.Map(userDto, existingUser);

            // Hash mật khẩu nếu có thay đổi
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            // Xử lý cập nhật ảnh đại diện nếu có
            if (userDto.Image != null && userDto.Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + userDto.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa ảnh cũ nếu có trước khi lưu ảnh mới
                if (!string.IsNullOrEmpty(existingUser.ProfileImage))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, existingUser.ProfileImage);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh mới vào thư mục
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    await userDto.Image.CopyToAsync(fileStream);
                }

                existingUser.ProfileImage = uniqueFileName; // Cập nhật tên tệp ảnh mới vào thuộc tính Image của User
            }

            // Cập nhật thông tin người dùng trong DbContext
            _repositoryWrapper.User.Update(existingUser);
            await _repositoryWrapper.SaveAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return true; // Cập nhật thành công
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            // Tìm người dùng hiện có trong DbContext dựa trên ID
            User? existingUser = await _repositoryWrapper.User.FindByCondition(u => u.IdUser == id).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return false; // Không tìm thấy người dùng để xóa
            }

            _repositoryWrapper.User.Delete(existingUser);
            await _repositoryWrapper.SaveAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return true; // Xóa thành công
        }

    }
}
