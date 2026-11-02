using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.PayableModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ImmediateExpenseController : ApiControllerBase
	{
		private readonly IImmediateExpenseServices _services;

		public ImmediateExpenseController(IImmediateExpenseServices services)
		{
			_services = services;
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] ImmediateExpenseRequest request)
		{
			var result = await _services.AddAsync(request);
			return CommandResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ImmediateExpenseRequest request)
		{
			var result = await _services.UpdateAsync(id, request);
			return CommandResult(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _services.DeleteAsync(id);
			return CommandResult(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var result = await _services.GetByIdAsync(id);
			return QueryResult(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var result = await _services.GetListAsync();
			return QueryResult(result);
		}
	}
}