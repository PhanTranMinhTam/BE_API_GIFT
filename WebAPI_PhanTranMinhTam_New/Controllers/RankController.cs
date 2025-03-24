using Microsoft.AspNetCore.Mvc;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Services;

namespace WebAPI_PhanTranMinhTam_New.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RankController : ControllerBase
    {
        private readonly IRankServices _rankServices;

        public RankController(IRankServices rankServices)
        {
            _rankServices = rankServices;

        }
        [Permisstion("Create")]
        [HttpPost]
        public async Task<IActionResult> Rank(int month, int year)
        {
            if (ModelState.IsValid)
            {
                await _rankServices.CalculateUserRankByMonthAsync(month, year);
                return Ok();
            }

            return BadRequest(ModelState);
        }
        [Permisstion("Create")]
        [HttpPost("RankTop3")]
        public async Task<IActionResult> RankTop3(int month, int year)
        {
            if (ModelState.IsValid)
            {
                await _rankServices.DistributeGiftsToTopRankedUsersAsync(month, year);
                return Ok();
            }

            return BadRequest(ModelState);
        }
        [Permisstion("Read")]
        [HttpGet]
        public async Task<IActionResult> GetRankings(int pageNumber = 1, int pageSize = 10, int month = 9, int year = 2024)
        {
            Models.PagedResult<Data.UserActivity> result = await _rankServices.GetRankedUsersAsync(pageNumber, pageSize, month, year);
            return Ok(result);
        }
    }
}
