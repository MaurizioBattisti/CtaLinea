using CtaLinea.Model.Attributes;
using CtaLineaWebApi.Application.Model;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
	public class ExportService 
        : IExportService
    {
        private const string Export_Culture = "it-IT";
        private const string Text_Format = "{0}";
        private const string Date_Format = "{0:dd/MM/yyyy}";
        private const string Time_Format = "{0:HH:mm}";

        private readonly ILogger _logger;
        private readonly CultureInfo _currentCulture;

        public ExportService(
            ILogger<ExportService> llogger)
        {
            _currentCulture = new CultureInfo(Export_Culture);
            _logger = llogger;
        }

        public async Task ExportToExcelASync<TEntity>(
            string filename,
            IEnumerable<Tuple <string, IEnumerable<TEntity>>> multiSheet
            )
            where TEntity : class
        {
			// implementare la esprotazione in excel vero
			var wb = new XSSFWorkbook();
			using var stream = new FileStream(filename, FileMode.Create);

			try
			{
				// recupera le instazioni di colonna dale decorazioni
				var colList = this.GetExportColumnInfo<TEntity>().ToList();

				int index = 0;
                if (multiSheet.Count() > 0)
                {
                    foreach (var sheetData in multiSheet)
                    {
                        ++index;
                        var sheet = wb.CreateSheet(sheetData.Item1 ?? "Sheet " + index);

                        await this.WriteAllrowToSheet(sheet, colList, sheetData.Item2);
                    }
                }
                else
                {
					var sheet = wb.CreateSheet("No Data");
                    await this.WriteHeaderToSheetAsync (
                        sheet , 0, colList)
                        .ConfigureAwait (false);
				}

				wb.Write(stream);
			}
            catch (Exception ex)
            {
                string ss = ex.Message;
                throw;
            }
            finally
            {
                wb.Close();
            }

            await Task.CompletedTask
                .ConfigureAwait(false);
        }

        public async Task ExportToExcelASync<TEntity>(
            string filename,
            string sheetTitle,
            IEnumerable<TEntity> items
            )
            where TEntity : class
        {
            var data = new[]
            {
                new Tuple<string, IEnumerable<TEntity>> (sheetTitle, items)
            };

            await this.ExportToExcelASync<TEntity>(
                filename,
                data
                )
                .ConfigureAwait(false);
        }

        #region supportin funcitions
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
        private string? CleanText(string? text)
        {
            if (text == null) return null;
            return text.Replace("\t", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\b", " ")
                .Replace("\"", "'")
                ;
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
            var font = sheet.Workbook.CreateFont();
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
            await this.WriteHeaderToSheetAsync(sheet, row, columns);
            ++row;

            Type t = typeof(TEntity);
            var props = t.GetProperties();

            var rowStyle_odd = sheet.Workbook.CreateCellStyle();
            var rowStyle_oevn = sheet.Workbook.CreateCellStyle();
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
        private async Task WriteRowToSheetAsync(
            ISheet sheet,
            int row,
            IEnumerable<object?> data,
            ICellStyle rowStyle
            )
        {
            var sheetRow = sheet.CreateRow(row);
            int col = 0;
            foreach (object? value in data)
            {
                var cell = sheetRow.CreateCell(col);
                this.SetCEllValue(cell, value, rowStyle);
                ++col;
            }

            await Task.CompletedTask;
        }
        private void SetCEllValue(
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
                cell.SetCellValue((string)value);
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
                cell.SetCellValue((double)value);
            }
            else if (t == typeof(decimal))
            {
                var num = (decimal)value;
                cell.SetCellValue((double)num);
            }
            else if (t == typeof(bool))
            {
                cell.SetCellValue((bool)value);
            }
            else
            {
                var txt = string.Format(_currentCulture, Text_Format, value.ToString());
                cell.SetCellValue(txt);
            }
        }

        #endregion

        private IEnumerable<ExcelExporterColumnInfo<TEntity>> GetExportColumnInfo<TEntity>()
			where TEntity : class
		{
			var type = typeof(TEntity);
			foreach (var prop in type.GetProperties())
			{
				var header = prop.Name;

				// recuper l'attributo 
				var attr = prop.GetCustomAttributes(true).Where(a => a.GetType() == typeof(ColumnDescriptionAttribute)).Select(a => a as ColumnDescriptionAttribute).FirstOrDefault();
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
