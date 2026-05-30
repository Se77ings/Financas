using System;
using System.Threading.Tasks;
using Financas.Application.Services.CreditCardServices;
using Financeiro.Application.Models.CreditCardModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers.CreditCardController
{
	[ApiController]
	[Route("/api/[controller]")]
	public class CreditCardController : Controller
	{
		private readonly ICreditCardServices _services;

		public CreditCardController(ICreditCardServices services)
		{
			_services = services;
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var (result, lista) = await _services.GetListAsync();
			if (!result.IsSuccess)
				return BadRequest(result.Error.Message);

			return Ok(lista);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
		{
			var (result, data) = await _services.GetByIdAsync(id);
			if (!result.IsSuccess)
				return NotFound(result.Error.Message);

			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync([FromBody] CreditCardRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _services.AddAsync(request);
			if (!result.IsSuccess)
				return BadRequest(result.Error.Message);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreditCardRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _services.UpdateAsync(id, request);
			if (!result.IsSuccess)
				return BadRequest(result.Error.Message);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _services.DeleteAsync(id);
			if (!result.IsSuccess)
				return BadRequest(result.Error.Message);

			return Ok();
		}
	}
}
