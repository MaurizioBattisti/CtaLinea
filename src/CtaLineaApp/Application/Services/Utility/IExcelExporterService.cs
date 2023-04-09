using CtaLineaApp.Application.Model.Utility;
using Microsoft.JSInterop;

namespace CtaLineaApp.Application.Services.Utility
{
    public interface IExcelExporterService
    {
        Task ExportCsv<TEntity>(
            string title,
            IEnumerable<TEntity> items,
            IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
            string? sheetTitle = null
            )
            where TEntity : class;
    }
}