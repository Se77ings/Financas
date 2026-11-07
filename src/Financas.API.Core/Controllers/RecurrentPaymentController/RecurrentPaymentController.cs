using Financas.Application.Services.RecurrentPaymentServices;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.RecurrentPaymentController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class RecurrentPaymentController : ApiControllerBase
	{
		private readonly IRecurrentPaymentServices _services;

		public RecurrentPaymentController(IRecurrentPaymentServices services)
		{
			_services = services;
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var result = await _services.GetListAsync();
			return QueryResult(result);
		}

		[HttpGet("credit-card/{creditCardId:guid}")]
		public async Task<IActionResult> GetByCreditCardAsync([FromRoute] Guid creditCardId)
		{
			var result = await _services.GetByCreditCardAsync(creditCardId);
			return QueryResult(result);
		}

		[HttpGet("immediate-expense")]
		public async Task<IActionResult> GetImmediateExpenseRecurrentPaymentsAsync()
		{
			var result = await _services.GetImmediateExpenseRecurrentPaymentsAsync();
			return QueryResult(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var result = await _services.GetByIdAsync(id);
			return QueryResult(result);
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] RecurrentPaymentRequest request)
		{
			var result = await _services.AddAsync(request);
			return CommandResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] RecurrentPaymentRequest request)
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
	}
}
