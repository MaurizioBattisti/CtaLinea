using CtaLinea.Application.Model.Query;
using CtaLineaApp.Application.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace CtaLineaApp.Application.Services.Query
{
    public class QueryUtilityService 
        : IQueryUtilityService
    {
        private const int DEfaultPageSize = 20;

        private const string Args_FullText = "ft";
        private const string Args_Filter = "f";
        private const string Args_Order = "s";
        private const string Args_PRojection = "p";
        private const string Args_Count = "count";
        private const string Args_Page = "page";
        private const string Args_PageSize = "pagesize";
        private const string Args_Skip = "skip";

        private const string Args_Equal = "=";
        private const string Args_Separator = "&";

        private readonly IHttpService _httpService;

        public QueryUtilityService(
             IHttpService httpService
            )
        {
            _httpService = httpService;
        }

        public async Task<TResul?> GetOneAsync<TResul>(
            QueryDefinition queryDef
            )
            where TResul : class
        {
            try
            {
                return await this._httpService.Get<TResul>(
                    queryDef.EndPoint ?? string.Empty);
            }
            catch (Exception exc)
            {
                Console.WriteLine("erroracccio: " + exc.Message);
                return null;
            }
        }
        public async Task<QueryResult<TResul>?> GetListAsync<TResul>(
            QueryDefinition queryDef
            )
        {
            return await this.SetRequestListAsync<TResul>(queryDef);
        }
        public async Task<QueryResult<TResul>?> GetListByPostAsync<TResul>(
            QueryDefinition queryDef,
            object? payload
            )
        {
            return await this.SetRequestListAsync<TResul>(
                queryDef,
                async(u) => await this._httpService.Post(u, payload)
                );
        }

        private async Task<QueryResult<TResul>?> SetRequestListAsync<TResul> (
            QueryDefinition queryDef,
            Func<string, Task<HttpResponseMessage?>>? sendAsync = null
            )
        {
            if (sendAsync == null) sendAsync = async (u) => await this._httpService.Get(u);

            int totalRows = -1;

            try
            {
                string queryString = this.GetQueryString(
                    queryDef.FullText,
                    queryDef.Filter,
                    queryDef.Sort,
                    string.Empty,
                    queryDef.Page,
                    queryDef.PageSize,
                    queryDef.Count,
                    queryDef.Skip
                    );
                if (string.IsNullOrEmpty(queryString) == false)
                {
                    queryString = "?" + queryString;
                }
                
                // esegue la chiamata vera e propria all'endpoint
                var response = await sendAsync (queryDef.EndPoint + queryString);

                if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var items = await response.Content.ReadFromJsonAsync<TResul[]>();

                        totalRows = response.Headers.ParseInt(Constants.ResponseHeader_TotalRows);

                        // se tutto OK
                        return new QueryResult<TResul>(items, totalRows, queryDef.Page);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Errore durante la deserializzazione: " + exc.Message);
                        Console.WriteLine(await response.Content.ReadAsStringAsync());
                        throw;
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine("erroracccio: " + exc.Message);
                return null;
            }
        }

        private string GetQueryString(
            string? fullText,
            string? filter,
            string? sort,
            string? projection,
            int page,
            int pageSize,
            bool returnCount,
            int skip = 0
            )
        {
            var sb = new StringBuilder(1024);
            if (pageSize == 0) pageSize = DEfaultPageSize;
            if (pageSize < 0) pageSize = int.MaxValue;

            this.AddArgument(sb, Args_FullText, fullText);
            this.AddArgument(sb, Args_Filter, filter);
            this.AddArgument(sb, Args_Order, sort);
            this.AddArgument(sb, Args_PRojection, projection);

            if (page > 0)
            {
                this.AddArgument(sb, Args_Page, page.ToString());
            }
            this.AddArgument(sb, Args_PageSize, pageSize.ToString());
            if (skip > 0)
            {
                this.AddArgument(sb, Args_Skip, skip.ToString());
            }

            if (returnCount == true)
            {
                this.AddArgument(sb, Args_Count, true.ToString());
            }

            return sb.ToString();
        }

        public string GetPRojection<TResource, TResult>()
        {
            var baseType = typeof(TResource);
            var prjType = typeof(TResult);
            if (baseType == prjType) return null;
            var sb = new StringBuilder(1024);
            foreach (var p in prjType.GetProperties())
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(p.Name);
            }

            return sb.ToString();
        }

        private StringBuilder AddArgument(
            StringBuilder sb,
            string argName,
            string? argValue)
        {
            if (string.IsNullOrWhiteSpace(argValue) == false)
            {
                argValue = argValue.Trim();
                if (sb.Length > 0) sb.Append(Args_Separator);
                sb.Append(argName);
                sb.Append(Args_Equal);
                sb.Append(argValue);
            }
            return sb;
        }
    }

    internal static class HttpResponse_HeadersExtensions
    {
        internal static int ParseInt(
            this HttpResponseHeaders headers, 
            string key)
        {
            if (headers == null) return 0;

            string? value = headers.GetValues(key)
                ?.FirstOrDefault();

            if (string.IsNullOrEmpty(value)) return 0;

            int.TryParse(value, out var number);
            return number;
        }
    }
}
