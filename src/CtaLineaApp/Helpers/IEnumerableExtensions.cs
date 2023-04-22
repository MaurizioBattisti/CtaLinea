using CtaLinea.Model.Attributes;
using CtaLineaApp.Application.Model.Utility;
using Radzen.Blazor;

namespace CtaLineaApp.Helpers
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<ExcelExporterColumnInfo<TEntity>> GetExportColumnInfo<TEntity>(
			this IEnumerable<TEntity> ites)
			where TEntity : class
		{
			var type = typeof(TEntity);
			foreach (var prop in type.GetProperties())
			{
				var header = prop.Name;

				// recuper l'attributo 
				var attr = prop.GetCustomAttributes(true).Where(a => a.GetType() == typeof(ColumnDescriptionAttribute)).Select (a => a as ColumnDescriptionAttribute) .FirstOrDefault();
				if (attr != null) 
				{
					if (attr.Ignore == true) continue;
                    header = attr.Header;
				}

				yield return new ExcelExporterColumnInfo<TEntity>()
				{
					ColumnName = prop.Name,
					Header = header
				};
            }
		}
	}
}
