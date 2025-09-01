using Financas.Application.Services.ObrigaçãoServices.Interfaces; // Namespace da sua service
using Financeiro.Business.Models.ObrigaçãoModels;             // Namespace dos seus DTOs
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DespesaAVistaController : Controller
	{
		private readonly IDespesaAVistaServices _services;

		public DespesaAVistaController(IDespesaAVistaServices services)
		{
			_services = services;
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] DespesaAVistaRequest request)
		{
			var result = await _services.AddAsync(request);

			if (!result.IsSuccess)
			{
				return BadRequest(result.Error.Message);
			}
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] DespesaAVistaRequest request)
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
	}
}