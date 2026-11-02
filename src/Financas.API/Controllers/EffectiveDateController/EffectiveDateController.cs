using Financas.Application.Services.EffectiveDateServices;
using Financeiro.Application.Models.EffectiveDateModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.EffectiveDateController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class EffectiveDateController : ApiControllerBase
	{
		private readonly IEffectiveDateServices _services;

		public EffectiveDateController(IEffectiveDateServices services)
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
		public async Task<IActionResult> AddAsync([FromBody] EffectiveDateRequest request)
		{
			var result = await _services.AddAsync(request);
			return CommandResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EffectiveDateRequest request)
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

		[HttpPost("{id}/sync-cards")]
		public async Task<IActionResult> SyncCardsAsync([FromRoute] Guid id)
		{
			// TODO: Este endpoint temporário de sincronização manual/sob demanda deve ser substituído por um fluxo baseado em Eventos de Domínio no futuro.
			var result = await _services.SyncMissingCardInvoicesAsync(id);
			return CommandResult(result);
		}

		[HttpGet("eligible-for-transfer")]
		public async Task<IActionResult> GetEligibleForTransferAsync()
		{
			var result = await _services.GetEligibleForTransferAsync();
			return QueryResult(result);
		}
	}
}
