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

    public AutoLoadDataList(
        IQueryUtilityService queryService)
    {
        _QueryDef = new QueryDefinition();
        _queryService = queryService;
    }

    public void Initilize(
        string endpoint
        )
    {
        _QueryDef.EndPoint = endpoint;
        _hasBeenInitialized = true;
    }

    public string FullTextSearch { get; set; } = string.Empty;
    public bool isLoading { get; private set; }
    public int RowCount { get; private set; }
    public IList<TItem>? Data { get; private set; }

    public async Task LoadData(
        LoadDataArgs args)
    {
        if (_hasBeenInitialized == false) return;

        isLoading = true;

        _QueryDef.Filter = args.GetQueryFilter<TItem>();
        _QueryDef.Sort = args.GetQuerySort();
        _QueryDef.FullText = FullTextSearch;

        _QueryDef.Page = args.GetQueryPage();
        _QueryDef.PageSize = args.GetQueryPageSize();
        try
        {
            var result = await _queryService.GetListAsync<TItem>(
                    _QueryDef);

            if (result != null)
            {
                Data = result.Items;
                RowCount = result.TotalRows;
            }
        }
        finally
        {
            isLoading = false;
        }
    }
}
}
