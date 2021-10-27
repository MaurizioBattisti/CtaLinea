using System.Net.Http;
using System.Threading.Tasks;

namespace CtaLinea.Application.Utility
{
    public interface IQueryUtilityService
    {
        HttpClient Http { get; }
        string GetCompleteUrl(string endpoint);

        string GetPRojection<TResource, TResult>();

        Task<QueryResult<TResul>> GetListAsync<TResul>(
            QueryDefinition queryDef
            );
        Task<TResul> GetOneAsync<TResul>(
            QueryDefinition queryDef
            )
            where TResul : class;
    }
}