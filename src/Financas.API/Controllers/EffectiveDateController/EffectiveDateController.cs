using Financas.Application.Services.CompetenciaServices;
using Financeiro.Application.Models.EffectiveDateModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.EffectiveDateController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class EffectiveDateController : Controller
	{
		private readonly IEffectiveDateServices _services;

		public EffectiveDateController(IEffectiveDateServices services)
		{
			_services = services;
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var (result, lista) = await _services.GetListAsync();

			if (!result.IsSuccess)
			{
				if (lista == null)
				{
					return NotFound(result.Error.Message);
				}

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
				if (data == null)
				{
					return NotFound(result.Error.Message);
				}

				return BadRequest(result.Error.Message);
			}
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] EffectiveDateRequest request)
		{
			var result = await _services.AddAsync(request);

			if (!result.IsSuccess)
				return NotFound(result.Error.Message);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EffectiveDateRequest request)
		{
			var result = await _services.UpdateAsync(id, request);

			if (!result.IsSuccess)
				return NotFound(result.Error.Message);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _services.DeleteAsync(id);

			if (!result.IsSuccess)
				return NotFound(result.Error.Message);

			return Ok();

		}

		[HttpPost("{id}/sync-cards")]
		public async Task<IActionResult> SyncCardsAsync([FromRoute] Guid id)
		{
			// TODO: Este endpoint temporário de sincronização manual/sob demanda deve ser substituído por um fluxo baseado em Eventos de Domínio no futuro.
			var result = await _services.SyncMissingCardInvoicesAsync(id);

			if (!result.IsSuccess)
				return BadRequest(result.Error.Message);

			return Ok();
		}

	}
}
