using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Reponsitory;

namespace WebAPI_PhanTranMinhTam_New.Services
{
    public interface ICartServices
    {
        Task<Result> AddOrUpdateGiftInCartAsync(CreateGiftCartDTO addGiftToCartDto);
        Task<int> GetTotalGiftsInCartAsync(int cartId);
        Task<Result> RemoveAllCartItemsAsync(int userId);
        Task<Result> RemoveCartItemAsync(int cartId, int giftId);
    }
    public class CartServices : ICartServices
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public CartServices(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }
        public async Task<Result> AddOrUpdateGiftInCartAsync(CreateGiftCartDTO addGiftToCartDto)
        {
            // Tìm giỏ hàng
            Cart? cart = await _repositoryWrapper.Cart
                .FindByCondition(c => c.IdCart == addGiftToCartDto.CartId)
                .Include(c => c.Cart_items)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Giỏ hàng không tồn tại." };
            }

            // Tìm quà chính
            Gift? gift = await _repositoryWrapper.Gift
                .FindByCondition(g => g.IdGift == addGiftToCartDto.GiftId)
                .FirstOrDefaultAsync();

            if (gift == null)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính không tồn tại." };
            }

            if (gift.DateEnd.HasValue && gift.DateEnd < DateTime.Now)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính đã hết hạn." };
            }

            // Số lượng quà chính ban đầu là 1
            int quantityToAdd = 1;

            // Kiểm tra số lượng quà chính trong kho
            if (gift.Quantity < quantityToAdd)
            {
                return new Result { IsSuccess = false, ErrorMessage = "Quà chính không đủ số lượng trong kho." };
            }

            // Xử lý quà chính
            Cartitem? cartItemGift = cart.Cart_items
                .FirstOrDefault(ci => ci.IdGift == addGiftToCartDto.GiftId && ci.IdCart == addGiftToCartDto.CartId);

            if (cartItemGift != null)
            {
                // Cập nhật số lượng quà chính
                cartItemGift.QuantityGift += quantityToAdd;
                _repositoryWrapper.CartItem.Update(cartItemGift);
            }
            else
            {
                // Thêm mới quà chính vào giỏ hàng
                cartItemGift = new Cartitem
                {
                    IdCart = addGiftToCartDto.CartId,
                    IdGift = addGiftToCartDto.GiftId,
                    QuantityGift = quantityToAdd
                };
                _repositoryWrapper.CartItem.Create(cartItemGift);
            }

            // Cập nhật số lượng quà chính trong kho
            gift.Quantity -= quantityToAdd;
            _repositoryWrapper.Gift.Update(gift);

            // Xử lý quà khuyến mãi liên quan
            List<Gift> childGifts = await _repositoryWrapper.Gift
                .FindByCondition(g => g.Idparents == gift.IdGift)
                .ToListAsync();

            foreach (Gift? childGift in childGifts)
            {
                // Lấy quantityRequire từ quà khuyến mãi (childGift)
                int quantityRequire = childGift.QuantityRequired;

                // Kiểm tra số lượng quà khuyến mãi trong kho
                if (childGift.Quantity < quantityRequire)
                {
                    return new Result { IsSuccess = false, ErrorMessage = $"Quà khuyến mãi {childGift.Name} không đủ số lượng trong kho." };
                }

                Cartitem? cartItemChildGift = cart.Cart_items
                    .FirstOrDefault(ci => ci.IdGift == childGift.IdGift && ci.IdCart == addGiftToCartDto.CartId);

                if (cartItemChildGift != null)
                {
                    // Cập nhật số lượng quà khuyến mãi trong giỏ hàng
                    cartItemChildGift.QuantityGift += quantityRequire;
                    _repositoryWrapper.CartItem.Update(cartItemChildGift);
                }
                else
                {
                    // Thêm mới quà khuyến mãi vào giỏ hàng
                    cartItemChildGift = new Cartitem
                    {
                        IdCart = addGiftToCartDto.CartId,
                        IdGift = childGift.IdGift,
                        QuantityGift = quantityRequire
                    };
                    _repositoryWrapper.CartItem.Create(cartItemChildGift);
                }

                // Cập nhật số lượng quà khuyến mãi trong kho
                childGift.Quantity -= quantityRequire;
                _repositoryWrapper.Gift.Update(childGift);
            }

            await _repositoryWrapper.SaveAsync();

            return new Result { IsSuccess = true, ErrorMessage = "Quà đã được thêm vào giỏ hàng thành công." };
        }
        public async Task<int> GetTotalGiftsInCartAsync(int cartId)
        {
            // Truy vấn số lượng quà trong giỏ hàng
            int totalGifts = await _repositoryWrapper.CartItem
                .FindByCondition(ci => ci.IdCart == cartId)
                .SumAsync(ci => ci.QuantityGift);

            return totalGifts;
        }
        public async Task<Result> RemoveCartItemAsync(int cartId, int giftId)
        {
            Cart? cart = await _repositoryWrapper.Cart
                .FindByCondition(c => c.IdCart == cartId)
                .Include(c => c.Cart_items)
                .SingleOrDefaultAsync();

            if (cart == null)
            {
                return Result.Failure("Giỏ hàng không tồn tại.");
            }

            Cartitem? cartItem = cart.Cart_items.SingleOrDefault(ci => ci.IdGift == giftId);

            if (cartItem != null)
            {
                // Xóa món quà chính khỏi giỏ hàng
                _repositoryWrapper.CartItem.Delete(cartItem);

                // Xóa tất cả quà khuyến mãi liên quan
                List<Cartitem> relatedPromotionItems = cart.Cart_items
                    .Where(ci => ci.IdGift == giftId)
                    .ToList();

                foreach (Cartitem? promoItem in relatedPromotionItems)
                {
                    _repositoryWrapper.CartItem.Delete(promoItem);
                }

                try
                {
                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await _repositoryWrapper.SaveAsync();
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    return Result.Failure($"Lỗi khi lưu thay đổi: {ex.Message}");
                }
            }
            else
            {
                return Result.Failure("Không tìm thấy món hàng trong giỏ.");
            }

            return Result.Success();
        }

        public async Task<Result> RemoveAllCartItemsAsync(int userId)
        {
            // Lấy tất cả các giỏ hàng của người dùng
            List<Cart> carts = await _repositoryWrapper.Cart
                .FindByCondition(c => c.UserId == userId)
                .Include(c => c.Cart_items)
                .ToListAsync();

            if (carts == null || !carts.Any())
            {
                return Result.Failure("Không có giỏ hàng cho người dùng này.");
            }

            foreach (Cart? cart in carts)
            {
                foreach (Cartitem? cartItem in cart.Cart_items.ToList())
                {
                    _repositoryWrapper.CartItem.Delete(cartItem);
                }
            }


            await _repositoryWrapper.SaveAsync();

            return Result.Success();
        }
    }
}
