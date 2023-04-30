using Radzen;

namespace CtaLineaApp.Application.Services.Utility
{
    public interface IAutoLoadDataList<TItem> where TItem : class
    {
        IList<TItem>? Data { get; }
        string FullTextSearch { get; set; }
        bool isLoading { get; }
        int RowCount { get; }

        void StartLoading();
        void EndLoading();

        Task<IEnumerable<TItem>?> GetAllAsync();

        void Initilize(
            string endpoint,
            bool usePost = false,
            object? postPayload = null,
            bool virtualized = false
            );
        Task LoadData(LoadDataArgs args);
    }
}