using Financas.Application.Services.CategoryServices;
using Financeiro.Application.Models.CategoryModels;
using Financeiro.Application.Models.EntryModels;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ApiControllerBase
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
        public async Task<IActionResult> AddAsync([FromBody] CategoryRequest request)
        {
            var result = await _services.AddAsync(request);
            return QueryResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CategoryRequest request)
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

        [HttpPut("link/payables")]
        public async Task<IActionResult> LinkPayablesAsync([FromBody] CategoryLinkRequest request)
        {
            var result = await _services.LinkPayablesAsync(request.Ids, request.CategoryId);
            return CommandResult(result);
        }

        [HttpPut("link/receivables")]
        public async Task<IActionResult> LinkReceivablesAsync([FromBody] CategoryLinkRequest request)
        {
            var result = await _services.LinkReceivablesAsync(request.Ids, request.CategoryId);
            return CommandResult(result);
        }

        [HttpGet("uncategorized/entries")]
        public async Task<IActionResult> GetUncategorizedEntriesAsync()
        {
            var result = await _managementServices.GetUncategorizedEntriesAsync();
            return QueryResult(result);
        }

        [HttpGet("{id:guid}/entries")]
        public async Task<IActionResult> GetEntriesByCategoryAsync([FromRoute] Guid id)
        {
            var result = await _managementServices.GetEntriesByCategoryAsync(id);
            return QueryResult(result);
        }

        [HttpGet("entries/categorized-descriptions")]
        public async Task<IActionResult> GetCategorizedDescriptionsAsync()
        {
            var result = await _managementServices.GetCategorizedDescriptionsAsync();
            return QueryResult(result);
        }

        [HttpGet("entries/search")]
        public async Task<IActionResult> SearchEntriesAsync([FromQuery] string? term)
        {
            var result = await _managementServices.SearchEntriesAsync(term);
            return QueryResult(result);
        }

        [HttpPut("entries/description")]
        public async Task<IActionResult> RenameEntriesAsync([FromBody] RenameEntriesRequest request)
        {
            var result = await _managementServices.RenameEntriesAsync(request);
            return CommandResult(result);
        }
    }
}
