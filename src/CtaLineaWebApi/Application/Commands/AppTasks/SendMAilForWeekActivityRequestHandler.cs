using CtaLinea.Model.Utilities;
using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Application.Services;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class SendMAilForWeekActivityRequestHandler
        : IRequestHandler<SendMAilForWeekActivityRequest, bool>
	{
        private class MailDetailData
        {
            public int? LineNumber { get; set; }
            public string RunNumber { get; set; }
            public TimeSpan? StartTime { get; set; }
            public string Path { get; set; }

            public string DayDescr { get; set; }
            public string RunDescr { get; set; }

        }

        private readonly IMailSender _mailSender;
        private readonly CtaDbContext _context;
        private readonly ISchedulerTaskLogger _schedulerLogger;
        private readonly ILogger _logger;
        private readonly CultureInfo _culture;

        public SendMAilForWeekActivityRequestHandler(
            CtaDbContext context,
            IMailSender mailSender,
            ISchedulerTaskLogger schedulerLogger,
            ILogger<SendMAilForWeekActivityRequestHandler> logger
			)
		{
            _mailSender = mailSender;
            _schedulerLogger = schedulerLogger;
            _context = context;
            _logger = logger;
            _culture = new CultureInfo( "it-IT" );
        }

		public async Task<bool> Handle(
            SendMAilForWeekActivityRequest request, 
			CancellationToken cancellationToken)
		{
            try
            {
                await _schedulerLogger.LogAsync(
                    string.Format ("Elaborazione dati per invio mail data: {0:dd/MM/yyyy}, id Ditta: {1}, invia sempre a : {2}",
                        request.ReferenceDate ?? DateTime.Today,
                        request.AssociateId?.ToString () ?? "<Tutti>",
                        request.ForseDestination ?? string.Empty
                        ));

                // per prima cosa ricalcola tutti i gironi delle corse che hanno modifiche  pendenti
                using var conn = _context.GetNewConnection();
                conn.Open();
                await conn.ExecuteAsync(
                    "[dbo].[uo_RecalcRunDays_Massive]",
                    new
                    {
                        ProcessCount = 0,
                        RunId = (Guid?)null
                    },
                    commandType: System.Data.CommandType.StoredProcedure,
                    commandTimeout: request.Timeout);

                if (string.IsNullOrWhiteSpace(request.ForseDestination) == true) request.ForseDestination = null;

                // recupera le informaizoni delle attività
                var activities = await conn.QueryAsync<AssociateActivityWeeksForMail> (
                    "[dbo].[up_AssociateActivityWeeksForMail]",
                    new
                    {
                        RefDate = request.ReferenceDate,
                        AssociateId = request.AssociateId,
                        ForceDsetination = request.ForseDestination

                    },
                    commandType: System.Data.CommandType.StoredProcedure,
                    commandTimeout: request.Timeout);

                if (activities != null)
                {
                    var associateList = (from d in activities
                                        select new
                                        {
                                            d.AssociateId,
                                            d.AssociateDescr,
                                            d.Email
                                        })
                                        .Distinct();

                    // cicla su tutti consorziati
                    foreach (var associate in associateList)
                    {
                        await _schedulerLogger.LogAsync(
                            string.Format( "Preparazione mail per {0} da inviare a {1}",
                                associate.AssociateDescr ?? "<no descr>",
                                associate.Email ?? "<no mail>"
                                ));

                        // recupera il delta delle attività
                        var delta = activities.Where (x => x.AssociateId == associate.AssociateId);

                        var body = this.PrepareMAilBody(
                            associate.AssociateId,
                            associate.AssociateDescr,
                            associate.Email,
                            delta);

                        // traccia il corpo della mail nel log
                        await _schedulerLogger.LogAsync(body);

                        // invia la mail
                        await this.SEndAsync(
                            associate.AssociateId,
                            associate.AssociateDescr,
                            associate.Email,
                            body)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    await _schedulerLogger.LogAsync("NEssuna differenza tra le settimane");
                }
            }
            catch (Exception ex)
            {
                await _schedulerLogger.LogAsync("Invio mail attività fallito " + " " + ex.Message, "ERROR");
            }

            return true;
		}

        private string PrepareMAilBody (
            Guid associateId,
            string associateDescr,
            string email,
            IEnumerable<AssociateActivityWeeksForMail> delta)
        {
            var notToDo = this.GetAggregatedDetails(
                delta.Where(x => x.Status == "OLD")
                );
            var newToDo = this.GetAggregatedDetails(
                delta.Where(x => x.Status == "NEW")
                );
            var allToDo = this.GetAggregatedDetails(
                delta.Where(x => x.Status == "ALL")
                );

            var crLf = "\r\n";
            var sb = new StringBuilder(1024);

            sb.Append("Buongiorno ");
            sb.Append(associateDescr);
            sb.Append(",");
            sb.Append(crLf);
            sb.Append(crLf);
            sb.Append("La prossima settimana abbiamo individuato differenze nel calendario, rispetto alla settimana in corso,.");

            if (notToDo.Count() > 0)
            {
                sb.Append(crLf);
                sb.Append("Non dovranno essere effettuati le seguenti corse:");
                sb.Append(crLf);
                foreach (var run in notToDo)
                {
                    sb.Append(this.GetRunDescr(run));
                    sb.Append(crLf);
                }
            }

            if (newToDo.Count() > 0)
            {
                sb.Append(crLf);
                sb.Append("dpvranno essere eseguite in aggiunta le seguenti corse:");
                sb.Append(crLf);
                foreach (var run in newToDo)
                {
                    sb.Append(this.GetRunDescr(run));
                    sb.Append(crLf);
                }
            }

            if (allToDo.Count() > 0)
            {
                sb.Append(crLf);
                sb.Append(crLf);
                sb.Append("Quindi dovranno essere eseguite le seguenti corse:");
                sb.Append(crLf);
                foreach (var run in allToDo)
                {
                    sb.Append(this.GetRunDescr(run));
                    sb.Append(crLf);
                }
            }

            sb.Append(crLf);
            sb.Append("Saluti");
            sb.Append(crLf);
            sb.Append("Lo Staff CTA Linea");

            return sb.ToString();
        }
        private IEnumerable<MailDetailData> GetAggregatedDetails (
            IEnumerable<AssociateActivityWeeksForMail> source
            )
        {
            var qry = (from a in source
                       group a by new { a.LineNumber, a.RunNumber, a.StartTime, a.Path, a.RunDataDescription } into d
                       select new MailDetailData()
                       {
                           LineNumber = d.Key.LineNumber,
                           RunNumber = d.Key.RunNumber ?? "<nr corsa non indicato>",
                           StartTime = d.Key.StartTime,
                           Path = d.Key.Path ?? "<percorso non indicato>",
                           RunDescr = d.Key.RunDataDescription,
                           DayDescr = string.Join(", ",
                                    d.Select(a =>
                                        string.Format(_culture, "{0:dddd}", a.Day)
                                    )
                            )
                       });
            return qry;
        }

        private string GetRunDescr (MailDetailData detail)
        {
            var hour = "<No orario>";
            if (detail.StartTime != null)
            {
                var tonly = new TimeOnly(
                    detail.StartTime.Value.Hours,
                    detail.StartTime.Value.Minutes,
                    detail.StartTime.Value.Seconds
                    );
                hour = string.Format(_culture, "{0:hh:mm}", tonly);
            }

            return string.Format(
                _culture,
                "{0} - {1} {2} {3} {4} {5}",
                detail.LineNumber ?? 0,
                detail.RunNumber,
                hour,
                string.IsNullOrWhiteSpace( detail.Path) == false ? detail.Path : "<percorso non definito>",
                detail.DayDescr ?? string.Empty,
                detail.RunDescr ?? string.Empty
                );;
        }

        private async Task SEndAsync(
            Guid associateId,
            string associateDescr,
            string email,
            string body)
        {
            try
            {
                // prepara l'invio del messaggio
                var msg = new CtaLineaWebApi.Application.Model.MailMessage(
                    new List<string>() { email },
                    "Notifica servizi di linea",
                    body
                    );

                await this._mailSender.SendEmailAsync(msg);
                await _schedulerLogger.LogAsync(
                    string.Format ("mail inviata a {0} con successo",
                    email));
            }
            catch ( Exception ex )
            {
                await _schedulerLogger.LogAsync("errore di invio mail" + ex.Message, "ERROR");
            }
        }
    }
}
