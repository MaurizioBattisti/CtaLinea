namespace ZzSoft.QueryHelper
{
    public interface IQueryDefinition<T>
    {
        object Arguments { get; }
        int RequestedPage { get; }

        string GetCountQuery();
        string GetSelectQuery();
    }
}