using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace ZzSoft.Api.Utility.Base
{
    public class ZControllerBase
        : ControllerBase
    {
        private const string Query_FullText = "ft";
        private const string Query_Filter = "f";
        private const string Query_OrderBy = "s";
        private const string Query_Projection = "p";
        private const string Query_Page = "page";
        private const string Query_PageSize = "pagesize";
        private const string Query_Skip = "skip";
        private const string Query_Count = "count";

        private const string ResponseHeader_TotalRows = "X-Total-Count";

        private const string ProjectionFieldSeparator = ",";

        private IFilteringContext _filteringContext;

        public IFilteringContext FilteringContext
        {
            get
            {
                if (this._filteringContext == null)
                {
                    this._filteringContext = this.GetIFilteringContext();
                }
                return this._filteringContext;
            }
        }

        public async Task<ActionResult> ModelOKAsync<T> (
            QueryItemList<T> modelList
            )
            where T : class
        {
            IEnumerable<object> model;

            // se è stato richiesto il conteggio
            if (this.FilteringContext.Count == true)
            {
                // restituisce il conteggio
                this.Response.Headers.Add(
                    ResponseHeader_TotalRows,
                    modelList.TotalRows.ToString());
            }
            var filedNames = this.GetProjectionFieldNames();
            // se viene richiesta la proiezione
            if (filedNames != null)
            {
                model = ProjectData<T>(modelList.Items, filedNames);
            }
            else
            {
                // se non c'è proiezione restituisce la lista dei modelli
                model = modelList.Items;
            }

            // restituisce i dati proiettati o meno
            return this.Ok(
                await Task.FromResult(model)
                    .ConfigureAwait(false)
                );
        }
        public async Task<ActionResult> ModelOKAsync<T>(
            T model
            )
            where T : class
        {
            if (model == null)
            {
                await Task.CompletedTask;
                return this.NotFound();
            }
            // restituisce il modello
            return this.Ok(
                await Task.FromResult(model)
                    .ConfigureAwait(false)
                );
        }
        private IFilteringContext GetIFilteringContext()
        {
            int page = 0;
            int pagesize = Constants.DefaultPageSize;
            int skip = 0;
            bool showCount = false;
            string fullText = null;
            string filter = null;
            string orderBy = null;
            string projection = null;

            // recupera i valori dalla querystring
            if (this.HttpContext.Request.Query.Keys.Contains(Query_FullText) == true)
            {
                fullText = this.HttpContext.Request.Query[Query_FullText];
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_Filter) == true)
            {
                filter = this.HttpContext.Request.Query[Query_Filter];
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_OrderBy) == true)
            {
                orderBy = this.HttpContext.Request.Query[Query_OrderBy];
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_Projection) == true)
            {
                projection = this.HttpContext.Request.Query[Query_Projection];
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_Page) == true)
            {
                page = int.Parse(this.HttpContext.Request.Query[Query_Page]);
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_PageSize) == true)
            {
                pagesize = int.Parse(this.HttpContext.Request.Query[Query_PageSize]);
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_Skip) == true)
            {
                skip = int.Parse(this.HttpContext.Request.Query[Query_Skip]);
            }
            if (this.HttpContext.Request.Query.Keys.Contains(Query_Count) == true)
            {
                var tmp = this.HttpContext.Request.Query[Query_Count];
                if (bool.TryParse(tmp, out bool newBool) == true)
                {
                    showCount = newBool;
                }
                else if (int.TryParse(tmp, out int newInt)  == true)
                {
                    showCount = newInt == 1;
                }
            }

            // se la pagina richiesta è negativa solleva un'eccezzione PagePutOfRangeException
            if (page < 0)
            {
                page = 0;
            }
            // se la pgesize è negativa restitui tuti i dati
            if (pagesize < 0)
            {
                pagesize = int.MaxValue;
            }
            return new FilteringContext(
                fullText,
                filter, orderBy, projection,
                page, pagesize, showCount,
                skip
                );
        }

        private  IEnumerable<string> GetProjectionFieldNames()
        {
            IEnumerable<string> list = null;

            if (string.IsNullOrEmpty (this._filteringContext.Projection) == false)
            {
                list = this.FilteringContext.Projection.Split(ProjectionFieldSeparator);
            }

            return list;
        }

        private IEnumerable<dynamic> ProjectData<T>(
            IEnumerable<T> list,
            IEnumerable<string> fieldNames
            )
        {
            var fields = new List<string>();
            foreach (string f in fieldNames)
            {
                fields.Add(f.ToUpper());
            }

            var t = typeof(T);
            var proprs = t.GetProperties()
                .Where(p => fields.Contains(p.Name.ToUpper()));
            foreach (T item in list)
            {
                dynamic result = new System.Dynamic.ExpandoObject();
                IDictionary<string, object> resultDict = result;
                foreach (var p in proprs)
                {
                    object value = p.GetValue(item);
                    resultDict.Add(p.Name, value);
                }
                yield return result;
            }
        }
    }
}
