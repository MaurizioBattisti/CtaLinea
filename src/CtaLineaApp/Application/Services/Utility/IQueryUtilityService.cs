using CtaLinea.Application.Model.Utility;
using System.Net.Http;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Services.Utility
{
    public interface IQueryUtilityService
    {
        string GetPRojection<TResource, TResult>();

        Task<QueryResult<TResul>?> GetListAsync<TResul>(
            QueryDefinition queryDef,
            bool usePostMethod = false,
            object? payload = null
            );
        Task<TResul?> GetOneAsync<TResul>(
            QueryDefinition queryDef
            )
            where TResul : class;
    }
}