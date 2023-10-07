using CtaLinea.Model;
using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Runs;
using CtaLineaApp.Application.Model.Utility;
using CtaLineaApp.Application.Services.Helper;
using NPOI.SS.Formula.Functions;
using System;

namespace CtaLineaApp.Application.Services.Run
{
    public class RunRepository 
        : IRunRepository
    {
        private readonly IHttpService _http;
        private readonly IRunChecker _runChecker;

        public RunRepository(
            IHttpService http,
            IRunChecker runChecker
            )
        {
            _runChecker= runChecker;
            _http = http;
        }

        public async Task<RunItem?> GetOneAsync(
            Guid runId)
        {
            string url = string.Format(Constants.Endpoint_OneRun_Frm, runId);
            var run = await _http.Get<RunItem>(url);
            return run;
        }
		public async Task<RunItemQueryModel?> GetOneDetailAsync(
			Guid runId)
		{
			string url = string.Format(Constants.Endpoint_Run_Detail_Fmt, runId);
			var run = await _http.Get<RunItemQueryModel>(url);
			return run;
		}

		public async Task SaveRunAsync (
            Guid runId,
            RunItem model)
        {
            // prima chiama la funzione di check dei dati
            await this._runChecker.CleanGraphAsync(model);
            var checkResult = await this._runChecker.CheckRunAsync(model);
            if (checkResult.Status != CheckStatus.Failed
                // se ci sono warning è comunque possibile salvare
                // && checkResult.Status != CheckStatus.Warning
                )
            {
                // èer il badrequest viee sollevata una eccezione
                string url = string.Format(Constants.Endpoint_OneRun_Frm, runId);
                await _http.Put<RunCheckResult>(url, model);
            }
            else
            {
                throw new BadRequestException<RunCheckResult>("Bad Request", checkResult);
            }
        }
        public async Task AddNewOneAsync(
            RunItem model
            )
        {
            // prima chiama la funzione di check dei dati
            await this._runChecker.CleanGraphAsync(model);
            var checkResult = await this._runChecker.CheckRunAsync(model);
            if (checkResult.Status != CheckStatus.Failed
                && checkResult.Status != CheckStatus.Warning)
            {
                // èer il badrequest viee sollevata una eccezione
                await _http.Post<RunCheckResult>(Constants.Endpoint_Runs, model);
            }
            else
            {
                throw new BadRequestException<RunCheckResult>("Bad Request", checkResult);
            }
        }

        public async Task DeleteRunAsync(
            Guid runId)
        {
			string url = string.Format(Constants.Endpoint_OneRun_Frm, runId);
			// eseuge la cancellazione della corsa
			await _http.Delete<RunCheckResult>(url);
		}

        public async Task<IEnumerable<int>?> GetRunTagsAsync (
            Guid runId)
        {
            var url = string.Format(Constants.Endpoint_Run_Tags_Fmt, runId);
			var data = await _http.Get<IEnumerable<int>?>(url);
			return data;
		}
		public async Task SaveRunTagsAsync(
			Guid runId,
            IEnumerable<int> tags)
		{
			var url = string.Format(Constants.Endpoint_Run_Tags_Fmt, runId);
			await this._http.Post<CheckResult>(url, tags);
		}

        public async Task <IEnumerable<RunNode>?> DecodeNodeAsync (string text)
        {
            var data = new RunNodeDecodeRequest() { NodesText = text };
			return await this._http.Post<IEnumerable<RunNode>, string>(Constants.Endpoint_Run_NodeDecode, data);
		}

        public async Task<MultiRunOperationResponse?> SetRunsSuspensionsAsync(
            MultiRunSetSuspensionRequest request)
        {
            return await this._http.Post<MultiRunOperationResponse, CheckResult>(
                Constants.Endpoint_Run_SetSuspensions, 
                request);
        }

        public async Task<OperationResult<Guid>?> CreateRunCopyAsync (
			CreateRunCopyRequest copyRequest
			)
        {
			return await this._http.Post<OperationResult<Guid>, CheckResult>(
	            Constants.Endpoint_Run_CreateCopy,
				copyRequest);
		}

        public async Task<InternalNoteQueryItem?> GetRunNoteAsync (Guid runId)
        {
            var url = string.Format(Constants.Endpoint_OneRun_Note_Frm, runId);
            var data = await _http.Get<InternalNoteQueryItem?>(url);
            return data;
        }
        public async Task<OperationResult<bool>?> UpdateRunInternalNoteAsync (
            Guid runId,
            InternalNoteQueryItem note)
        {
            var url = string.Format(Constants.Endpoint_OneRun_Note_Frm, runId);
            return await this._http.Post<OperationResult<bool>, CheckResult>(
                url,
                note);
        }
        public async Task<IEnumerable<RunForPrintModel>?> GetRunsForPrintAsync (
            GetRunForPrintRequest request)
        {
            var url = Constants.Endpoint_Run_ForPRint;
            return await this._http.Post<IEnumerable<RunForPrintModel>, CheckResult>(
                url,
                request);
        }
    }
}
