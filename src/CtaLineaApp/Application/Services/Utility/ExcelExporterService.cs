using CtaLineaApp.Application.Model.Utility;
using CtaLineaApp.Application.Services.Account;
using Microsoft.JSInterop;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Radzen;
using Radzen.Blazor.Rendering;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace CtaLineaApp.Application.Services.Utility
{
	public class ExcelExporterService 
        : IExcelExporterService
    {
        private const string Field_Separator = ";";
        private const string Text_Format = "\"{0}\"";
        private const string Date_Format = "\"{0:dd/MM/yyyy}\"";
		private const string Time_Format = "\"{0:HH:mm}\"";
		private const string Number_Format = "{0}";
		private const string DecimalNumber_Format = "{0:0.0000}";
		private const string FloatNumber_Format = "{0:0.00}";
		private const string Boolean_Format = "{0}";

		private readonly IAccountService _userService;
        private readonly IJSRuntime _js;
        private readonly IApplicationSettings _appSettings;

		public ExcelExporterService(
            IJSRuntime js,
            IAccountService userService,
			IApplicationSettings appSettings
			)
        {
            _js = js;
            _userService = userService;
			_appSettings = appSettings;
		}

        public async Task Export<TEntity>(
            string title,
            IEnumerable<TEntity> items,
            IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
            string? sheetTitle = null
            )
            where TEntity : class
        {
			// per default esegue l'esportazione in excel
            await this.ExportCsv(title, items, columns, sheetTitle);
        }
        public async Task ExportExcel<TEntity>(
            string title,
            IEnumerable<TEntity> items,
            IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
            string? sheetTitle = null
            )
            where TEntity : class
        {
            if (sheetTitle == null) sheetTitle = title;

            var colList = (columns ??
                        this.GetDefaultColumns<TEntity>()).ToList();

            // implementare la esprotazione in excel vero
            using var stream = new MemoryStream();
            var wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet(sheetTitle);

            await this.WriteAllrowToSheet(sheet, colList, items);

            wb.Write(stream);
            var  buff = stream.GetBuffer();

            using var stream2 = new MemoryStream(buff);
            stream2.Position = 0;
            var fileName = title + ".xlsx";

            using var streamRef = new DotNetStreamReference(stream: stream2);
            await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
			return;
        }

        public async Task ExportCsv<TEntity>(
			string title,
			IEnumerable<TEntity> items,
			IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
			string? sheetTitle = null
			)
			where TEntity : class
		{
            Encoding encoding = Encoding.UTF8;
            using var stream = new MemoryStream();
            TextWriter wrt = new StreamWriter(stream, encoding);

			var colList = (columns ??
						this.GetDefaultColumns<TEntity>()).ToList();

            // scrive le intestazioni delle colonne
            await this.WriteHEaderAsync(wrt, colList);

            await this.WriteAllRowsAsunc(wrt, colList, items);
            stream.Position = 0;

			var fileName = title + ".csv";

			using var streamRef = new DotNetStreamReference(stream: stream);
			await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);

			return;
		}

		#region funzioni interne
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

        private string? CleanText (string? text)
        {
            if (text == null) return null;
            return text.Replace("\t", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\b", " ")
				.Replace("\"", "'")
				;
		}

		private async Task WriteHEaderAsync<TEntity>(
			TextWriter wrt,
			IList<ExcelExporterColumnInfo<TEntity>> columns
			)
			where TEntity : class
		{
            var headersLine = new List<string>();

			foreach (var column in columns)
			{
				string header = column.ColumnName;
				if (string.IsNullOrWhiteSpace(column.Header) == false) header = column.Header;
                
                var headerText = string.Format(Text_Format, 
                    this.CleanText(header));
                headersLine.Add(headerText);
			}
            await wrt.WriteLineAsync(
                string.Join(Field_Separator, headersLine));
		}
        private async Task WriteAllRowsAsunc<TEntity>(
			TextWriter wrt,
			IList<ExcelExporterColumnInfo<TEntity>> columns,
            IEnumerable<TEntity> items
			)
			where TEntity : class
        {
			Type t = typeof(TEntity);
			var props = t.GetProperties();

			foreach (TEntity item in items)
            {
				var headersLine = new List<string>();

				foreach (var column in columns)
				{
					var prop = props.Where(p => p.Name == column.ColumnName).SingleOrDefault();
					if (prop != null)
					{
						object? value = prop.GetValue(item);

                        var txtValue = FormatValue(value, prop);
                        headersLine.Add(txtValue ?? string.Empty);
					}
				}
				await wrt.WriteLineAsync(
					string.Join(Field_Separator, headersLine));
			}
			await wrt.FlushAsync();
        }
        private string? FormatValue (
            object? value,
            PropertyInfo  prop)
        {
            if (value == null) return null;
            string txt = string.Empty;

			var t = value.GetType();
			// testo
			if (t == typeof(string))
			{
				// assegna un  testo pulito
				txt = (string)value;
				txt = this.CleanText(txt) ?? string.Empty;
				txt = string.Format(_appSettings.CurrentCulture, Text_Format, txt);
			}
			// date
			else if (t == typeof(DateTime))
			{
				txt = string.Format(_appSettings.CurrentCulture, Date_Format, value);
			}
			// ore
			else if (t == typeof(TimeSpan))
			{
				var ts = (TimeSpan)value;
				var to = new TimeOnly(ts.Hours, ts.Minutes, ts.Seconds); ;
				txt = string.Format(_appSettings.CurrentCulture, Time_Format, to);
			}
			// numerici
			else if (t == typeof(int)
				|| t == typeof(long)
				)
			{
				txt = string.Format(_appSettings.CurrentCulture, Number_Format, value);
			}
			else if (t == typeof(float)
				|| t == typeof(double)
				)
			{
				txt = string.Format(_appSettings.CurrentCulture, FloatNumber_Format, value);
			}
			else if (t == typeof(decimal))
			{
				txt = string.Format(_appSettings.CurrentCulture, DecimalNumber_Format, value);
			}
			else if (t == typeof(bool))
			{
				txt = string.Format(_appSettings.CurrentCulture, Boolean_Format, value);
			}
			else
			{
				txt = string.Format(_appSettings.CurrentCulture, Text_Format, value.ToString());
			}

			return txt;
        }
        #endregion

        #region export to Excel
        private async Task WriteHeaderToSheetAsync<TEntity>(
            ISheet sheet,
			int row, 
            IList<ExcelExporterColumnInfo<TEntity>> columns
            )
            where TEntity : class
        {
            var headersLine = new List<string>();
            foreach (var column in columns)
            {
                string header = column.ColumnName;
                if (string.IsNullOrWhiteSpace(column.Header) == false) header = column.Header;

                var headerText = string.Format(Text_Format,
                    this.CleanText(header));
                headersLine.Add(headerText);
            }

            var rowStyle = sheet.Workbook.CreateCellStyle();
            var font =sheet.Workbook.CreateFont();
            font.IsBold = true;
            rowStyle.SetFont(font);

            // TODO: trovare un modo per far funzionare il colore
            /*
            rowStyle.FillBackgroundColor = IndexedColors;
            rowStyle.FillPattern = FillPattern.SolidForeground;
            */

            await WriteRowToSheetAsync(sheet, row, headersLine, rowStyle);
        }
        private async Task WriteAllrowToSheet<TEntity>(
            ISheet sheet,
            IList<ExcelExporterColumnInfo<TEntity>> columns,
            IEnumerable<TEntity> items
            )
            where TEntity : class
        {
			int row = 0;

			// scrive la riga di intestazione
			await this.WriteHeaderToSheetAsync (sheet, row, columns);
			++row;
            
            Type t = typeof(TEntity);
            var props = t.GetProperties();

            var rowStyle_odd = sheet.Workbook.CreateCellStyle();
            var rowStyle_oevn= sheet.Workbook.CreateCellStyle();
            // TODO: quanto il voore funziona fare l'alternato
            /*
            rowStyle_oevn.FillBackgroundColor = 22;
            */

            // cicla su tutte le righe e esporta tutto
            foreach (var item in items)
            {
                var headersLine = new List<object?>();

                foreach (var column in columns)
                {
                    var prop = props.Where(p => p.Name == column.ColumnName).SingleOrDefault();
                    if (prop != null)
                    {
                        object? value = prop.GetValue(item);
                        headersLine.Add(value);
                    }
                }
                var rowStype = rowStyle_oevn;
                if (row % 2 > 0) rowStype = rowStyle_odd;

                await WriteRowToSheetAsync(sheet, row, headersLine, rowStype);
                ++row;
            }
        }
		private async Task WriteRowToSheetAsync (
            ISheet sheet,
			int row,
			IEnumerable <object?> data,
            ICellStyle rowStyle
            )
		{
            var sheetRow = sheet.CreateRow(row);
            int col = 0;
			foreach (object? value in data)
			{
				var cell = sheetRow.CreateCell(col);
                this.SetCEllValue (cell, value, rowStyle);
                ++col;
			}

			await Task.CompletedTask;
		}
		private void SetCEllValue (
            ICell cell, 
            object? value,
            ICellStyle rowStyle)
		{
			if (value == null) return;

            var t = value.GetType();
            cell.CellStyle = rowStyle;
            // testo
            if (t == typeof(string))
            {
                cell.SetCellValue( (string)value) ;
            }
            // date
            else if (t == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)value);
                cell.CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat(Date_Format);
                
                var createHelper = cell.Sheet.Workbook.GetCreationHelper();
                short format = createHelper.CreateDataFormat().GetFormat("d/m/yyyy");

                var cellStyle = cell.Sheet.Workbook.CreateCellStyle();
                cellStyle.CloneStyleFrom(rowStyle);
                cellStyle.DataFormat = format;
                cell.CellStyle = cellStyle;
            }
            // ore
            else if (t == typeof(TimeSpan))
            {
                var ts = (TimeSpan)value;
                DateTime dt = new DateTime(2000, 1, 1, ts.Hours, ts.Minutes, ts.Seconds); ;
                cell.SetCellValue(dt);

                var createHelper = cell.Sheet.Workbook.GetCreationHelper();
                short format = createHelper.CreateDataFormat().GetFormat("hh:mm");
                
                var cellStyle = cell.Sheet.Workbook.CreateCellStyle();
                cellStyle.CloneStyleFrom(rowStyle);
                cellStyle.DataFormat = format;
                cell.CellStyle = cellStyle;
            }
            // numerici
            else if (t == typeof(int))
            {
                float num = (int)value;
                cell.SetCellValue((double)num);
            }
            else if (t == typeof(long))
            {
                float num = (long)value;
                cell.SetCellValue((double)num);
            }
            else if (t == typeof(float))
            {
                float num = (float)value;
                cell.SetCellValue((double)num);
            }
            else if (t == typeof(double))
            {
                cell.SetCellValue((double) value );
            }
            else if (t == typeof(decimal))
            {
                var num = (decimal)value;
                cell.SetCellValue((double)num);
            }
            else if (t == typeof(bool))
            {
                cell.SetCellValue((bool) value);
            }
            else
            {
                var txt = string.Format(_appSettings.CurrentCulture, Text_Format, value.ToString());
                cell.SetCellValue(txt);
            }
        }

        #endregion
    }
}
