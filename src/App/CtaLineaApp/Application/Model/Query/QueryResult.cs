namespace CtaLinea.Application.Model.Query
{
    public class QueryResult<T>
    {
        public T[] Items { get; private set; }
        public int TotalRows { get; private set; }
        public int CurrentPage { get; private set; }

        public QueryResult(
           T[] items,
           int totlaRows,
           int currentPage)
        {
            this.Items = items;
            this.TotalRows = totlaRows;
            this.CurrentPage = currentPage;
        }
    }
}
