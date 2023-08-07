using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Model;
using ZzSoft.CtaLinea.Dal.Utility;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class TtServiceRepository 
        : ITtServiceRepository
    {
        private const string Tbl_TtServices = "dbo.TtServices";
        private const string Tbl_TtServiceNodes = "dbo.TtNodes";

        private readonly ILogger _logger;
        private readonly CtaDbContext _context;

        private readonly string Sql_InsertService;
        private readonly string Sql_UpdateService;
        private readonly string SQL_InsertNodes;
        private readonly IZzRequestConstx _zzContext;

        public TtServiceRepository(
            CtaDbContext context,
            IZzRequestConstx zzContext,
            ILogger<TtServiceRepository> logger
            )
        {
            this._context = context;
            this._zzContext = zzContext;    
            this._logger = logger;

            // comandi di insert
            this.Sql_InsertService = this.GetInsertCommnad<TtService>(Tbl_TtServices);
            this.SQL_InsertNodes = this.GetInsertCommnad<TtServiceNode>(
                Tbl_TtServiceNodes,
                new List<string> { nameof(TtServiceNode.NodeId) });

            // comandi di uopdate
            this.Sql_UpdateService = this.GetUpdateCommnad<TtService>(
                Tbl_TtServices,
                new List<string> { nameof(TtService.ServiceId) } );
        }

        public async Task<bool> CanImportAsync(
            string importDescr)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            string sql = "SELECT COUNT(*) FROM dbo.Imports WHERE ImportDescr=@Descr AND ImportStatus = 'PROGRESS' AND LastUpdateDate >= dateadd(SECOND,-10,getdate())";
            var num = await conn.ExecuteScalarAsync<int>(sql,
                new { Descr = importDescr });
            return (num == 0);
        }

        public async Task<ImportContext> StartImportAsync(
            string importDescr,
            string user
            )
        {
            ImportContext context = null;

            if (await this.CanImportAsync(importDescr) == true)
            {
                using IDbConnection conn = this._context.GetNewConnection();
                conn.Open();
                await conn.InitializeSession(this._zzContext);

                context = new ImportContext(Guid.NewGuid(), conn);

                await conn.ExecuteAsync(
                    "INSERT INTO dbo.Imports (Id, ImportDescr, [User]) VALUES (@Id, @Descr, @User);",
                    new { Id = context.Id, Descr = importDescr, User = user }
                    );
            }
            return context;
        }

        public async Task ImportCopleteAsync(
            ImportContext context)
        {
            await this.SetStatusAsync(context, "COMPLETED");
        }

        public async Task ImportFailedAsync(
            ImportContext context)
        {
            await this.SetStatusAsync(context, "ERROR");
        }
        public async Task ImportAbortedAsync(
            ImportContext context)
        {
            await this.SetStatusAsync(context, "ABORTED");
        }
        private async Task SetStatusAsync(
            ImportContext context,
            string importStatus)
        {
            await context.Connection.ExecuteAsync(
                "UPDATE dbo.Imports SET ImportStatus = @ImportStatus, Note = @Note, LastUpdateDate = SYSDATETIME() WHERE Id = @Id;",
                new { ImportStatus = importStatus, Note = context.Note, Id = context.Id }
                );
        }

        public async Task ReportProgressAsync(
            ImportContext context)
        {
            await context.Connection.ExecuteAsync(
                "UPDATE dbo.Imports SET ImportedElements = @ImportedElements, ProcessedElements = @ProcessedElements, LastUpdateDate = SYSDATETIME() WHERE Id = @Id;",
                new { ImportedElements = context.ImportedElements, ProcessedElements = context.ProcessedElements, Id = context.Id }
                );
        }

        public async Task FinalizeImportServiceASync (
            ImportContext context
            )
        {
            // chiama una stored proc che cistema tutte le righe CTA per i servizi nuovi
            await context.Connection.ExecuteAsync(
                "dbo.up_FinalizeServiceImport",
                new { ImportId = context.Id } ,
                commandType: CommandType.StoredProcedure
                );
        }
        public async Task ImportServiceASync(
            ImportContext context,
            TtService service,
            IEnumerable<TtServiceNode> nodes)
        {
            bool newRow = false;
            string variations = string.Empty;

            context.ProcessedElements++;

            var oldSvc = await context.Connection.QueryFirstOrDefaultAsync<TtService>(
                "SELECT * FROM dbo.TtServices WHERE ServiceId = @ServiceId",
                new { ServiceId = service.ServiceId });

            int rowsAffected = 0;
            if (oldSvc != null)
            {
                // necessitas un aggiornamento
                variations = context.GetCahngesMessage<TtService>(service, oldSvc);

                if (string.IsNullOrWhiteSpace(variations) == false)
                {
                    // aggiorna la riga di dati nel database
                    await context.Connection.ExecuteAsync(
                        this.Sql_UpdateService,
                        service);
                }
            }
            else
            {
                // è una nuova riga
                newRow = true;

                rowsAffected = await context.Connection.ExecuteAsync(
                    this.Sql_InsertService,
                    service);
                if (rowsAffected == 0)
                {
                    // segna un errore
                    if (context.Note == null) context.Note = string.Empty;
                    context.Note += string.Format("\nla riga per il servizio {0} non è stata inserita", service.ServiceId);
                    return;
                }
                variations = "Nuovo servizio";
            }

            bool nodesChanged = false;
            bool insertAllNodes = false;
            // gestisce i nodi
            if (newRow == true)
            {
                insertAllNodes = true;
            }
            else
            {
                // contorlla quali devono essere inseiti e quali sono da aggiornare
                var oldNodes = await context.Connection.QueryAsync<TtServiceNode>(
                    "SELECT * FROM " + Tbl_TtServiceNodes + " WHERE ServiceId = @ServiceId ORDER BY ProgrNumber",
                    new { ServiceId = service.ServiceId });

                var oldNodeList = oldNodes.ToList();

                if (oldNodeList.Count != nodes.Count())
                {
                    nodesChanged = true;
                    variations += "\nNr di nodi modificato";
                }
                else
                {
                    var index = 0;
                    // controlla riga per riga se sono cambiati i nodi
                    foreach (var node in nodes.OrderBy(n =>  n.ProgrNumber))
                    {
                        var old = oldNodeList[index];
                        string difference = context.GetCahngesMessage<TtServiceNode>(node, old);
                        if (string.IsNullOrWhiteSpace(difference) == false)
                        {
                            variations = "sono stati modificati i nodi";
                            nodesChanged = true;
                            break;
                        }
                        ++index;
                    }
                }
                if (nodesChanged == true)
                {
                    // elimina i vecchi nodi e li re-inserisce tutti
                    await context.Connection.ExecuteAsync(
                        "DELETE FROM " + Tbl_TtServiceNodes + " WHERE ServiceId = @ServiceId",
                        new { ServiceId = service.ServiceId });

                    // indica che devono essere reinseriti tutti i dati
                    insertAllNodes = true;
                }
            }
            if (insertAllNodes == true)
            {
                // tutti i nodi devono essere inseriti
                foreach (var node in nodes)
                {
                    await context.Connection.ExecuteAsync(
                        SQL_InsertNodes,
                        node);
                }
            }

            // inserisce la riga nella tabella dei dettagli delle importazioni
            if (string.IsNullOrWhiteSpace(variations) == false)
            {
                // se necessario aggiorna i dati specifici di CTA
                if (oldSvc != null)
                {
                    // l'aggiornamento verso le tabelle CTA viene fatto solo se il servizio è stato modificato perchè
                    // per i nuovi servizi esiste una procedura cumulativa alla fine
                    await this.UpdateCtaSpecificDataASync(
                        context,
                        service,
                        oldSvc,
                        nodes);
                }

                await context.Connection.ExecuteAsync(
                    "INSERT INTO dbo.ImportDetails (ImportId, ServiceId, Note) VALUES (@ImportId, @ServiceId, @Note)",
                    new { ImportId = context.Id, ServiceId = service.ServiceId, Note = variations } );
            }

            // incrementa il numero dei servizi importati
            if (string.IsNullOrWhiteSpace(variations) == false)
            {
                context.ImportedElements++;
            }
        }

        private async Task UpdateCtaSpecificDataASync (
            ImportContext context,
            TtService service,
            TtService oldService,
            IEnumerable<TtServiceNode> nodes)
        {
            // per ilmomento nonf a nulla
            await Task.CompletedTask;

            return;
        }

        #region Helper SQL
        private string GetInsertCommnad<T> (
            string tableName,
            IList<string> keyFields = null)
        {
            var sb = new StringBuilder(1024);
            var type = typeof(T);
            sb.Append("INSERT INTO ");
            sb.Append(tableName);
            sb.Append("( ");
            bool first = true;
            if (keyFields == null) keyFields = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                var fieldName = prop.Name;
                if (keyFields.Contains(fieldName) == true) continue;
                fieldName = this.EscapeFieldName(fieldName);

                // calcola il nome del campo
                var sqlFieldAttr = (from a in prop.GetCustomAttributes(true)
                                    where a is SqlFieldAttribute
                                    select a as SqlFieldAttribute)
                                    .FirstOrDefault();
                if (sqlFieldAttr != null)
                {
                    if (string.IsNullOrEmpty(sqlFieldAttr.Name) == false)
                    {
                        fieldName = this.EscapeFieldName(sqlFieldAttr.Name);
                    }
                }
                
                if (first == false) sb.Append(",");
                sb.Append(fieldName);
                first = false;
            }
            sb.Append(" ) VALUES ( ");
            first = true;
            foreach (var prop in type.GetProperties())
            {
                if (keyFields.Contains(prop.Name) == true) continue;
                if (first == false) sb.Append(",");
                sb.Append("@");
                sb.Append(prop.Name);
                first = false;
            }
            sb.Append(" )");

            return sb.ToString();
        }
        private string GetUpdateCommnad<T>(
            string tableName,
            IList<string> keyFields)
        {
            var sb = new StringBuilder(1024);
            var type = typeof(T);
            sb.Append("UPDATE ");
            sb.Append(tableName);
            sb.Append(" SET ");
            bool first = true;
            foreach (var prop in type.GetProperties())
            {
                var fieldName = prop.Name;
                if (keyFields.Contains(fieldName) == true) continue;
                fieldName = this.EscapeFieldName(fieldName);

                // calcola il nome del campo
                var sqlFieldAttr = (from a in prop.GetCustomAttributes(true)
                                    where a is SqlFieldAttribute
                                    select a as SqlFieldAttribute)
                                    .FirstOrDefault();
                if (sqlFieldAttr != null)
                {
                    if (string.IsNullOrEmpty(sqlFieldAttr.Name) == false)
                    {
                        fieldName = this.EscapeFieldName(sqlFieldAttr.Name);
                    }
                }

                if (first == false) sb.Append(",");
                sb.Append(fieldName);
                sb.Append("=@");
                sb.Append(prop.Name);
                first = false;
            }
            sb.Append(" WHERE ");
            first = true;
            foreach (var key in keyFields)
            {
                if (first == false) sb.Append(" AND ");
                sb.Append(key);
                sb.Append(" = @" + key);
            }

            return sb.ToString();
        }
        #endregion

        private string EscapeFieldName(string filedName)
        {
            return "[" + filedName + "]";
        }

    }
}
