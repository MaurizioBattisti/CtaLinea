using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZzSoft.QueryHelper
{
    public class FilteringContext 
        : IFilteringContext
    {
        public FilteringContext(
            string fullText,
            string filter,
            string sort,
            string projection,
            int Page,
            int pageSize,
            bool count)
        {
            this.FullText = fullText;

            this.Filter = filter;
            this.Sort = sort;
            this.Projection = projection;

            this.Page = Page;
            this.PageSize = pageSize;

            this.Count = count;
        }

        public string FullText { get; private set; }

        public string Filter { get; private set; }
        public string Sort { get; private set; }
        public string Projection { get; private set; }

        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public bool Count { get; private set; }
    }
}
