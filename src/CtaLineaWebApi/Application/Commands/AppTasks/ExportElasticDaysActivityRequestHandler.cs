using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Repositories;
using CtaLineaWebApi.Application.Services;
using ZzSoft.CtaLinea.Dal.Model.Import;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Record.Chart;
using CtaLinea.Model.Attributes;
using System.Linq;
using System.IO;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ExportElasticDaysActivityRequestHandler
        : IRequestHandler<ExportElasticDaysActivityRequest, bool>
    {
        private class ElastibsuExportRow
        {
            public static CultureInfo s_Culture = new CultureInfo("it-IT");

            [ColumnDescription(Header = "G. Sett")]
            public string WeekDay
            {
                get
                {
                    return string.Format(s_Culture, "{0:dddd}", this.Date);
                }
            }
            [ColumnDescription(Header = "Giorno")]
            public DateTime Date { get; set; }
            [ColumnDescription(Header = "Km")]
            public float Km { get; set; }
            [ColumnDescription(Header = "Nr. Persone")]
            public int PeopleCount { get;set; }
        };

        private readonly ILogger _logger;
        private readonly ISchedulerTaskLogger _schedulerLogger;
        private readonly ImporterConfiguration _options;
        private readonly CultureInfo _dateCulture;
        private readonly CtaDbContext _context;
        private readonly IExportService _exportService;
        private readonly IUtilityREpository _repo;

        public ExportElasticDaysActivityRequestHandler(
            ImporterConfiguration options,
            IExportService exportService,
            ISchedulerTaskLogger schedulerLogger,
            CtaDbContext context,
            IUtilityREpository repo,
            ILogger<ExportElasticDaysActivityRequestHandler> logger
            )
        {
            _context = context;
            _dateCulture = new CultureInfo("it-IT");
            _options = options;
            _schedulerLogger = schedulerLogger;
            _exportService = exportService;
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool> Handle(
            ExportElasticDaysActivityRequest request,
            CancellationToken cancellationToken)
        {
            // calcal il nome dle file da usare
            string fileName = this._options.ElastibusExportFilePath;
            if (fileName.EndsWith (Path.DirectorySeparatorChar) == false)
            {
                fileName += Path.DirectorySeparatorChar;
            }
            if (string.IsNullOrWhiteSpace(request.FileName) == true)
            {
                fileName += "Giorni_Elastibus";
            }
            else fileName += request.FileName.Trim();
			fileName += ".xlsx";

			try
            {
                await _schedulerLogger.LogAsync("inizio esportaizone dati elastibus");

                // estrae i dati dell'elastibus
                var elasticDays = await this._repo.GetElastibusDayForExportAsyunc(
                    request.StartDate,
                    request.EndDate,
                    request.ContractId)
                    .ConfigureAwait(false);

                await _schedulerLogger.LogAsync("dati caricati dal DB");

                // recupera le intestazioni degli sheet
                var sheets = (from d in elasticDays
                              select new { d.RunId, d.Description } )
                              .Distinct();

                var data = new List<Tuple<string, IEnumerable<ElastibsuExportRow>>>();
                foreach (var h in sheets)
                {
                    var items = (from d in elasticDays
                                 where d.RunId == h.RunId
                                    && d.Description == h.Description
                                 orderby d.Day ascending
                                 select new ElastibsuExportRow()
                                 {
                                     Date = d.Day,
                                     Km = d.Km ?? 0,
                                     PeopleCount = d.PeopleCount ?? 0
                                 });
                    var t = new Tuple<string, IEnumerable<ElastibsuExportRow>>(h.Description, items);
                    data.Add(t);
                }

                await this._exportService.ExportToExcelASync(
					fileName,
                    data)
                    .ConfigureAwait(false);

                await _schedulerLogger.LogAsync("fine esportaizone dati elastibus");
            }
            catch (Exception ex)
            {
                await _schedulerLogger.LogAsync(ex.Message, "ERROR");
                return false;
            }

            return true;
        }
    }
}
