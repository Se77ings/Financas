using Financas.Application.Services.CompetenciaServices;
using Financeiro.Business.Models.CompetenciaModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.CompetenciaController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class CompetenciaController : Controller
	{
		private readonly ICompetenciaServices _services;

		public CompetenciaController(ICompetenciaServices services)
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
		public async Task<IActionResult> AddAsync([FromBody] CompetenciaRequest request)
		{
			var result = await _services.AddAsync(request);

			if (!result.IsSuccess)
				return NotFound(result.Error.Message);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CompetenciaRequest request)
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

	}
}
