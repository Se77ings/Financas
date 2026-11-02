using Financas.Application.Services.DashboardServices;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _services;

        public DashboardController(IDashboardService services)
        {
            _services = services;
        }

        [HttpGet("{effectiveDateId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid effectiveDateId)
        {
            var (result, data) = await _services.GetAsync(effectiveDateId, DateOnly.FromDateTime(DateTime.Now));

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(data);
        }
    }
}
