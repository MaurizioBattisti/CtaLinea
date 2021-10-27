using MediatR;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;
using ZzSoft.CtaLinea.Dal.Model;
using CtaLineaWebApi.Application.Utility;
using CtaLineaWebApi.Utility.BackGround;
using Microsoft.Extensions.DependencyInjection;

namespace CtaLineaWebApi.Application.Commands
{
    public class TtServiceImportRequestHandler
        : IRequestHandler<TtServiceImportRequest, bool>
    {
        private const int Delta_Seconds = 5;

        private readonly ILogger _logger;
        private readonly ITtServiceRepository _importRepo;

        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TtServiceImportRequestHandler (
            ITtServiceRepository importRepo,
            ILogger<TtServiceImportRequestHandler> logger,
            IBackgroundTaskQueue queue,
            IServiceScopeFactory serviceScopeFactory)
        {
            this._importRepo = importRepo;
            this._logger = logger;

            this._queue = queue;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> Handle(
            TtServiceImportRequest request, 
            CancellationToken cancellationToken)
        {
            /*
            this._queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    */
                    // controla se sia possibile o meno eseguire l'importazione
                    bool canProcede = await this._importRepo.CanImportAsync(Constants.ImportDescr_TT);

                    if (canProcede == true
                        && request.File.Length > 0)
                    {
                        await this.BackGroundPorcessing(request);
                    }
                    /*
                }
            });
            */
            return await Task.FromResult(true)
                .ConfigureAwait(false);
        }

        private async Task BackGroundPorcessing(
            TtServiceImportRequest request)
        {
            var context = await this._importRepo.StartImportAsync(
                Constants.ImportDescr_TT,
                request.User
                );

            try
            {
                string sFileExtension = Path.GetExtension(request.File.FileName).ToLower();
                ISheet sheet = null;

                using (var stream = request.File.OpenReadStream())
                {
                    if (sFileExtension == ".xls")
                    {
                        //This will read the Excel 97-2000 formats  
                        var hssfwb = new HSSFWorkbook(stream);
                        //get first sheet from workbook  
                        sheet = hssfwb.GetSheetAt(0);
                    }
                    else if (sFileExtension == ".xlsx")
                    {
                        //This will read 2007 Excel format  
                        var hssfwb = new XSSFWorkbook(stream);
                        //get first sheet from workbook   
                        sheet = hssfwb.GetSheetAt(0);
                    }
                }
                if (sheet == null)
                {
                    this._logger.LogWarning("Importazione del file TT non è stata avviata, il file {file} non è un file Excel valido", request.File.FileName);
                    
                    context.Note = string.Format("il file {0} non è un file Excel valido", request.File.FileName);
                    // indica che limportazione è stata abortita
                    await this._importRepo.ImportAbortedAsync(context);
                    return;
                }

                // fai il lavoor  sporco
                await this.ProcessRowsAsync(context, sheet);

                // completa il lavoro di importazione
                await this._importRepo.ImportCopleteAsync(context);

            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, "errpre nel caricametno del file dei servizi TT");
                if (context.Note == null) context.Note = string.Empty;
                context.Note += "\n" +  exc.Message;
                await this._importRepo.ImportFailedAsync(context);
                throw;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        private async Task ProcessRowsAsync(
            ImportContext context,
            ISheet sheet
            )
        {
            IRow headerRow = sheet.GetRow(0);

            var firstRow = sheet.FirstRowNum + 1;
            var lastRow = sheet.LastRowNum;

            // prende un punto di partenza
            var LastCheck = DateTime.Now;

            int serviceId = 0;
            TtService curretService = null;
            var nodes = new List<TtServiceNode>();

            try
            {
                foreach (var item in this.IterateTtService(sheet, firstRow, lastRow))
                {
                    // contolla se deve aggiornare los tato di progresso
                    if (DateTime.Now > LastCheck.AddSeconds(Delta_Seconds))
                    {
                        LastCheck = DateTime.Now;

                        // aggiorna lo stato di avanzamento del progresso
                        await this._importRepo.ReportProgressAsync(context);
                    }
                    if (serviceId != 0
                        && item.Item1.ServiceId != serviceId)
                    {
                        // salva i dati e riparte
                        await this._importRepo.ImportServiceASync(
                            context,
                            curretService,
                            nodes);

                        curretService = null;
                        nodes = new List<TtServiceNode>();
                    }

                    // salva il valroe del primo elemento
                    serviceId = item.Item1.ServiceId;

                    curretService = item.Item1;
                    nodes.Add(item.Item2);
                }
            }
            finally
            {
                if (curretService != null)
                {
                    // salva l'ultimo servizio
                    await this._importRepo.ImportServiceASync(
                        context,
                        curretService,
                        nodes);
                }

                LastCheck = DateTime.Now;

                // aggiorna lo stato di avanzamento del progresso
                await this._importRepo.ReportProgressAsync(context);
            }

            return;
        }

        private IEnumerable<Tuple<TtService, TtServiceNode >> IterateTtService(
            ISheet sheet,
            int firstRowIndex,
            int lastRowIndex)
        {
            for (int rowIndex = firstRowIndex; rowIndex <= lastRowIndex; ++rowIndex)
            {

                var dataRow = sheet.GetRow(rowIndex);
                var service = new TtService();
                var node = new TtServiceNode();

                // popola i dati dei due elementi
                service.ServiceId = dataRow.GetCell((int) TtImportColumn.idServizio).GetIntValue().Value;
                service.LineNumber = dataRow.GetCell((int)TtImportColumn.codiceLinea)?.GetStringValue();
                service.RunNumber = dataRow.GetCell((int) TtImportColumn.numeroCorsa)?.GetStringValue();

                service.LineDescription = dataRow.GetCell((int)TtImportColumn.descrizioneLinea)?.GetStringValue();
                service.PathDescription = dataRow.GetCell((int)TtImportColumn.percorso)?.GetStringValue();
                service.Km = dataRow.GetCell((int)TtImportColumn.chilometriCapitolato)?.GetDecimalValue();
                
                service.FrequencyId = dataRow.GetCell((int)TtImportColumn.codiceFrequenza)?.GetIntValue();
                service.FrequencyDescription = dataRow.GetCell((int)TtImportColumn.descrizioneFrequenza)?.GetStringValue();

                service.TarifLineId = dataRow.GetCell((int)TtImportColumn.codiceLineaTariffaria)?.GetIntValue();
                service.TurnOneId = dataRow.GetCell((int)TtImportColumn.codiceTurno1)?.GetIntValue();
                service.TurnTwoId = dataRow.GetCell((int)TtImportColumn.codiceTurno2)?.GetIntValue();

                service.InsertDate = dataRow.GetCell((int)TtImportColumn.dataIns)?.GetDateTimeValue();
                service.ModifyDate = dataRow.GetCell((int)TtImportColumn.dataMod)?.GetDateTimeValue();
                service.LastUpdateDate = dataRow.GetCell((int)TtImportColumn.dataUltimoAggiornamentoCorsa)?.GetDateTimeValue();

                service.ServiceTypeId = dataRow.GetCell((int)TtImportColumn.idTipoServizio)?.GetIntValue();
                service.StartingLocation = dataRow.GetCell((int)TtImportColumn.localitaPartenza)?.GetStringValue();
                service.EndingLocation = dataRow.GetCell((int)TtImportColumn.localitaArrivo)?.GetStringValue();
                service.Note = dataRow.GetCell((int)TtImportColumn.note)?.GetStringValue();
                service.StartHour = dataRow.GetCell((int)TtImportColumn.oraPartenza)?.GetDateTimeValue();
                service.EndHour = dataRow.GetCell((int)TtImportColumn.oraArrivo)?.GetDateTimeValue();

                service.TimingDescr = dataRow.GetCell((int)TtImportColumn.descrizioneFrequenzaProspetto)?.GetStringValue();
                service.RequestedSittings = dataRow.GetCell((int)TtImportColumn.postiCapitolato)?.GetIntValue();
                service.SpecReference = dataRow.GetCell((int)TtImportColumn.riferimentoCapitolato)?.GetStringValue();
                service.RunType = dataRow.GetCell((int)TtImportColumn.tipoCorsa)?.GetStringValue();
                service.ServiceTypeDescr = dataRow.GetCell((int)TtImportColumn.tipoServizio)?.GetStringValue();
                service.VariantId = dataRow.GetCell((int)TtImportColumn.variante)?.GetIntValue();

                service.RunInHoursBook = dataRow.GetCell((int)TtImportColumn.corsaInOrario).GetBooleanValue();

                service.Company = dataRow.GetCell((int)TtImportColumn.ditta)?.GetStringValue();
                service.Company1 = dataRow.GetCell((int)TtImportColumn.ditta1)?.GetStringValue();
                service.Company2 = dataRow.GetCell((int)TtImportColumn.ditta2)?.GetStringValue();
                service.Company3 = dataRow.GetCell((int)TtImportColumn.ditta3)?.GetStringValue();
                service.DestinationTable = dataRow.GetCell((int)TtImportColumn.tabellaDestinazione)?.GetStringValue();

                // dati del nodo
                node.ServiceId = service.ServiceId;
                node.CollectionPointId = dataRow.GetCell((int)TtImportColumn.NODO_codNodo)?.GetStringValue();
                node.Description = dataRow.GetCell((int)TtImportColumn.NODO_descNodo)?.GetStringValue();
                node.ProgrNumber = dataRow.GetCell((int)TtImportColumn.NODO_progressivo).GetIntValue().Value;
                node.Hour = dataRow.GetCell((int)TtImportColumn.NODO_ora)?.GetDateTimeValue();

                DateTime? dt = dataRow.GetCell((int)TtImportColumn.NODO_sosta)?.GetDateTimeValue();
                if (dt != null)
                {
                    node.MinutesStop = dt.Value.Minute;
                }

                node.ArrivedAtHour = dataRow.GetCell((int)TtImportColumn.NODO_oraArrivoFermata)?.GetDateTimeValue();
                node.Latitude = dataRow.GetCell((int)TtImportColumn.NODO_X_nodoWGS84)?.GetDecimalValue();
                node.Longitude = dataRow.GetCell((int)TtImportColumn.NODO_Y_nodoWGS84)?.GetDecimalValue();

                var item = new Tuple<TtService, TtServiceNode>(service, node);
                yield return item;
            }
        }
    }
}
