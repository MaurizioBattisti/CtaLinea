using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.DataSource;
using Telerik.SvgIcons;

namespace CtaLineaApp.Application.Helpers
{
    public static class DataSourceRequestExtensions
    {
        private static IDictionary<FilterOperator, string> _operatorMap = new Dictionary<FilterOperator, string>()
        {
            {  FilterOperator.Contains, "$like" },
            {  FilterOperator.DoesNotContain, "$notlike" },
            {  FilterOperator.StartsWith, "$start" },
            {  FilterOperator.EndsWith, "$like" },

            {  FilterOperator.IsEqualTo, "$eq" },
            {  FilterOperator.IsNotEqualTo, "$ne" },
            {  FilterOperator.IsGreaterThan, "$gt" },
            {  FilterOperator.IsGreaterThanOrEqualTo, "$ge" },
            {  FilterOperator.IsLessThan, "$lt" },
            {  FilterOperator.IsLessThanOrEqualTo, "$le" }
        };

        public static string GetQueryFilter<T>(
            this DataSourceRequest request,
            string? existingFilter = null)
            where T : class
        {
            var sbFilter = new StringBuilder(1024);
            if (request.Filters != null
                && request.Filters.Count > 1)
            {
                sbFilter.Append("$and(");
                // crea il filtro
                HandleFilterDescriptor(request.Filters, sbFilter);
                sbFilter.Append(")");
            }
            else if(request.Filters != null
                && request.Filters.Count == 1)
            {
                var single = request.Filters.Single();
                var fDesc = single as CompositeFilterDescriptor;
                if (fDesc != null)
                {
                    HandleCompositeFilterDescriptor(fDesc, sbFilter);
                }
                else 
                {
                    var tehFilter = single as FilterDescriptor;
                    if (tehFilter != null)
                    {
                        HandleFilterDescriptor(tehFilter, sbFilter);
                    }
                }
            }

            return sbFilter.ToString();
        }

        public static string? GetQuerySort(
            this DataSourceRequest request,
            string? defaultSort = null)
        {
            var sortSb = new StringBuilder(1024);

            // crea l'ordinamento
            if (request.Sorts != null
                && request.Sorts.Count > 0)
            {
                foreach (var s in request.Sorts)
                {
                    if (sortSb.Length > 0) sortSb.Append(@",");
                    sortSb.Append(s.Member);
                    if (s.SortDirection == ListSortDirection.Descending)
                    {
                        sortSb.Append(@"-");
                    }
                }
            }

            var sort = sortSb.ToString();
            if (string.IsNullOrWhiteSpace(sort) == true)
            {
                sort = defaultSort;
            }
            return sort;
        }
        public static int GetPageSize(
            this DataSourceRequest request,
            int? defaultSort = null)
        {
            int pageSize = 50;

            if (defaultSort != null)
            {
                pageSize = defaultSort.Value;
            }
            if (request.PageSize >= 1) pageSize = request.PageSize;
            return pageSize;
        }
        public static int GetQueryPage(
            this DataSourceRequest request)
        {
            return request.Page;
        }
        public static int GetQueryPageSize(
            this DataSourceRequest request)
        {
            return request.PageSize;
        }
        public static int GetQuerySkip (
            this DataSourceRequest request)
        {
            return request.Skip;
        }

        #region gestione dei filtri
        private static void HandleFilterDescriptor(
            IEnumerable<IFilterDescriptor> filters,
            StringBuilder sb
            )
        {
            var count = 0;
            foreach (var descr in filters)
            {
                if (count > 0) { sb.Append(@","); }

                var composite = descr as CompositeFilterDescriptor;
                if (composite != null)
                {
                    HandleCompositeFilterDescriptor(composite, sb);
                }
                var fDesc = descr as FilterDescriptor;
                if (fDesc != null)
                {
                    HandleFilterDescriptor(fDesc, sb);
                }

                ++count;
            }
        }

        private static void HandleFilterDescriptor(
            FilterDescriptor filter,
            StringBuilder sb
            )
        {
            string filterOp = "$eq";
            string? filterVal = null;

            // applica l'operatore
            if (_operatorMap.ContainsKey (filter.Operator))
            {
                filterOp = _operatorMap[filter.Operator];
            }

            if (filter.MemberType == null)
            {
                // se il tipo non è definito assume testo 
                filterVal = "'" + (string)filter.Value + "'";
            }
            // per il momento gestisce solo luguaglianza e il like
            else if (filter.MemberType == typeof(string))
            {
                filterOp = "$like";
                filterVal = "'" + (string)filter.Value + "'";
            }
            else if (filter.MemberType == typeof(DateTime)
                || filter.MemberType == typeof(Nullable<DateTime>)
                || filter.MemberType == typeof(DateOnly)
                || filter.MemberType == typeof(Nullable<DateOnly>)
                )
            {
                filterVal = string.Format("{0:yyyy-MM-dd}", filter.Value);
            }
            else if ((
                    filter.MemberType == typeof(TimeSpan)
                    || filter.MemberType == typeof(Nullable<TimeSpan>)
                ) && (
                    filter.Value.GetType() == typeof(DateTime)
                    || filter.Value.GetType() == typeof(Nullable<DateTime>)
                ))
            {
                filterVal = string.Format("{0:HH:mm:ss}", filter.Value);
            }
            else
            {
                filterVal = filter.Value.ToString();
            }

            if (filterVal != null)
            {
                sb.Append(filterOp);
                sb.Append(@"(");
                sb.Append(filter.Member);
                sb.Append(",");
                sb.Append(filterVal);
                sb.Append(@")");
            }
        }
        private static void HandleCompositeFilterDescriptor(
            CompositeFilterDescriptor filter,
            StringBuilder sb
            )
        {
            if (filter.FilterDescriptors.Count > 1)
            {
                if (filter.LogicalOperator == FilterCompositionLogicalOperator.And)
                {
                    sb.Append("$and(");
                }
                else if (filter.LogicalOperator == FilterCompositionLogicalOperator.Or)
                {
                    sb.Append("$or(");
                }

                HandleFilterDescriptor(filter.FilterDescriptors, sb);

                sb.Append(")");
            }
            else if (filter.FilterDescriptors.Count == 1)
            {
                var filterDescr = filter.FilterDescriptors.Single() as FilterDescriptor;
                if (filterDescr != null) 
                {
                    HandleFilterDescriptor(filterDescr, sb);
                }
            }
        }

        #endregion
    }
}
