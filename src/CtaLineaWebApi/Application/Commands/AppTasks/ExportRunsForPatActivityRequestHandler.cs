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
using ZzSoft.CtaLinea.Dal.Queries;
using ZzSoft.QueryHelper;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ExportRunsForPatActivityRequestHandler
		: IRequestHandler<ExportRunsForPatActivityRequest, bool>
    {
		private readonly ILogger _logger;
        private readonly ISchedulerTaskLogger _schedulerLogger;
        private readonly ImporterConfiguration _options;
        private readonly CultureInfo _dateCulture;
        private readonly CtaDbContext _context;
        private readonly IExportService _exportService;
        private readonly IZzRequestConstx _zzContext;
		private readonly IUtilityREpository _repo;

		public ExportRunsForPatActivityRequestHandler(
            ImporterConfiguration options,
            IExportService exportService,
            ISchedulerTaskLogger schedulerLogger,
            CtaDbContext context,
			IUtilityREpository repo,
            IZzRequestConstx zzContext,
			ILogger<ExportRunsForPatActivityRequestHandler> logger
            )
        {
            _context = context;
            _dateCulture = new CultureInfo("it-IT");
            _options = options;
            _schedulerLogger = schedulerLogger;
            _exportService = exportService;
            _repo = repo;
			_zzContext = zzContext;
			_logger = logger;
        }

        public async Task<bool> Handle(
			ExportRunsForPatActivityRequest request,
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
                fileName += "Corse_per_TT";
            }
            else fileName += request.FileName.Trim();
			fileName += ".xlsx";

			try
            {
                await _schedulerLogger.LogAsync("inizio esportaizone dati per TT / PAT");

                this._zzContext.Override(
                    request.ContractId,
                    request.StartDate,
                    request.EndDate);

				var data = await _repo.GetExportTtAsync (
					request.StartDate,
					request.EndDate,
					request.ContractId)
					.ConfigureAwait(false);

				// esporta la lista ridotta
				await this._exportService.ExportToExcelASync(
					fileName,
                    "Elenco Corse",
                    data)
                    .ConfigureAwait(false);

				await _schedulerLogger.LogAsync("fine esportaizone dati per TT / PAT");
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
