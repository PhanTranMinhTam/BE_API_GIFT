using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }
        [Permisstion("CreateCart")]
        [HttpPost("AddGiftPromotionToCart")]
        public async Task<IActionResult> AddGiftToCart([FromBody] CreateGiftCartDTO addGiftToCartDto)
        {
            Result result = await _cartServices.AddOrUpdateGiftInCartAsync(addGiftToCartDto);

            if (result.IsSuccess)
            {
                return Ok(result.ErrorMessage);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [Permisstion("ReadAllCart")]
        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetTotalGiftsInCart(int cartId)
        {
            // Gọi phương thức GetTotalGiftsInCartAsync từ CartService
            int totalGifts = await _cartServices.GetTotalGiftsInCartAsync(cartId);

            // Trả về kết quả
            return Ok(new { TotalGifts = totalGifts });
        }
        [Permisstion("DeleteCart")]
        [HttpDelete("{cartId}/items/{giftId}")]
        public async Task<IActionResult> RemoveCartItem(int cartId, int giftId)
        {
            Result result = await _cartServices.RemoveCartItemAsync(cartId, giftId);

            if (result.IsSuccess)
            {
                return Ok("Quà chính đã được xóa khỏi giỏ hàng.");
            }

            return BadRequest(result.ErrorMessage);
        }
        [Permisstion("DeleteCart")]
        [HttpDelete("user/{userId}/cart/items")]
        public async Task<IActionResult> RemoveAllCartItems(int userId)
        {
            Result result = await _cartServices.RemoveAllCartItemsAsync(userId);

            if (result.IsSuccess)
            {
                return Ok("Tất cả quà trong giỏ hàng của người dùng đã được xóa.");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
