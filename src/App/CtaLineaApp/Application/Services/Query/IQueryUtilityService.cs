using CtaLinea.Application.Model.Query;

namespace CtaLineaApp.Application.Services.Query
{
    public interface IQueryUtilityService
    {
        string GetPRojection<TResource, TResult>();

        Task<QueryResult<TResul>?> GetListAsync<TResul>(
            QueryDefinition queryDef
            );
        Task<QueryResult<TResul>?> GetListByPostAsync<TResul>(
            QueryDefinition queryDef,
            object? payload
            );
        Task<TResul?> GetOneAsync<TResul>(
            QueryDefinition queryDef
            )
            where TResul : class;
    }
}