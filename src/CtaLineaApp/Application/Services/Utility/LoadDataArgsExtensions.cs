using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Services.Utility
{
    public static class LoadDataArgsExtensions
    {
        public static string GetQueryFilter<T>(
            this LoadDataArgs args,
            string? existingFilter = null)
            where T : class
        {
            var filter = new StringBuilder(1024);
            int filterCount = 0;
            if (args.Filters != null)
            {
                foreach (var f in args.Filters)
                {
                    string? singleFilter = null;
                    var type = typeof(T);
                    var prop = type.GetProperty(f.Property);
                    if (prop != null)
                    {
                        string filterOp = "$eq(";
                        string? filterVal = f.FilterValue.ToString();
                        if (filterVal != null)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                filterOp = "$like(";
                                filterVal = "'" + filterVal + "'";
                            }
                            else if (prop.PropertyType == typeof(DateTime)
                                || prop.PropertyType == typeof(Nullable<DateTime>)
                                || prop.PropertyType == typeof(DateOnly)
                                || prop.PropertyType == typeof(Nullable<DateOnly>)
                                )
                            {
                                filterVal = string.Format("{0:yyyy-MM-dd}", f.FilterValue);
                            }
                            else if ((
                                    prop.PropertyType == typeof(TimeSpan)
                                    || prop.PropertyType == typeof(Nullable<TimeSpan>)
                                ) && (
                                    f.FilterValue.GetType() == typeof(DateTime)
                                    || f.FilterValue.GetType() == typeof(Nullable<DateTime>)
                                ))
                            {
                                filterVal = string.Format("{0:HH:mm:ss}", f.FilterValue);
                            }
                            singleFilter = filterOp + f.Property + "," + filterVal + ")";
                            ++filterCount;
                        }
                    }

                    if (string.IsNullOrEmpty(singleFilter) == false)
                    {
                        if (filter.Length > 0) filter.Append(",");
                        filter.Append(singleFilter);
                    }
                }
            }
            var myFilter = filter.ToString();
            if (filterCount > 1)
            {
                myFilter = "$and(" + myFilter + ")";
            }

            return myFilter;
        }
        public static string? GetQuerySort(
            this LoadDataArgs args,
            string? defaultSort = null)
        {
            var sortSb = new StringBuilder(1024);

            // crea l'ordinamento
            if (args.Sorts != null)
            {
                foreach (var s in args.Sorts)
                {
                    if (sortSb.Length > 0) sortSb.Append(",");
                    sortSb.Append(s.Property);
                    if (s.SortOrder == SortOrder.Descending)
                    {
                        sortSb.Append(@"-");
                    }
                }
            }

            var sort = sortSb.ToString();
            if (string.IsNullOrWhiteSpace (sort) == true)
            {
                sort = defaultSort;
            }
            return sort;
        }
        public static int GetQueryPage(
            this LoadDataArgs args)
        {
            return (args.Skip ?? 0) / (args.Top ?? 1);
        }
        public static int GetQueryPageSize(
            this LoadDataArgs args)
        {
            return args.Top ?? -1;
        }
    }
}
