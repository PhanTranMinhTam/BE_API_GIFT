using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface ISendGiftServices
    {
        //qua co khuyen mai
        Task<Result> AddGiftToSendPromotionAsync(AddGiftDTO addGiftToDTO);
        //qua khong khuyen mai
        Task<Result> AddGiftToSendAsync(int userid, AddGiftDTO addGiftDTO);
        //qua tu dong 
        Task<string> ScheduleGiftDistributionAsync(ScheduleGiftDistributionDto dto);
        Task CancelScheduledGiftDistribution(string jobId);
        Task DistributeGiftsToAllWardrobesAsync(int giftId);
    }
    public class SendGiftServices : ISendGiftServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public SendGiftServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        //Them qua chinh có qua khuyen mai 
        public async Task<Result> AddGiftToSendPromotionAsync(AddGiftDTO addGiftToDTO)
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

            // Kiểm tra quà khuyến mãi liên quan
            bool hasPromotion = false;

            if (gift.Idparents.HasValue)
            {
                Gift? parentGift = await _repositoryWrapper.Gift
                    .FindByCondition(g => g.IdGift == gift.Idparents.Value)
                    .FirstOrDefaultAsync();

                if (parentGift == null)
                {
                    return new Result { IsSuccess = false, ErrorMessage = "Quà cha không tồn tại." };
                }

                if (parentGift.Quantity < gift.QuantityRequired)
                {
                    return new Result { IsSuccess = false, ErrorMessage = "Số lượng quà cha không đủ để kích hoạt quà chính." };
                }

                // Tạo Orderitem cho quà cha
                Orderitem orderItemParent = new()
                {
                    IdOrder = addGiftToDTO.orderId,
                    IdGift = gift.Idparents.Value,
                    IdParent = gift.Idparents,
                    QuantityGift = gift.QuantityRequired
                };
                _repositoryWrapper.OrderItem.Create(orderItemParent);

                // Cập nhật số lượng quà cha trong kho
                parentGift.Quantity -= gift.QuantityRequired;
                _repositoryWrapper.Gift.Update(parentGift);

                // Đánh dấu có quà khuyến mãi
                hasPromotion = true;
            }

            if (hasPromotion)
            {
                // Tạo hoặc cập nhật Orderitem cho quà chính
                Orderitem? orderItemGift = await _repositoryWrapper.OrderItem
                    .FindByCondition(oi => oi.IdOrder == addGiftToDTO.orderId && oi.IdGift == addGiftToDTO.giftId)
                    .FirstOrDefaultAsync();

                if (orderItemGift != null)
                {
                    // Cập nhật số lượng quà chính
                    orderItemGift.QuantityGift += 1;
                    _repositoryWrapper.OrderItem.Update(orderItemGift);
                }
                else
                {
                    // Thêm mới Orderitem cho quà chính
                    orderItemGift = new Orderitem
                    {
                        IdOrder = addGiftToDTO.orderId,
                        IdGift = addGiftToDTO.giftId,
                        QuantityGift = 1
                    };
                    _repositoryWrapper.OrderItem.Create(orderItemGift);
                }

                // Cập nhật số lượng quà chính trong kho
                gift.Quantity -= 1;
                _repositoryWrapper.Gift.Update(gift);

                // Cập nhật tổng tiền trong đơn hàng
                int giftPrice = int.TryParse(gift.Price, out int price) ? price : 0;
                order.Total += giftPrice;
                order.Date = DateTime.Now;
                _repositoryWrapper.Order.Update(order);

                await _repositoryWrapper.SaveAsync();

                return new Result { IsSuccess = true, ErrorMessage = "Đã thêm quà vào đơn hàng thành công." };
            }
            else
            {
                return new Result { IsSuccess = false, ErrorMessage = "Không có khuyến mãi để thêm quà chính." };
            }
        }
        //Them qua chinh 
        public async Task<Result> AddGiftToSendAsync(int userid, AddGiftDTO addGiftDTO)
        {
            // Tìm đơn hàng
            Order? order = await _repositoryWrapper.Order
                .FindByCondition(o => o.IdOrder == addGiftDTO.orderId)
                .Include(o => o.Order_items)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                // Nếu đơn hàng không tồn tại, thêm đơn hàng mới
                order = new Order
                {
                    UserId = userid,
                    Date = DateTime.Now,
                    Total = 0
                };
                _repositoryWrapper.Order.Create(order);
            }

            // Tìm quà chính
            Gift? gift = await _repositoryWrapper.Gift
                .FindByCondition(g => g.IdGift == addGiftDTO.giftId)
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

            // Nếu quà chính có quà cha, kiểm tra và thêm quà cha
            if (gift.Idparents.HasValue)
            {
                Gift? parentGift = await _repositoryWrapper.Gift
                    .FindByCondition(g => g.IdGift == gift.Idparents.Value)
                    .FirstOrDefaultAsync();

                if (parentGift == null)
                {
                    return new Result { IsSuccess = false, ErrorMessage = "Quà cha không tồn tại." };
                }

                if (parentGift.Quantity < gift.QuantityRequired)
                {
                    return new Result { IsSuccess = false, ErrorMessage = "Số lượng quà cha không đủ để kích hoạt quà chính." };
                }

                // Tạo Orderitem cho quà cha
                Orderitem orderItemParent = new()
                {
                    IdOrder = addGiftDTO.orderId,
                    IdGift = gift.Idparents.Value,
                    IdParent = gift.Idparents,
                    QuantityGift = gift.QuantityRequired
                };
                _repositoryWrapper.OrderItem.Create(orderItemParent);

                // Cập nhật số lượng quà cha trong kho
                parentGift.Quantity -= gift.QuantityRequired;
                _repositoryWrapper.Gift.Update(parentGift);
            }

            // Tạo hoặc cập nhật Orderitem cho quà chính
            Orderitem? orderItemGift = await _repositoryWrapper.OrderItem
                .FindByCondition(oi => oi.IdOrder == addGiftDTO.orderId && oi.IdGift == addGiftDTO.giftId)
                .FirstOrDefaultAsync();

            if (orderItemGift != null)
            {
                // Cập nhật số lượng quà chính
                orderItemGift.QuantityGift += 1;
                _repositoryWrapper.OrderItem.Update(orderItemGift);
            }
            else
            {
                // Thêm mới Orderitem cho quà chính
                orderItemGift = new Orderitem
                {
                    IdOrder = addGiftDTO.orderId,
                    IdGift = addGiftDTO.giftId,
                    QuantityGift = 1
                };
                _repositoryWrapper.OrderItem.Create(orderItemGift);
            }

            // Cập nhật số lượng quà chính trong kho
            gift.Quantity -= 1;
            _repositoryWrapper.Gift.Update(gift);

            // Cập nhật tổng tiền trong đơn hàng
            int giftPrice = int.TryParse(gift.Price, out int price) ? price : 0;
            order.Total += giftPrice;
            order.Date = DateTime.Now;
            _repositoryWrapper.Order.Update(order);

            await _repositoryWrapper.SaveAsync();

            return new Result { IsSuccess = true, ErrorMessage = "Đã thêm quà vào đơn hàng thành công." };
        }
        //Them qua tu dong 
        // Tạo job tự động phát quà
        public async Task<string> ScheduleGiftDistributionAsync(ScheduleGiftDistributionDto dto)
        {
            if (!DateTime.TryParseExact(dto.TimeOfDay, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeOfDay))
            {
                throw new ArgumentException("Thời gian không hợp lệ. Định dạng phải là HH:mm:ss.");
            }

            DateTime now = DateTime.Now;
            DateTime scheduleDateTime = new(now.Year, now.Month, now.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);

            if (scheduleDateTime <= now)
            {
                scheduleDateTime = scheduleDateTime.AddDays(1);
            }

            string jobId = BackgroundJob.Schedule(() => DistributeGiftsToAllWardrobesAsync(dto.GiftId), scheduleDateTime);
            return jobId;
        }

        // Hủy job đã lên lịch
        public async Task CancelScheduledGiftDistribution(string jobId)
        {
            BackgroundJob.Delete(jobId);
        }
        public async Task DistributeGiftsToAllWardrobesAsync(int giftId)
        {
            // Lấy tất cả các tủ đồ của người dùng
            IQueryable<Order> orders = _repositoryWrapper.Order.FindAll();

            // Tìm quà chính cần phát
            Gift? gift = await _repositoryWrapper.Gift
                .FindByCondition(g => g.IdGift == giftId)
                .FirstOrDefaultAsync();

            if (gift == null || gift.Quantity < 1)
            {
                // Nếu quà không tồn tại hoặc hết hàng, thoát khỏi phương thức
                return;
            }

            foreach (Order order in orders)
            {
                // Tạo hoặc cập nhật Orderitem cho quà chính trong tủ đồ của người dùng
                Orderitem orderItem = new()
                {
                    IdOrder = order.IdOrder, // Hoặc cần tạo đơn hàng mới nếu cần
                    IdGift = gift.IdGift,
                    QuantityGift = 1
                };

                _repositoryWrapper.OrderItem.Create(orderItem);

                // Cập nhật số lượng quà trong kho
                gift.Quantity -= 1;
                _repositoryWrapper.Gift.Update(gift);

                // Cập nhật tổng tiền trong đơn hàng nếu cần
                Order? existingOrder = await _repositoryWrapper.Order
                    .FindByCondition(o => o.IdOrder == order.IdOrder)
                    .FirstOrDefaultAsync();

                if (existingOrder != null)
                {
                    int giftPrice = int.TryParse(gift.Price, out int price) ? price : 0;
                    existingOrder.Total += giftPrice;
                    existingOrder.Date = DateTime.Now;
                    _repositoryWrapper.Order.Update(existingOrder);
                }
            }

            await _repositoryWrapper.SaveAsync();
        }
    }
}
