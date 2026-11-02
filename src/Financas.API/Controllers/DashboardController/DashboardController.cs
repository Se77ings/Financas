using Financas.Application.Services.DashboardServices;
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

        [HttpGet("{effectiveDateId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid effectiveDateId)
        {
            var result = await _services.GetAsync(effectiveDateId, DateOnly.FromDateTime(DateTime.Now));
            return QueryResult(result);
        }
    }
}
