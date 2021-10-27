using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLinea.Application.Utility
{
    public static class LoadDataArgsExtensions
    {
        public static string GetQueryFilter<T>(
            this LoadDataArgs args,
            string existingFilter = null)
            where T : class
        {
            var filter = string.Empty;
            foreach (var f in args.Filters)
            {
                var type = typeof(T);
                var prop = type.GetProperty(f.Property);
                if (prop != null)
                {
                    string filterOp = "$eq(";
                    string filterVal = f.FilterValue.ToString();
                    if (prop.PropertyType == typeof(string))
                    {
                        filterOp = "$like(";
                        filterVal = "\"" + filterVal + "\"";
                    }
                    var singleFilter = filterOp + "," + filterVal + ")";

                    filter += filter;
                }
            }

            return filter;
        }
        public static string GetQuerySort(
            this LoadDataArgs args,
            string defaultSort = null)
        {
            string sort = string.Empty;




            if (string.IsNullOrWhiteSpace (sort) == true)
            {
                sort = defaultSort;
            }
            return sort;
        }
        public static int GetQueryPage(
            this LoadDataArgs args)
        {
            return args.Skip.Value / args.Top.Value;
        }
        public static int GetQueryPageSize(
            this LoadDataArgs args)
        {
            return args.Top.Value;
        }
    }
}
