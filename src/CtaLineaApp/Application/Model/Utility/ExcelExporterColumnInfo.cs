using ClosedXML.Excel;

namespace CtaLineaApp.Application.Model.Utility
{
    public class ExcelExporterColumnInfo<TEntity>
        where TEntity : class
    {
        public string ColumnName { get; set; } = string.Empty;
        public string? Header { get; set; }
    }
}
