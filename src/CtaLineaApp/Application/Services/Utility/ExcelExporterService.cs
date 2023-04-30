using CtaLineaApp.Application.Model.Utility;
using CtaLineaApp.Application.Services.Account;
using Microsoft.JSInterop;
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
		private const string Time_Format = "\"{0:hh:mm}\"";
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
            await this.ExportExcel(title, items, columns, sheetTitle);
        }
        public async Task ExportExcel<TEntity>(
            string title,
            IEnumerable<TEntity> items,
            IEnumerable<ExcelExporterColumnInfo<TEntity>>? columns = null,
            string? sheetTitle = null
            )
            where TEntity : class
        {
			// TODO: implementare la esprotazione in excel vero


            await this.ExportCsv(title, items, columns, sheetTitle);
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
	}
}
