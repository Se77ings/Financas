using System;
using System.Threading.Tasks;
using Financas.Application.Services.CreditCardServices;
using Financeiro.Application.Models.CreditCardModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.CreditCardController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class CreditCardController : ApiControllerBase
	{
		private readonly ICreditCardServices _services;

		public CreditCardController(ICreditCardServices services)
		{
			_services = services;
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

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] CreditCardRequest request)
		{
			var result = await _services.AddAsync(request);
			return CommandResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreditCardRequest request)
		{
			var result = await _services.UpdateAsync(id, request);
			return CommandResult(result);
		}

		[HttpPut("{id}/activate")]
		public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
		{
			var result = await _services.ActivateAsync(id);
			return CommandResult(result);
		}

		[HttpPut("{id}/deactivate")]
		public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
		{
			var result = await _services.DeactivateAsync(id);
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
