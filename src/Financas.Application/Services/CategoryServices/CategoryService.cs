using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.CategoryModels;
using Financeiro.Business.Aggregates.CategoryValidators;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;
using CategoryEntity = Financeiro.Business.Aggregates.Category.Category;

namespace Financas.Application.Services.CategoryServices
{
    public class CategoryService : ServiceBase, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<(Result<bool> result, CategoryView view)> AddAsync(CategoryRequest request)
        {
            if (await _categoryRepository.ExistsAsync(request.Name))
                return (Result<bool>.Failure(new Error("Já existe uma categoria com esse nome.")), null);

            var category = new CategoryEntity(request.Name, request.Description);
            var validation = await ValidateEntityAsync<CategoryEntity, CategoryValidator>(category);

            if (!validation.Success)
                return (Result<bool>.Failure(validation.Error), null);

            await _categoryRepository.AddAsync(category);
            return (Result<bool>.Success(true), (CategoryView)category);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, CategoryRequest request)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category is null)
                return Result<bool>.Failure(new Error("Categoria não encontrada."));

            if (await _categoryRepository.ExistsOtherAsync(id, request.Name))
                return Result<bool>.Failure(new Error("Já existe uma categoria com esse nome."));

            var updatedCategory = new CategoryEntity(request.Name, request.Description);
            var validation = await ValidateEntityAsync<CategoryEntity, CategoryValidator>(updatedCategory);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            category.Update(request.Name, request.Description);
            await _categoryRepository.UpdateAsync(category);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category is null)
                return Result<bool>.Failure(new Error("Categoria não encontrada."));

            var linkedEntries = await _categoryRepository.CountLinkedEntriesAsync(id);
            if (linkedEntries > 0)
                return Result<bool>.Failure(new Error($"Não é possível excluir a categoria pois existem {linkedEntries} lançamento(s) vinculado(s). Remova o vínculo antes de prosseguir."));

            await _categoryRepository.RemoveAsync(category);
            return Result<bool>.Success(true);
        }

        public async Task<(Result<bool> result, CategoryView view)> GetByIdAsync(Guid id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category is null)
                return (Result<bool>.Failure(new Error("Categoria não encontrada.")), null);

            return (Result<bool>.Success(true), (CategoryView)category);
        }

        public async Task<(Result<bool> result, List<CategoryView> data)> GetListAsync()
        {
            var categories = await _categoryRepository.GetListAsync();
            var counts = await _categoryRepository.CountLinkedEntriesByCategoryAsync();

            var data = categories.Select(category =>
            {
                CategoryView view = category;
                view.EntriesCount = counts.GetValueOrDefault(category.Id);
                return view;
            }).ToList();

            return (Result<bool>.Success(true), data);
        }


        public async Task<Result<bool>> LinkPayablesAsync(IEnumerable<Guid> payableIds, Guid categoryId)
        {
            var ids = payableIds.Distinct().ToList();
            if (ids.Count == 0)
                return Result<bool>.Failure(new Error("Nenhuma conta a pagar selecionada."));

            var categoryResult = await ValidateCategoryAsync(categoryId);
            if (!categoryResult.IsSuccess)
                return categoryResult;

            if (await _categoryRepository.CountExistingPayablesAsync(ids) != ids.Count)
                return Result<bool>.Failure(new Error("Uma ou mais contas a pagar não foram encontradas."));

            await _categoryRepository.SetPayablesCategoryAsync(ids, categoryId);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> LinkReceivablesAsync(IEnumerable<Guid> receivableIds, Guid categoryId)
        {
            var ids = receivableIds.Distinct().ToList();
            if (ids.Count == 0)
                return Result<bool>.Failure(new Error("Nenhuma conta a receber selecionada."));

            var categoryResult = await ValidateCategoryAsync(categoryId);
            if (!categoryResult.IsSuccess)
                return categoryResult;

            if (await _categoryRepository.CountExistingReceivablesAsync(ids) != ids.Count)
                return Result<bool>.Failure(new Error("Uma ou mais contas a receber não foram encontradas."));

            await _categoryRepository.SetReceivablesCategoryAsync(ids, categoryId);
            return Result<bool>.Success(true);
        }

        private Task<CategoryEntity> GetCategoryByIdAsync(Guid id)
        {
            return _categoryRepository.GetByIdAsync(id);
        }

        private async Task<Result<bool>> ValidateCategoryAsync(Guid categoryId)
        {
            var category = await GetCategoryByIdAsync(categoryId);
            return category is null
                ? Result<bool>.Failure(new Error("Categoria não encontrada."))
                : Result<bool>.Success(true);
        }
    }
}
