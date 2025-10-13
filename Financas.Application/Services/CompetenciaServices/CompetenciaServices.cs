using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Direito;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Models.CompetenciaModels;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Shared.Localization.ErrorMessages;

namespace Financas.Application.Services.CompetenciaServices
{
	public class CompetenciaServices : ServiceBase, ICompetenciaServices
	{
		private readonly ICompetenciaRepository _repository;
		public CompetenciaServices(ICompetenciaRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(CompetenciaRequest request)
		{
			var competencia = new Competencia(request.Mes, request.Ano);

			// TODO: Verificar se já existe essa entidade.

			var validation = await ValidateEntityAsync<Competencia, CompetenciaValidator>(competencia);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(competencia);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, CompetenciaRequest request)
		{

			var competencia = await ObterCompetenciaPorId(id);
			//TODO: Verificar se já existe a nova entidade ou homônima.

			if (competencia == null)
				return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(Competencia))));

			var validation = await ValidateEntityAsync<Competencia, CompetenciaValidator>(competencia);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			competencia.Atualizar(request.Mes, request.Ano);

			await _repository.UpdateAsync(competencia);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var competencia = await ObterCompetenciaPorId(id);

			if (competencia == null)
				return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(Competencia))));

			await _repository.RemoveAsync(competencia);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, CompetenciaView view)> GetByIdAsync(Guid id)
		{
			var competencia = await ObterCompetenciaPorId(id);

			return (Result<bool>.Success(true), (CompetenciaView)competencia);
		}

		public async Task<(Result<bool> result, List<CompetenciaSummary> lista)> GetListAsync()
		{

			var list = await _repository.GetListAsync();
			// TODO: Mudar para implicit operator
			return (Result<bool>.Success(true), list.Select(x => new CompetenciaSummary { Id = x.Id, Ano = x.Ano, Mes = x.Mes }).ToList());
		}

		#region Métodos internos
		public async Task<Competencia> ObterCompetenciaPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
