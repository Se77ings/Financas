using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Business.Models.PayableModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CardExpenseController : Controller
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

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CardExpenseRequest request)
		{
			var result = await _services.UpdateAsync(id, request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _services.DeleteAsync(id);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var (result, lista) = await _services.GetListAsync();

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok(lista);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var (result, data) = await _services.GetByIdAsync(id);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok(data);
		}

		[HttpPost("{id}/items")]
		public async Task<IActionResult> AddItemAsync([FromRoute] Guid id, [FromBody] CardExpenseItemRequest request)
		{
			var result = await _services.AddItemAsync(id, request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);

			}
			return Ok();
		}

		[HttpPut("{id}/items/{itemId}")]
		public async Task<IActionResult> UpdateItemAsync([FromRoute] Guid id, [FromRoute] Guid itemId, [FromBody] CardExpenseItemRequest request)
		{
			var result = await _services.UpdateItemAsync(id, itemId, request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}

		[HttpDelete("{id}/items/{itemId}")]
		public async Task<IActionResult> RemoveItemAsync([FromRoute] Guid id, [FromRoute] Guid itemId)
		{
			var result = await _services.RemoveItemAsync(id, itemId);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}
	}
}