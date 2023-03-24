using CtaLinea.Application.Model.Utility;
using Radzen;

namespace CtaLineaApp.Application.Services.Utility
{
    public class AutoLoadDataList<TItem> 
        : IAutoLoadDataList<TItem> 
        where TItem : class
    {
        private bool _hasBeenInitialized = false;
        private readonly IQueryUtilityService _queryService;
        private readonly QueryDefinition _QueryDef;
        private bool _usePost = false;
        private object? _postPayload;
        private bool _virtualized = false;

        public AutoLoadDataList(
            IQueryUtilityService queryService)
        {
            _QueryDef = new QueryDefinition();
            _queryService = queryService;
        }

        public void Initilize(
            string endpoint,
            bool usePost = false,
            object? postPayload = null,
            bool virtualized = false
            )
        {
            _QueryDef.EndPoint = endpoint;
            _hasBeenInitialized = true;
            _usePost = usePost;
            _postPayload = postPayload;
            _virtualized = virtualized;
        }

        public string FullTextSearch { get; set; } = string.Empty;
        public bool isLoading { get; private set; }
        public int RowCount { get; private set; }
        public IList<TItem>? Data { get; private set; }

        public void StartLoading()
        {
            isLoading = true;
        }
        public void EndLoading()
        {
            isLoading = false;
        }

        private bool UsePost ()
        {
            return (this._usePost == true
                || this._postPayload != null);
        }

        public async Task LoadData(
            LoadDataArgs args)
        {
            if (_hasBeenInitialized == false) return;
            StartLoading();            

            _QueryDef.Filter = args.GetQueryFilter<TItem>();
            _QueryDef.Sort = args.GetQuerySort();
            _QueryDef.FullText = FullTextSearch;

            _QueryDef.Page = 0;
            if (_virtualized == false)
            {
                _QueryDef.Page = args.GetQueryPage();
            }
            else
            {
                _QueryDef.Skip = args.Skip ?? 0;
            }
            _QueryDef.PageSize = args.GetQueryPageSize();
            try
            {
                var result = await _queryService.GetListAsync<TItem>(
                            _QueryDef, this.UsePost(),  _postPayload);
                
                if (result != null)
                {
                    Data = result.Items;
                    RowCount = result.TotalRows;
                }
            }
            finally
            {
                EndLoading();
            }
        }
    }
}
