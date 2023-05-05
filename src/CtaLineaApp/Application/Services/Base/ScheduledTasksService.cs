using CtaLinea.Model.Helpers;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.ScheduledTasks;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Base
{
    public class ScheduledTasksService 
        : IScheduledTasksService
    {
        private readonly IHttpService _http;

        public ScheduledTasksService(
            IHttpService htto)
        {
            _http = htto;
        }

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
    }
}
