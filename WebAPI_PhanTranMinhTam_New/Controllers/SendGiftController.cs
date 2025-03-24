using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendGiftController : ControllerBase
    {
        private readonly ISendGiftServices _sendgiftServices;

        public SendGiftController(ISendGiftServices sendgiftServices)
        {
            _sendgiftServices = sendgiftServices;
        }
        //Them qua có qua khuyen mai 
        [Permisstion("Create")]
        [HttpPost("add-giftpromotion")]
        public async Task<IActionResult> AddGiftToSendPromotion([FromBody] AddGiftDTO addGiftDTO)
        {
            // Gọi phương thức AddGiftToSendAsync từ service
            Result result = await _sendgiftServices.AddGiftToSendPromotionAsync(addGiftDTO);

            if (result.IsSuccess)
            {
                return Ok(new { Message = result.ErrorMessage });
            }
            else
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }
        }
        //Them qua chinh
        [Permisstion("Create")]
        [HttpPost("add-gift")]
        public async Task<IActionResult> AddGiftToSend(int userid, [FromBody] AddGiftDTO addGiftDTO)
        {
            // Gọi phương thức AddGiftToSendAsync từ service
            Result result = await _sendgiftServices.AddGiftToSendAsync(userid, addGiftDTO);

            if (result.IsSuccess)
            {
                return Ok(new { Message = result.ErrorMessage });
            }
            else
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }
        }
        //Phat qua tu dong 
        [Permisstion("Create")]
        [HttpPost("schedule-gift-distribution")]
        public async Task<IActionResult> ScheduleGiftDistribution([FromBody] ScheduleGiftDistributionDto dto)
        {
            string jobId = await _sendgiftServices.ScheduleGiftDistributionAsync(dto);

            if (!string.IsNullOrEmpty(jobId))
            {
                return Ok(new { JobId = jobId });
            }

            return BadRequest(new { ErrorMessage = "Không thể lên lịch phát quà. Vui lòng kiểm tra thông tin đầu vào." });
        }

        //Xoa jobid
        [Permisstion("Delete")]
        [HttpDelete("CancelScheduledGiftDistribution/{jobId}")]
        public IActionResult CancelScheduledGiftDistribution(string jobId)
        {
            _sendgiftServices.CancelScheduledGiftDistribution(jobId);
            return NoContent();
        }
    }
}
