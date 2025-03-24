using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Models;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftServices _giftServices;

        public GiftController(IGiftServices giftServices)
        {
            _giftServices = giftServices;

        }
        [Permisstion("ReadGift")]
        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            IEnumerable<Data.Gift> gifts = await _giftServices.GetAllGiftsAsync();
            return Ok(gifts);
        }
        [Permisstion("ReadGift")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            Data.Gift gift = await _giftServices.GetGiftByIdAsync(id);
            if (gift == null)
            {
                return BadRequest();
            }

            return Ok(gift);
        }
        [Permisstion("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateGift([FromForm] CreateGiftDTO giftDto)
        {
            if (ModelState.IsValid)
            {
                await _giftServices.AddGift(giftDto);
                return Ok();
            }

            return BadRequest(ModelState);
        }
        [Permisstion("Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGift(int id, [FromBody] UpdateGiftDTO giftDto)
        {
            if (ModelState.IsValid)
            {
                Data.Gift updatedGift = await _giftServices.UpdateGiftAsync(id, giftDto);
                if (updatedGift == null)
                {
                    return BadRequest();
                }

                return Ok(updatedGift);
            }

            return BadRequest(ModelState);
        }
        [Permisstion("Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            bool result = await _giftServices.DeleteGiftAsync(id);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(result);

        }
    }
}
