namespace CtaLineaWebApi.Application.Model
{
    internal class ExcelExporterColumnInfo<TEntity>
        where TEntity : class
    {
        public string ColumnName { get; set; } = string.Empty;
        public string? Header { get; set; }
    }
}
