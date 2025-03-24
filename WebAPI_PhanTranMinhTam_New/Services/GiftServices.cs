using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IGiftServices
    {
        Task AddGift(CreateGiftDTO giftDto);
        Task<IEnumerable<Gift>> GetAllGiftsAsync();
        Task<Gift> GetGiftByIdAsync(int id);
        Task<Gift> UpdateGiftAsync(int id, UpdateGiftDTO giftDto);
        Task<bool> DeleteGiftAsync(int id);
    }
    public class GiftServices : IGiftServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public GiftServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        //Xem tat ca danh sách 
        public async Task<IEnumerable<Gift>> GetAllGiftsAsync()
        {
            // Lấy tất cả các quà từ repository
            IQueryable<Gift> gifts = _repositoryWrapper.Gift.FindAll();
            return await gifts.ToListAsync();
        }
        //Xem 1 quà 
        public async Task<Gift> GetGiftByIdAsync(int id)
        {
            // Lấy món quà theo ID từ repository
            Gift? gift = await _repositoryWrapper.Gift.FindByCondition(g => g.IdGift == id).FirstOrDefaultAsync();
            return gift;
        }
        //them 
        public async Task AddGift(CreateGiftDTO giftDto)
        {
            Gift gift = _mapper.Map<Gift>(giftDto); // Ánh xạ từ CreateGiftDTO sang Gift

            if (giftDto.Image != null && giftDto.Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + giftDto.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Lưu ảnh vào thư mục
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    await giftDto.Image.CopyToAsync(fileStream);
                }

                gift.Image = uniqueFileName; // Lưu tên tệp ảnh vào thuộc tính Image của Gift
            }

            _repositoryWrapper.Gift.Create(gift);  // Lưu Gift vào repository
            await _repositoryWrapper.SaveAsync();   // Lưu thay đổi vào cơ sở dữ liệu
        }
        // Update 
        public async Task<Gift> UpdateGiftAsync(int id, UpdateGiftDTO giftDto)
        {
            // Tìm đối tượng Gift hiện có trong DbContext dựa trên ID
            Gift? existingGift = await _repositoryWrapper.Gift.FindByCondition(g => g.IdGift == id).FirstOrDefaultAsync();

            if (existingGift == null)
            {
                return null; // Trả về null nếu không tìm thấy đối tượng Gift
            }

            // Ánh xạ các thuộc tính từ giftDto vào đối tượng đã tìm được
            _mapper.Map(giftDto, existingGift);

            // Xử lý hình ảnh nếu có
            if (!string.IsNullOrEmpty(giftDto.Image))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + giftDto.Image;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Lưu ảnh vào thư mục
                // Nếu có ảnh cũ, xóa ảnh cũ trước khi lưu ảnh mới
                if (!string.IsNullOrEmpty(existingGift.Image))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, existingGift.Image);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh mới
                // Chú ý: Nếu `giftDto.Image` là một tệp `IFormFile`, cần phải lấy `FileName` từ đó
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    // Cần có phương thức để sao chép dữ liệu từ `IFormFile` vào `fileStream`
                }

                existingGift.Image = uniqueFileName; // Cập nhật tên tệp ảnh mới vào thuộc tính Image của Gift
            }

            // Cập nhật đối tượng Gift trong DbContext
            _repositoryWrapper.Gift.Update(existingGift);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _repositoryWrapper.SaveAsync();

            return existingGift;
        }
        //Delete 
        public async Task<bool> DeleteGiftAsync(int id)  // Triển khai phương thức xóa
        {
            Gift? existingGift = await _repositoryWrapper.Gift.FindByCondition(g => g.IdGift == id).FirstOrDefaultAsync();

            if (existingGift == null)
            {
                return false; // Không tìm thấy đối tượng để xóa
            }

            _repositoryWrapper.Gift.Delete(existingGift);

            await _repositoryWrapper.SaveAsync();

            return true; // Xóa thành công
        }
    }
}
