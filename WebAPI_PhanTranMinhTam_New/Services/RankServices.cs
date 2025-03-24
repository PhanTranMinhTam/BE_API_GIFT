using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface IRankServices
    {
        Task CalculateUserRankByMonthAsync(int month, int year);
        Task<Result> DistributeGiftsToTopRankedUsersAsync(int month, int year);
        Task<PagedResult<UserActivity>> GetRankedUsersAsync(int pageNumber, int pageSize, int month, int year);
    }
    public class RankServices : IRankServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public RankServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        public async Task CalculateUserRankByMonthAsync(int month, int year)
        {
            // Lấy tổng điểm tích lũy của mỗi người dùng từ bảng Order trong tháng và năm đã chọn
            var userPoints = await _repositoryWrapper.Order
                .FindByCondition(o => o.Date.HasValue && o.Date.Value.Month == month && o.Date.Value.Year == year)
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(o => o.EarnedPoints) // Tính tổng điểm EarnedPoints từ Order
                })
                .OrderByDescending(up => up.TotalPoints) // Sắp xếp theo tổng điểm từ cao xuống thấp
                .ToListAsync();
            // Cập nhật bảng UserActivity với tổng điểm và thứ hạng
            int rank = 1; // Thứ hạng bắt đầu từ 1
            foreach (var userPoint in userPoints)
            {
                // Tìm hoặc tạo mới UserActivity cho tháng và năm hiện tại
                UserActivity? userActivity = await _repositoryWrapper.UserActivity
                    .FindByCondition(ua => ua.UserId == userPoint.UserId && ua.Month == month && ua.Year == year)
                    .FirstOrDefaultAsync();

                if (userActivity != null)
                {
                    // Nếu đã có UserActivity, cập nhật điểm và thứ hạng
                    userActivity.Points = userPoint.TotalPoints;
                    userActivity.Rank = rank;
                    _repositoryWrapper.UserActivity.Update(userActivity);
                }
                else
                {
                    // Nếu chưa có UserActivity, tạo mới bản ghi
                    UserActivity newUserActivity = new()
                    {
                        UserId = userPoint.UserId,
                        Points = userPoint.TotalPoints,
                        Rank = rank,
                        Month = month,
                        Year = year
                    };
                    _repositoryWrapper.UserActivity.Create(newUserActivity);
                }

                rank++; // Tăng thứ hạng cho người tiếp theo
            }
            // Lưu các thay đổi vào cơ sở dữ liệu
            await _repositoryWrapper.SaveAsync();
        }
        public async Task<Result> DistributeGiftsToTopRankedUsersAsync(int month, int year)
        {
            // Lấy danh sách người dùng có rank 1, 2, 3 trong tháng và năm đã chọn
            List<UserActivity> topRankedUsers = await _repositoryWrapper.UserActivity
                .FindByCondition(ua => (ua.Rank == 1 || ua.Rank == 2 || ua.Rank == 3) && ua.Month == month && ua.Year == year)
                .ToListAsync();

            if (!topRankedUsers.Any())
            {
                return new Result { IsSuccess = false, ErrorMessage = "Không có người dùng nào đạt hạng 1, 2, hoặc 3 trong tháng này." };
            }

            foreach (UserActivity? userActivity in topRankedUsers)
            {
                Gift? gift = null;

                // Phát quà dựa trên thứ hạng
                if (userActivity.Rank == 1)
                {
                    // Phát quà đặc biệt cho người đứng top 1
                    gift = await _repositoryWrapper.Gift
                        .FindByCondition(g => g.IdGift == 1 && g.Quantity > 0)
                        .FirstOrDefaultAsync();
                }
                else if (userActivity.Rank == 2 || userActivity.Rank == 3)
                {
                    // Phát quà bình thường cho top 2 và 3
                    gift = await _repositoryWrapper.Gift
                        .FindByCondition(g => g.IdGift == 2 && g.Quantity > 0)
                        .FirstOrDefaultAsync();
                }

                if (gift != null)
                {
                    // Tìm đơn hàng đã có của người dùng
                    List<Orderitem> existingOrderItems = await _repositoryWrapper.OrderItem
                        .FindByCondition(oi => oi.Gifts.IdGift == gift.IdGift && oi.Orders.UserId == userActivity.UserId)
                        .ToListAsync();

                    if (!existingOrderItems.Any())
                    {
                        // Nếu không có orderitem nào, có thể tạo mới một Orderitem
                        Orderitem newOrderItem = new()
                        {
                            IdOrder = 0, // Chưa có IdOrder nếu không tạo mới Order
                            IdGift = gift.IdGift,
                            QuantityGift = 1 // Số lượng quà
                        };

                        _repositoryWrapper.OrderItem.Create(newOrderItem);
                    }
                    else
                    {
                        // Nếu đã có orderitem, chỉ cần cập nhật số lượng
                        Orderitem existingOrderItem = existingOrderItems.First();
                        existingOrderItem.QuantityGift += 1; // Tăng số lượng quà

                        _repositoryWrapper.OrderItem.Update(existingOrderItem);
                    }

                    // Giảm số lượng quà sau khi phát
                    gift.Quantity -= 1;
                    _repositoryWrapper.Gift.Update(gift);
                }
            }

            await _repositoryWrapper.SaveAsync(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

            return new Result { IsSuccess = true, ErrorMessage = "Quà đã được phát vào bảng Orderitem cho người dùng có rank 1, 2, 3." };
        }
        public async Task<PagedResult<UserActivity>> GetRankedUsersAsync(int pageNumber, int pageSize, int month, int year)
        {
            // Tính tổng số bản ghi
            int totalRecords = await _repositoryWrapper.UserActivity
                .FindByCondition(ua => ua.Month == month && ua.Year == year)
                .CountAsync();

            // Lấy danh sách người dùng đã xếp hạng với phân trang
            List<UserActivity> rankedUsers = await _repositoryWrapper.UserActivity
                .FindByCondition(ua => ua.Month == month && ua.Year == year)
                .OrderBy(ua => ua.Rank) // Sắp xếp theo thứ hạng
                .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước
                .Take(pageSize) // Lấy số lượng bản ghi tương ứng với pageSize
                .ToListAsync();

            return new PagedResult<UserActivity>
            {
                Items = rankedUsers,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}
