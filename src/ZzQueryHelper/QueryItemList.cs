using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZzSoft.QueryHelper
{
    public class QueryItemList<T>
        where T: class
    {
        public QueryItemList (
            IEnumerable<T> items,
            int currentPage = -1,
            int totalRows = -1)
        {
            this.Items = items;
            this.CurrentPage = currentPage;
            this.TotalRows = totalRows;
        }

        public IEnumerable<T> Items { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalRows { get; private set; }
    }
}
