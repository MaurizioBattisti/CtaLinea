namespace ZzSoft.QueryHelper
{
    public interface IFilteringContext
    {
        string FullText { get; }

        string Filter { get; }
        string Sort { get; }
        string Projection { get; }

        int Page { get; }
        int PageSize { get; }
        int Skip { get; }

        bool Count { get; }
    }
}