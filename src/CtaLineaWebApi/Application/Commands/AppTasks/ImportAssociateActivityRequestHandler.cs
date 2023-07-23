using CsvHelper.Configuration;
using CsvHelper;
using CtaLineaWebApi.Application.Commands.Budgets;
using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model.Import;
using NPOI.HSSF.Record;
using System.Linq;
using ZzSoft.CtaLinea.Dal.Context;
using System.Data;
using ZzSoft.CtaLinea.Dal.Repositories;
using CtaLinea.Model.External;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ImportAssociateActivityRequestHandler
        : IRequestHandler<ImportAssociateActivityRequest, bool>
    {
		private const string SQL_GetAllAssociate = @"SELECT * FROM dbo.Associate a";

		private int[] PrimaryUsage =new int[] { 5, 6, 9, 14, 17, 19, 20, 25, 27, 32, 34, 35, 39, 41, 42, 43 };
		private int[] SpareUsage = new int[] { 7, 8, 13, 16, 24, 26, 28, 29, 40, 44, 45, 46, 47 };

		private readonly ILogger _logger;
        private readonly ISchedulerTaskLogger _schedulerLogger;
		private readonly ImporterConfiguration _options;
		private readonly CultureInfo _dateCulture;
		private readonly IAssociatesRepository _associateRepo;
		private readonly CtaDbContext _context;

		public ImportAssociateActivityRequestHandler(
            ImporterConfiguration options,
			ISchedulerTaskLogger schedulerLogger,
			CtaDbContext context,
			IAssociatesRepository associateRepo,
			ILogger<ImportAssociateActivityRequestHandler> logger
            )
        {
			_context = context;
			_dateCulture = new CultureInfo ( "it-IT" );
			_options = options;
			_associateRepo = associateRepo;
			_schedulerLogger = schedulerLogger;
			_logger = logger;
        }

        public async Task<bool> Handle(
            ImportAssociateActivityRequest request, 
            CancellationToken cancellationToken)
        {
            IList<AssociateImport> associates = null;
            IList<CarImport> cars = null; ;
            IList<DriverImport> drivers = null;
			try
            {
				await _schedulerLogger.LogAsync("inizio lettura file ditte");
				// legge il contenuto del file degli associati
				associates = await this.ReadFileASync<AssociateImport>(
                    this._options.AssociateFilePath
					).ConfigureAwait(false);

				await _schedulerLogger.LogAsync("inizio lettura file mezzi");
				// legge il contenutod el file dei mezzi
				cars = await this.ReadFileASync<CarImport>(
					this._options.CarFilePath
					).ConfigureAwait(false);

				await _schedulerLogger.LogAsync("inizio lettura file autisti");
				// legge il contenuto del file degli autisti
				drivers = await this.ReadFileASync<DriverImport>(
					this._options.DriverFilePath
					).ConfigureAwait(false);
			}
			catch (Exception ex)
            {
                await _schedulerLogger.LogAsync(ex.Message, "ERROR");
                return false;
            }

			// le tre operaizoni di scrittura vengono fatte in transazione
			using var conn = _context.GetNewConnection();
			IDbTransaction tran = null;
			try
            {
				conn.Open();
				tran = conn.BeginTransaction();

				await _schedulerLogger.LogAsync("Elaborazione import ditte");
				// aggiorna i consorziati
				await this.UpdateAssociateAsync (
					associates ,
					conn, tran
					).ConfigureAwait(false);
				
				await _schedulerLogger.LogAsync("Elaborazione import mezzi");
				// aggiorna la tabella dei mezzi
				await this.UpdateCarsAsync(
					cars,
					conn, tran
					).ConfigureAwait(false);

				await _schedulerLogger.LogAsync("Elaborazione import autisti");
				// aggiorna la tabella degli autisti
				await this.UpdateDriversAsync(
					drivers,
					conn, tran
					).ConfigureAwait(false);

				// confemra la transazione
				tran.Commit();
				tran = null;
			}
            catch (Exception ex)
            {
				await _schedulerLogger.LogAsync(ex.Message, "ERROR");
                return false;
            }
			finally
			{
				if (tran != null)
				{
					tran.Rollback();
					tran = null;
				}
			}

			await _schedulerLogger.LogAsync("Importazioen Ditte / Mezzi / Autisti completata con successo");
			return true;
        }

		private DateTime? GetDAte (string stringDate)
		{
			if (string.IsNullOrWhiteSpace(stringDate)) return null;
			if (DateTime.TryParse(stringDate, _dateCulture, DateTimeStyles.AssumeLocal,  out DateTime dt) == true)
			{
				return dt;
			}
			return null;
		}

        private async Task<IList<T>> ReadFileASync<T> (
            string filePath
			)
        {
			var items = new List<T>();
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true                
			};
            using var reader = new StreamReader(filePath);
			using var csv = new CsvReader(reader, config);

			var records = csv.GetRecords<T>();
            return await Task.FromResult(records.ToList());
        }

		private bool GetPRimaryCarByUsage(int carUsage)
		{ 
			return PrimaryUsage.Contains(carUsage);
		}
		private bool GetSpareCarByUsage(int carUsage)
		{
			return SpareUsage.Contains(carUsage);
		}

		private async Task UpdateAssociateAsync (
			IList<AssociateImport> associates,
			IDbConnection conn,
			IDbTransaction tran)
		{
			// carica i consorziati dal db
			var olds = await this._associateRepo.GetAllAssociatesAsync(
				conn, tran)
				.ConfigureAwait(false);

			// crea la lista di qeulli non più presenti
			var newIds = (from a in associates select a.BsSupplierCode);
			var idsToDisactivate = (from a in olds
									where newIds.Contains(a.BsSupplierCode) == false
									select a.AssociateId);

			// crea la lista dei nuovi e li aggiunge
			var oldCodes = (from a in olds select a.BsSupplierCode); 
			var toAdd = (from a in associates
						 where oldCodes.Contains(a.BsSupplierCode) == false
						 select a);
			foreach (var a in toAdd)
			{
				// prepara un nuovo associate ID e lo aggiunge
				var newAss = new Associate()
				{
					AssociateId  = Guid.NewGuid(),
					Description = a.Description,
					BsSupplierCode = a.BsSupplierCode,
					Email = a.Email,
					BsCustomerCode = null,
					Active = true
				};
				// ... elo inserisce
				await this._associateRepo.InsertAssociateASync(newAss).ConfigureAwait (false);
			}

			// cra la lista di quelli da aggiornare
			var toUpdate = (from a in associates
							where oldCodes.Contains(a.BsSupplierCode) == true
							select a);
			foreach (var a in toUpdate)
			{
				// cerca il consorziato tra i vecchi
				var assToUpd = olds.Where(x => x.BsSupplierCode == a.BsSupplierCode).FirstOrDefault();
				if (assToUpd != null)
				{
					// controlla che ci siano delle variazioni tra i dati
					if (assToUpd.Description != a.Description
						|| assToUpd.Email != a.Email)
					{
						// aggiorna i dati
						assToUpd.Description = a.Description;
						assToUpd.Email = a.Email;
						assToUpd.Active = true;

						await this._associateRepo.UpdateAssociateASync(assToUpd).ConfigureAwait (false);
					}
				}
			}

			// aggiorna tuti quelli da aggiornare e quelli da cancellare mettendo Active a false
			foreach (var id in idsToDisactivate)
			{
				var delAss = olds.Where (x => x.AssociateId == id).FirstOrDefault();
				if (delAss != null
					&& delAss.Active == true)
				{
					delAss.Active = false;
					await this._associateRepo.UpdateAssociateASync(delAss);
				}
			}
		}

		private async Task UpdateCarsAsync(
			IList<CarImport> cars,
			IDbConnection conn,
			IDbTransaction tran)
		{
			// carica i consorziati dal db
			var oldAssociates = await this._associateRepo.GetAllAssociatesAsync(
				conn, tran)
				.ConfigureAwait(false);

			foreach (var a in oldAssociates)
			{
				// crea la query filtrata dei mezzi da improtare
				var assCars = cars.Where(x => x.BsAssociateId == a.BsSupplierCode);
				var assCarsIBsc = assCars.Select(x => x.BsCarId);

				// carica i mezzi dal db
				var oldCars = await this._associateRepo.GetAssociateCarsAsync(
					a.AssociateId, conn, tran)
					.ConfigureAwait(false);

				//  crea la lista di qeulli non più presenti				
				var carsToDel = (from c in oldCars
								 where assCarsIBsc.Contains(c.BsCarId) == false
								 select c);

				// TODO: crea la lista dei nuovi e li aggiunge


				// TODO: cra la lista di quelli da aggiornare
				// TODO; aggiorna tuti quelli da aggiornare e quelli da cancellare mettendo Active a false


			}
		}

		private async Task UpdateDriversAsync(
			IList<DriverImport> drivers,
			IDbConnection conn,
			IDbTransaction tran)
		{
			// TODO: carica gli autisti dal db



			// TODO: crea la lista di qeulli non più presenti



			// TODO: crea la lista dei nuovi e li aggiunge


			// TODO: cra la lista di quelli da aggiornare
			// TODO; aggiorna tuti quelli da aggiornare e quelli da cancellare mettendo Active a false



			await Task.CompletedTask;
		}
	}
}
