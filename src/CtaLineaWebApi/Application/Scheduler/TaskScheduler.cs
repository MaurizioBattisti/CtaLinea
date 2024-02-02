using CtaLinea.Model.ScheduledTasks;
using CtaLineaWebApi.Auth.Services;
using CtaLineaWebApi.Configuration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Scheduler
{
	public   class TaskScheduler
		// : IDisposable
	{
		// considera morta una attivaià dopo 4 ore che è iniziata e  non è finita
		private readonly  TimeSpan Dead_TimeSpan = TimeSpan.FromHours(4);

		// millisecondi da aspettare perima di avviare lo scheduler
		private const int FirstStart_Delay_Milliseconds = 5000;
		// millisecondi da as pettare per ogni tick dello scheduler
		private const int Tick_Milliseconds = 5000;

		private readonly Timer _timer;
		private readonly IServiceProvider _serviceProvider;
		// private readonly AutoResetEvent _autoEvent;
		private readonly ILogger _logger;
		private readonly TaskSchedulerOptions _options;

		// elenco dei task da eseguire
		private List<ScheduledTaskItem> AllTasks = new List<ScheduledTaskItem>();
	
		public TaskScheduler (
			IServiceProvider serviceProvider,
			TaskSchedulerOptions options,
			ILogger<TaskScheduler> logger)
		{
			_logger = logger;
			// _autoEvent = new AutoResetEvent(false);
			_serviceProvider = serviceProvider;
			_options = options;

			this.AddReloadSchedulerTask();
            // _autoEvent.Set();

			_timer = new Timer(
				TaskScheduler.Tick,
				this,
				FirstStart_Delay_Milliseconds,
				Tick_Milliseconds
				); ;
        }

        public IEnumerable<ScheduledTaskItem> GetActualScheduledTasks ()
		{
			var list = new List<ScheduledTaskItem>();
            // if (this._autoEvent.WaitOne(500) == true)
			{
				list = this.AllTasks.ToList();
			}
			return list;
        }

		public async Task ReplaceAllTAsks (
			IEnumerable <ScheduledTaskItem> newTasks)
		{
			// _autoEvent.Reset();

			// salva da una parte  l'elemento dello scheduler loader
			var loadSchedulerItem = AllTasks.Where(x => x.ActivityId == Constants.Activity_ReloadScheduler).FirstOrDefault();

			this.AllTasks.Clear();
			this.AllTasks.AddRange(newTasks);

			var newLoadSchedulerItem = AllTasks.Where(x => x.ActivityId == Constants.Activity_ReloadScheduler).FirstOrDefault();
			if (loadSchedulerItem != null
				&& newLoadSchedulerItem != null)
			{
				newLoadSchedulerItem.LastStart = loadSchedulerItem.LastStart;
				newLoadSchedulerItem.LastEnd = loadSchedulerItem.LastEnd;
			}

			// _autoEvent.Set();
			await Task.CompletedTask;
		}

		public async Task SetStartActivityTime (
			string activityId, 
			int id,
			DateTime start)
		{
			await Task.CompletedTask;
			var activity = this.AllTasks.Where(t => t.ActivityId == activityId && t.Id == id).SingleOrDefault();
			if (activity != null)
			{
				activity.LastStart = start;
			}
		}
		public async Task SetEndActivityTime(
			string activityId,
			int id,
			DateTime end)
		{
			await Task.CompletedTask;
			var activity = this.AllTasks.Where(t => t.ActivityId == activityId && t.Id == id).SingleOrDefault();
			if (activity != null)
			{
				activity.LastEnd = end;
			}
		}

		private void AddReloadSchedulerTask ()
		{
			// aggiunge l'attività che fa qualcosa
			AllTasks.Add(
				new ScheduledTaskItem()
				{
					// L'id del  task m,emorizzato nel db per il reload dello scheduler
					Id = 1,
					ActivityId = Constants.Activity_ReloadScheduler,
					Frequency = ScheduleFrequency.Daily,
					RrequencyMask = 0,

					StartTime = TimeSpan.FromHours(1),
					EndTime = TimeSpan.FromHours(23),
					Interval = TimeSpan.FromSeconds(30),
					Active = true
				});
		}

		private string _baseUrl = string.Empty;
		private string GetBaseUrl ()
		{
			if (string.IsNullOrEmpty (_baseUrl) == true)
			{
				_baseUrl = _options.BaseUrl;

				if (string.IsNullOrEmpty(_baseUrl) == true)
				{
					var server = _serviceProvider.GetService<IServer>();
					var addresses = server?.Features.Get<IServerAddressesFeature>();
					var baseurls = addresses?.Addresses ?? Array.Empty<string>();
					_baseUrl = baseurls.FirstOrDefault();
				}
			}
			return _baseUrl;
		}

		private string _Token = null;
		private DateTime _Token_expiration = DateTime.MinValue;

		private async Task<string> GetTokenAsync ()
		{
			// se il token è in scadenza a breve lo azzera in modo da chiederne uno nuovo
			if (DateTime.Now.AddMinutes(1) > _Token_expiration)
			{
				_Token = null;
			}

			if (_Token == null)
			{
				using var scioe = _serviceProvider.CreateScope();
				// recuera il servizio di autenticazione
				var auth = scioe.ServiceProvider.GetService<IUserService>();
				var data = await auth.AuthentifateSystemUserAsync();
				_Token = data.Token;
				_Token_expiration = data.Expiration;
			}

			return await  Task.FromResult(_Token);
		}

		private async Task CallTAskAction (
			string activityId,
			int id, 
			string arguments,
			int timeout = 0)
		{
			try
			{
				var baseUrl = this.GetBaseUrl() + "/api/tasks/";
				var http = new HttpClient();
				var token = await this.GetTokenAsync();

				var url = baseUrl + string.Format("{0}/{1}", activityId, id);

				var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Authorization = new AuthenticationHeaderValue("BEARER", token);

				if (timeout > 0)
				{
					http.Timeout = TimeSpan.FromSeconds(timeout);
					url += string.Format("?timeout={0}", timeout);
				}

				// crea il contenuto della richiesta http
				if (arguments != null)
				{
					request.Content = new StringContent(arguments);
					MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("application/json");
					request.Content.Headers.ContentType = mediaType;
				}

				await http.SendAsync(request)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Errore invocando l l'endpont per il'attività {0} ", activityId);
			}
		}

		public void DoTick ()
		{
			Tick(this);
        }

		private bool _Working = false;
		private static async void Tick (object? stateInfo)
		{
			var self = stateInfo as TaskScheduler;

			if (self._Working == false)
			// if (self._autoEvent.WaitOne(500) == true)
			{
				self._Working = true;
				var taskToRun = self.SelectTaskToRun();
				if (taskToRun != null)
				{
					taskToRun.LastStart = DateTime.Now;

					await self.CallTAskAction(
						taskToRun.ActivityId,
						taskToRun.Id,
						taskToRun.Arguments,
						taskToRun.Timeout)
						.ConfigureAwait (false);

					taskToRun.LastEnd = DateTime.Now;
				}
                // self._autoEvent.Set();
                self._Working = false;
            }

			return;
		}

		private ScheduledTaskItem SelectTaskToRun ()
		{
			var validTAsks = (from t in this.AllTasks
							 where t.Active == true
							 orderby t.LastEnd ascending
							 select t);

            DateTime midnight = DateTime.Today.Date;
			TimeSpan now = DateTime.Now - midnight;

			ScheduledTaskItem taskToRun = null;
			foreach (var item in validTAsks)
			{
				var lastStart = item.LastStart ?? DateTime.Today.Date.AddYears(-1);
				var lastEnd = item.LastEnd ?? DateTime.Today.Date.AddYears(-1);

				//  se la frequenza non è giornaliera verifica di essere nelle condizioni corrette
				bool candidate = true;
				bool mustExecute = false;
				switch (item.Frequency)
				{
					case ScheduleFrequency.Monthly:
                        candidate = (DateTime.Today.Day == item.RrequencyMask
							&& lastStart.Date < midnight);
                        break;
					case ScheduleFrequency.Weekly:
                        candidate = this.WeeklyNeedToExecute(item.RrequencyMask, lastEnd);
						break;
					case ScheduleFrequency.Daily:
						// se non è passato abbastanza tempo dall'ultima esecuzione
						candidate =  ((lastStart < midnight && item.Interval == TimeSpan.Zero)
							|| lastStart + item.Interval < DateTime.Now) ;
                        break;
				}
                if (candidate == false) continue;
				
				if (now < item.StartTime) continue;
				if (item.EndTime != TimeSpan.Zero && now > item.EndTime) continue;

				// l'erazione è ancora in corso
				if (lastEnd < lastStart)
				{
					// controlla da quanto tempo è segnata come avviata
					// se supera le 2 ore la considera morta
					if (DateTime.Now - lastStart < Dead_TimeSpan) continue;
				}

				// restituisce il task 
				taskToRun = item;
				break;
			}

			return taskToRun;
		}

		private bool WeeklyNeedToExecute (
			int mask,
			DateTime lastEnd)
		{
			// se l'ultima esecuzione è stata fatta oggi non ne fa altre
			if (lastEnd.Date >= DateTime.Today.Date) return false;
			bool needToExecute = false;

			var day = DateTime.Today.DayOfWeek;
			switch (day)
			{
				case DayOfWeek.Monday:
					needToExecute = (mask & 1) == 1;				
					break;
				case DayOfWeek.Tuesday:
					needToExecute = (mask & 2) == 2;						
					break;
				case DayOfWeek.Wednesday:
					needToExecute = (mask & 4) == 4;
					break;
				case DayOfWeek.Thursday:
					needToExecute = (mask & 8) == 8;						
					break;
				case DayOfWeek.Friday:
					needToExecute = (mask & 16) == 16;
					break;
				case DayOfWeek.Saturday:
					needToExecute = (mask & 32) == 32;
					break;
				case DayOfWeek.Sunday:
					needToExecute = (mask & 64) == 64;
					break;
			}

			return needToExecute;
		}

		public void Dispose()
		{
			_timer?.Dispose ();
		}
	}
}
