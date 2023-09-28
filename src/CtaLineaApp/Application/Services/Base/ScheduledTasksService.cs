using CtaLinea.Model.Helpers;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.ScheduledTasks;
using CtaLinea.Model.TaskRequest;
using CtaLineaApp.Application.Model;
using CtaLineaApp.Application.Services.Helper;
using Radzen.Blazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Services.Base
{
    public class ScheduledTasksService 
        : IScheduledTasksService
    {
        private readonly IHttpService _http;

        public ScheduledTasksService(
            IHttpService htto)
        {
            this.TaskDescriptions = this.GetTaskDescriptions().ToList();
			_http = htto;
        }

        public IEnumerable<AppTaskDescription> TaskDescriptions { get; private set; }

		public async Task<IEnumerable<ScheduledTaskItem>?> GetListAsync()
        {
            var data = await this._http.Get<IEnumerable<ScheduledTaskItem>>(Constants.Endpoint_ScheduledTasks);
            return data;
        }
        public async Task<ScheduledTaskItem?> GetOneAsync(int id)
        {
            var url = string.Format(Constants.Endpoint_ScheduledTasks_Single_Fmt, id);
            var data = await this._http.Get<ScheduledTaskItem>(url);
            return data;
        }
        public async Task<int?> InsertAsync(ScheduledTaskItem item)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_ScheduledTasks, item);
        }
        public async Task UpdateAsync(ScheduledTaskItem item)
        {
            var url = string.Format(Constants.Endpoint_ScheduledTasks_Single_Fmt, item.Id);
            await this._http.Put<CheckResult>(url, item);
        }
        public async Task DeleteAsync(int id)
        {
            var url = string.Format(Constants.Endpoint_ScheduledTasks_Single_Fmt, id);
            await this._http.Delete(url);
        }

        public async Task<IEnumerable<SchedulerTaskLogItem>?> GetLogListAsync(int id)
        {
            var url = string.Format(Constants.Endpoint_ScheduledTasks_Log_Fmt, id);
            var data = await this._http.Get<IEnumerable<SchedulerTaskLogItem>>(url);
            return data;
        }
        public async Task DeleteLogAsync(int id, DateTime? refDAte)
        {
            var url = string.Format(Constants.Endpoint_ScheduledTasks_Log_Fmt, id);
            if (refDAte != null)
            {
                url += string.Format("?refDate={0:yyyy-MM-dd}", refDAte);
            }
            await this._http.Delete(url);
        }

        public async Task InvokeTaskASync (
            string activityId,
            int taskId,
            object? data = null,
            int timeout = 100)
        {
            var url = string.Format (Constants.Endpoint_Task_Invoke_Fmt ,
				activityId,
				taskId,
                timeout);
			await this._http.Post<CheckResult>(url, data);
		}

        private IEnumerable<AppTaskDescription> GetTaskDescriptions ()
        {
			var optins = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			yield return new AppTaskDescription()
			{
				TaskId = Constants.Activity_ReloadScheduler,
				TaskName = "caricamento sceduler dal Db",
				DefaultArguments = null
			};
			yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_RecalcRunDays,
                TaskName = "Ricalcolo dei giorni delle corse che hanno necessità",
                DefaultArguments = JsonSerializer.Serialize(
				    new RecalcRunDaysTaskRequest()
				    {
					    MaxRuns = 50
				    }, optins)
			};
			yield return new AppTaskDescription()
			{
				TaskId = Constants.Activity_CleanLog,
				TaskName = "Esegue la pulizia del log delle attività",
				DefaultArguments = JsonSerializer.Serialize(
					new CleanTaskLogTaskRequest()
					{
						DailyRetention = 5,
						WeeklyRetention = 4,
						MonthlyRetention = 3
					}, optins)
			};
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_SendMail,
                TaskName = "Manda le mail ai consorziati",
                DefaultArguments = JsonSerializer.Serialize(
                    new WeekActivityMailSendRequest()
                    {
                        AssociateId = null,
                        ForseDestination = null,
                        ReferenceDate = null,
                    }, optins)
            };
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_RecalcCosts,
                TaskName = "Ricalcola i costi correnti",
                DefaultArguments  =null
            };
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_ImportAssociate,
                TaskName = "Import Ditte / mezzi / autisti",
                DefaultArguments = null
            };
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_ImportPoints,
                TaskName = "Import Punti di Raccolta",
                DefaultArguments = null
            };
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_ExportElastibus,
                TaskName = "Esportazione giorni Elastibus",
                DefaultArguments = JsonSerializer.Serialize(
                    new ExportElasticDaysTaskRequest()
                    {
                        ContractId = null,
                        StartDate = null,
                        EndDate = null,
                    }, optins)
            };
            yield return new AppTaskDescription()
            {
                TaskId = Constants.Activity_ExporTT,
                TaskName = "Esportazione dati per TT / PAT",
                DefaultArguments = JsonSerializer.Serialize(
                new ExportTtTaskRequest()
                {
                    ContractId = null,
                    StartDate = null,
                    EndDate = null,
                }, optins)
            };
        }
    }
}
