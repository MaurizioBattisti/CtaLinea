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
            var notToDo = delta.Where(x => x.Status == "OLD");
            var newToDo = delta.Where(x => x.Status == "NEW");
            var allToDo = delta.Where(x => x.Status == "ALL");

            var crLf = "\n";
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
                    sb.Append(this.getRunDescr(run));
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
                    sb.Append(this.getRunDescr(run));
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
                    sb.Append(this.getRunDescr(run));
                    sb.Append(crLf);
                }
            }

            sb.Append(crLf);
            sb.Append("Saluti");
            sb.Append(crLf);
            sb.Append("Lo Staff CTA Linea");

            return sb.ToString();
        }
        private string getRunDescr (AssociateActivityWeeksForMail run)
        {
            var hour = "<No orario>";
            if (run.StartTime != null)
            {
                var tonly = new TimeOnly(
                    run.StartTime.Value.Hours,
                    run.StartTime.Value.Minutes,
                    run.StartTime.Value.Seconds
                    );
                hour = string.Format(_culture, "{0:hh:mm}", tonly);
            }

            return string.Format(
                _culture,
                "nr Linea: {0} corsa {1} delle {2} percorso {3} il girono: {4:dddd} {5} ",
                run.LineNumber ?? 0,
                run.RunNumber ?? "<nessun numero>",
                hour,
                string.IsNullOrWhiteSpace( run.Path) == false ? run.Path : "<percorso non definito>",
                run.Day,
                run.RunDataDescription ?? string.Empty
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
