using CtaLineaApp.Application.Model.Utility;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Radzen.Blazor;

namespace CtaLineaApp.Helpers
{
    public static class RadzenDataGridExtensions
    {
        public static IEnumerable<ExcelExporterColumnInfo<TEntity>> GetExportColumnInfo<TEntity> (
            this RadzenDataGrid<TEntity> grid)
            where TEntity: class
        {
            foreach (var column in grid.ColumnsCollection) 
            {
                if (column.Visible == false) continue;
                var colInfo = new ExcelExporterColumnInfo<TEntity>()
                {
                    ColumnName = column.Property,
                    Header= column.Title
                };
                yield return colInfo;
            }
        }

    }
}
