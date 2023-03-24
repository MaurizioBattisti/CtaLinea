using ClosedXML.Excel;
using CtaLineaApp.Application.Model.Utility;
using CtaLineaApp.Application.Services.Account;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using System.Net.Security;
using System.Text;

namespace CtaLineaApp.Application.Services.Utility
{
    public class ExcelExporterService 
        : IExcelExporterService
    {
        private readonly IAccountService _userService;
        private readonly IJSRuntime _js;

        public ExcelExporterService(
            IJSRuntime js,
            IAccountService userService
            )
        {
            _js = js;
            _userService = userService;
        }

        public async Task ExcelExport<TEntity>(
            string title,
            IEnumerable<TEntity> items,
            IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
            string? sheetTitle = null
            )
            where TEntity : class
        {
            var wb = new XLWorkbook();

            wb.Properties.Author = _userService.User?.Description ?? string.Empty;
            wb.Properties.Title = title;
            wb.Properties.Subject = DateTime.Today.ToString();

            var ws = wb.Worksheets.Add(sheetTitle ?? "Dati");

            var colList = (columns ??
                        this.GetDefaultColumns<TEntity>()).ToList();

            // crea le intestazioni di colonna
            this.WriteHEader(ws, colList, 1);

            // scrive tutte le righe a partire dalla numero 1
            this.WriteAllRows(ws, items, colList, 2);

            // salva ilf ile in uno stream
            var XLSStream = new MemoryStream();
            wb.SaveAs(XLSStream);

            var fileName = title + ".xlsx";
            
            await _js.InvokeVoidAsync("BlazorDownloadFile", fileName, "application/octet-stream", XLSStream.GetBuffer());

            // await _js.InvokeAsync<object>("saveFile", fileName, XLSStream);
            // await _js.InvokeVoidAsync("saveFile", fileName, XLSStream);
            // using var streamRef = new DotNetStreamReference(stream: XLSStream);
            // await _js.InvokeVoidAsync("BlazorDownloadFile", fileName, streamRef);
        }

        private void WriteHEader<TEntity>(
            IXLWorksheet ws,
            IList<ExcelExporterColumnInfo<TEntity>> columns,
            int row = 0
            )
            where TEntity : class
        {
            var col = 1;

            foreach (var column in columns)
            {
                string header = column.ColumnName;
                if (string.IsNullOrWhiteSpace(column.Header) == false) header = column.Header;
                ws.Cell(row, col).Value = header;
                ++col;
            }
        }

        private void WriteAllRows<TEntity>(
            IXLWorksheet ws,
            IEnumerable<TEntity> items,
            IList<ExcelExporterColumnInfo<TEntity>> columns,
            int startRow = 1
            )
            where TEntity : class
        {
            int row = startRow;
            foreach (var item in items)
            {
                this.WriteOneRow(ws, item, columns, row);
                ++row;
            }
        }

        private void WriteOneRow<TEntity>(
            IXLWorksheet ws,
            TEntity item,
            IList<ExcelExporterColumnInfo<TEntity>> columns,
            int row
            )
            where TEntity : class
        {
            Type t = typeof(TEntity);
            var props = t.GetProperties();

            int colIndex = 1;
            foreach (var column in columns)
            {
                var prop = props.Where(p => p.Name == column.ColumnName).SingleOrDefault();
                if (prop != null)
                {
                    object? value = prop.GetValue(item);
                    this.WriteOneCell(ws, item, value, column, row, colIndex);
                }
                ++colIndex;
            }
        }
        private void WriteOneCell<TEntity>(
            IXLWorksheet ws,
            TEntity item,
            object? value,
            ExcelExporterColumnInfo<TEntity> column,
            int rowIndex,
            int colIndex
            )
            where TEntity : class
        {
            if (value != null)
            {
                SetAnDromatCell(ws.Cell(rowIndex, colIndex), value);
            }
        }

        private IEnumerable<ExcelExporterColumnInfo<TEntity>> GetDefaultColumns<TEntity>()
            where TEntity : class
        {
            var t = typeof(TEntity);
            var props = t.GetProperties();
            foreach (var prop in props)
            {
                yield return new ExcelExporterColumnInfo<TEntity>()
                {
                    ColumnName = prop.Name
                };
            }
        }

        private static void SetAnDromatCell(
            IXLCell cell,
            object value)
        {
            var cellVal = new XLCellValue();
            var t = value.GetType();
            // testo
            if (t == typeof(string))
            {
                cell.Value = (string)value;
            }
            // date
            else if (t == typeof(DateTime))
            {
                cell.Value = (DateTime)value;
            }
            // ore
            else if (t == typeof(TimeSpan))
            {
                var ts = (TimeSpan)value;
                var to = new TimeOnly(ts.Hours, ts.Minutes, ts.Seconds); ;
                cell.Value = string.Format("{0:HH:mm:ss}", to);
            }
            // numerici
            else if (t == typeof(int))
            {
                cell.Value = (int)value;
            }
            else if (t == typeof(long))
            {
                cell.Value = (long)value;
            }
            else if (t == typeof(float))
            {
                cell.Value = (float)value;
            }
            else if (t == typeof(double))
            {
                cell.Value = (double)value;
            }
            else if (t == typeof(decimal))
            {
                cell.Value = (decimal)value;
            }
            // bbooleani
            else if (t == typeof(bool))
            {
                cell.Value = (bool)value;
            }
        }
    }
}
