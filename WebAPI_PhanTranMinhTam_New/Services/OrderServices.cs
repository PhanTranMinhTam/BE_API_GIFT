using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IOrderServices
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Result> CreateOrderAsync(int userId);
        Task<Result> AddGiftToSendAsync(AddGiftDTO addGiftToDTO);
        Task<PagedResult<Order>> GetOrdersByUserIdAsync(int userId, int pageNumber, int pageSize);

    }
    public class OrderServices : IOrderServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public OrderServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        //Xem tat ca danh sách 
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Lấy tất cả các quà từ repository
            IQueryable<Order> Orders = _repositoryWrapper.Order.FindAll();
            return await Orders.ToListAsync();
        }
        //Xem 1 đơn hàng 
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            // Lấy món quà theo ID từ repository
            Order? order = await _repositoryWrapper.Order.FindByCondition(g => g.IdOrder == id).FirstOrDefaultAsync();
            return order;
        }
        public async Task<Result> CreateOrderAsync(int userId)
        {
            // Tạo đơn hàng mới
            Order newOrder = new()
            {
                UserId = userId, // ID người dùng liên quan
                Date = DateTime.Now, // Ngày tạo đơn hàng
                Total = 0 // Tổng tiền ban đầu là 0
            };

            // Thêm đơn hàng vào cơ sở dữ liệu
            _repositoryWrapper.Order.Create(newOrder);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _repositoryWrapper.SaveAsync();

            return new Result { IsSuccess = true, ErrorMessage = "Đơn hàng đã được tạo thành công." };
        }
        public async Task<Result> AddGiftToSendAsync(AddGiftDTO addGiftToDTO)
        {
            // Tìm đơn hàng
            Order? order = await _repositoryWrapper.Order
                .FindByCondition(o => o.IdOrder == addGiftToDTO.orderId)
                .Include(o => o.Order_items)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Đơn hàng không tồn tại." };
            }

            // Tìm quà chính
            Gift? gift = await _repositoryWrapper.Gift
                .FindByCondition(g => g.IdGift == addGiftToDTO.giftId)
                .FirstOrDefaultAsync();

            if (gift == null)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính không tồn tại." };
            }

            if (gift.DateEnd.HasValue && gift.DateEnd < DateTime.Now)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính đã hết hạn." };
            }

            if (gift.Quantity < 1)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính hết hàng." };
            }

            // Tính giá quà chính
            int giftPrice = int.TryParse(gift.Price, out int price) ? price : 0;

            // Kiểm tra nếu quà có IdParent
            Gift? parentGift = null;
            if (gift.Idparents.HasValue)
            {
                parentGift = await _repositoryWrapper.Gift
                    .FindByCondition(g => g.IdGift == gift.Idparents.Value)
                    .FirstOrDefaultAsync();

                if (parentGift == null)
                {
                    return new Result { IsSuccess = false, ErrorMessage = "Quà cha (parent gift) không tồn tại." };
                }
            }

            // Kiểm tra nếu đã có OrderItem cho quà chính hoặc quà khuyến mãi
            Orderitem? existingOrderItemGift = order.Order_items
                .FirstOrDefault(oi => oi.IdGift == addGiftToDTO.giftId);

            if (existingOrderItemGift != null)
            {
                // Cập nhật số lượng quà chính
                existingOrderItemGift.QuantityGift += 1;

                // Cập nhật tổng tiền của đơn hàng
                order.Total += giftPrice;

                // Cập nhật OrderItem
                _repositoryWrapper.OrderItem.Update(existingOrderItemGift);
            }
            else
            {
                // Thêm mới OrderItem cho quà chính
                Orderitem orderItemGift = new()
                {
                    IdOrder = addGiftToDTO.orderId,
                    IdGift = addGiftToDTO.giftId,
                    QuantityGift = 1,
                    IdParent = gift.Idparents // Nếu có quà cha, lưu vào OrderItem
                };
                _repositoryWrapper.OrderItem.Create(orderItemGift);

                // Cập nhật tổng tiền của đơn hàng
                order.Total += giftPrice;
            }

            // Cập nhật số lượng quà chính trong kho
            gift.Quantity -= 1;
            _repositoryWrapper.Gift.Update(gift);

            // Cập nhật ngày đặt hàng
            order.Date = DateTime.Now;

            // Cập nhật điểm kiếm được dựa trên tổng tiền của đơn hàng
            int pointsEarned = order.Total / 10000; // 1 điểm cho mỗi 10,000 VND
            order.EarnedPoints = pointsEarned; // Cập nhật EarnedPoints cho đơn hàng
            _repositoryWrapper.Order.Update(order);

            // Tính và cập nhật điểm cho người dùng
            User? user = await _repositoryWrapper.User
                .FindByCondition(u => u.IdUser == order.UserId)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.Point += pointsEarned;
                _repositoryWrapper.User.Update(user);
            }

            await _repositoryWrapper.SaveAsync();

            return new Result { IsSuccess = true, ErrorMessage = "Đã thêm quà vào đơn hàng và tích điểm thành công." };
        }

        public async Task<PagedResult<Order>> GetOrdersByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            // Lọc các đơn hàng theo userId
            IQueryable<Order> query = _repositoryWrapper.Order
                .FindByCondition(o => o.UserId == userId);

            // Lấy tổng số lượng đơn hàng sau khi lọc
            int totalRecords = await query.CountAsync();

            // Thực hiện phân trang
            List<Order> orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PagedResult<Order>
            {
                Items = orders,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
