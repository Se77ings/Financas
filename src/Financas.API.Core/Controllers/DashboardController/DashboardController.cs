using Financas.Application.Services.DashboardServices;
using Financeiro.Business.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _services;

        public DashboardController(IDashboardService services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? year, [FromQuery] CalendarMonth? month)
        {
            var result = await _services.GetAsync(year, month, DateOnly.FromDateTime(DateTime.Now));
            return QueryResult(result);
        }
    }
}
