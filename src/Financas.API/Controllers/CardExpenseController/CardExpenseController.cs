using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.PayableModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CardExpenseController : ApiControllerBase
	{
		private readonly ICardExpenseServices _services;

		public CardExpenseController(ICardExpenseServices services)
		{
			_services = services;
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] CardExpenseRequest request)
		{
			var result = await _services.AddAsync(request);
			return CommandResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CardExpenseRequest request)
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

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var result = await _services.GetListAsync();
			return QueryResult(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var result = await _services.GetByIdAsync(id);
			return QueryResult(result);
		}

		[HttpPost("{id}/items")]
		public async Task<IActionResult> AddItemAsync([FromRoute] Guid id, [FromBody] CardExpenseItemRequest request)
		{
			var result = await _services.AddItemAsync(id, request);
			return CommandResult(result);
		}

		[HttpPut("{id}/items/{itemId}")]
		public async Task<IActionResult> UpdateItemAsync([FromRoute] Guid id, [FromRoute] Guid itemId, [FromBody] CardExpenseItemRequest request)
		{
			var result = await _services.UpdateItemAsync(id, itemId, request);
			return CommandResult(result);
		}

		[HttpDelete("{id}/items/{itemId}")]
		public async Task<IActionResult> RemoveItemAsync([FromRoute] Guid id, [FromRoute] Guid itemId)
		{
			var result = await _services.RemoveItemAsync(id, itemId);
			return CommandResult(result);
		}

		[HttpPost("transfer-items")]
		public async Task<IActionResult> TransferItemsAsync([FromBody] CardExpenseItemTransferRequest request)
		{
			var result = await _services.TransferItemsAsync(request);
			return CommandResult(result);
		}
	}
}