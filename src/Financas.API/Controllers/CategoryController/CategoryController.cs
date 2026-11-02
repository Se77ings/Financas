using Financas.Application.Services.CategoryServices;
using Financeiro.Application.Models.CategoryModels;
using Financeiro.Application.Models.EntryModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _services;
        private readonly ICategoryManagementService _managementServices;

        public CategoryController(ICategoryService services, ICategoryManagementService managementServices)
        {
            _services = services;
            _managementServices = managementServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var (result, data) = await _services.GetListAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(data);
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

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CategoryRequest request)
        {
            var (result, data) = await _services.AddAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CategoryRequest request)
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

        [HttpPut("link/payables")]
        public async Task<IActionResult> LinkPayablesAsync([FromBody] CategoryLinkRequest request)
        {
            var result = await _services.LinkPayablesAsync(request.Ids, request.CategoryId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok();
        }

        [HttpPut("link/receivables")]
        public async Task<IActionResult> LinkReceivablesAsync([FromBody] CategoryLinkRequest request)
        {
            var result = await _services.LinkReceivablesAsync(request.Ids, request.CategoryId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok();
        }

        [HttpGet("uncategorized/entries")]
        public async Task<IActionResult> GetUncategorizedEntriesAsync()
        {
            var (result, data) = await _managementServices.GetUncategorizedEntriesAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(data);
        }

        [HttpGet("{id:guid}/entries")]
        public async Task<IActionResult> GetEntriesByCategoryAsync([FromRoute] Guid id)
        {
            var (result, data) = await _managementServices.GetEntriesByCategoryAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(data);
        }

        [HttpPut("entries/description")]
        public async Task<IActionResult> RenameEntriesAsync([FromBody] RenameEntriesRequest request)
        {
            var result = await _managementServices.RenameEntriesAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok();
        }
    }
}
