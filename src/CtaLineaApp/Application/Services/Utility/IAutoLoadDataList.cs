using Radzen;

namespace CtaLineaApp.Application.Services.Utility
{
    public interface IAutoLoadDataList<TItem> where TItem : class
    {
        IList<TItem>? Data { get; }
        string FullTextSearch { get; set; }
        bool isLoading { get; }
        int RowCount { get; }

        void Initilize(string endpoint);
        Task LoadData(LoadDataArgs args);
    }
}