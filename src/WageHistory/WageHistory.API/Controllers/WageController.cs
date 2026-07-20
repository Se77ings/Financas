using Microsoft.AspNetCore.Mvc;
using WageHistory.Application.Models.WageModels;
using WageHistory.Application.Services.WageServices;

namespace WageHistory.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WageController : Controller
	{
		private readonly IWageServices _services;

		public WageController(IWageServices services)
		{
			_services = services;
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var (result, lista) = await _services.GetListAsync();

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error?.Message ?? "An error occurred");
			}
			return Ok(lista);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var (result, data) = await _services.GetByIdAsync(id);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error?.Message ?? "An error occurred");
			}
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] WageRequest request)
		{
			var result = await _services.AddAsync(request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error?.Message ?? "An error occurred");
			}
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] WageRequest request)
		{
			var result = await _services.UpdateAsync(id, request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error?.Message ?? "An error occurred");
			}
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _services.DeleteAsync(id);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error?.Message ?? "An error occurred");
			}
			return Ok();
		}
	}
}
