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

        void Initilize(
            string endpoint,
            bool usePost = false,
            object? postPayload = null
            );
        Task LoadData(LoadDataArgs args);
    }
}