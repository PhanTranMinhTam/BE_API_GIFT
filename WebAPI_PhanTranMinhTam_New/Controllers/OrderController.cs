using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;

        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;

        }
        [Permisstion("CreateOrder")]
        //Thêm order
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateGift(int userid)
        {
            if (ModelState.IsValid)
            {
                await _orderServices.CreateOrderAsync(userid);
                return Ok();
            }

            return BadRequest(ModelState);
        }
        [Permisstion("CreateOrder")]
        //Them orderitem
        [HttpPost("createOrderitem")]
        public async Task<IActionResult> CreateGift([FromForm] AddGiftDTO sendgiftDto)
        {
            if (ModelState.IsValid)
            {
                await _orderServices.AddGiftToSendAsync(sendgiftDto);
                return Ok();
            }
            return BadRequest(ModelState);
        }
        [Permisstion("ReadOrder")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId, int pageNumber = 1, int pageSize = 10)
        {
            PagedResult<Data.Order> pagedResult = await _orderServices.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);

            return Ok(pagedResult);
        }

    }
}
