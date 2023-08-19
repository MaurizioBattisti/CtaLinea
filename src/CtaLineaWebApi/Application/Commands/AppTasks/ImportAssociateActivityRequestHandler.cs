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
using System.Security.Cryptography;

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
				await this._associateRepo.InsertAssociateASync(newAss, conn, tran)
					.ConfigureAwait (false);
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

						await this._associateRepo.UpdateAssociateASync(assToUpd, conn, tran)
							.ConfigureAwait (false);
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
					await this._associateRepo.UpdateAssociateASync(delAss, conn, tran)
						.ConfigureAwait(false);
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

                // crea la lista dei nuovi e li aggiunge
                var oldCodes = (from c in oldCars select c.BsCarId);
                var toAdd = (from c in assCars
                             where oldCodes.Contains(c.BsCarId) == false
                             select c);
                foreach (var c in toAdd)
                {
					// prepara un nuovo car ....
					var newCar = new Car()
					{
						CarId = Guid.NewGuid(),
						AssociateId = a.AssociateId,
						BsCarId = c.BsCarId,
						RegNumber = c.RegNumber,
                        NrSittings = c.NrSittings ?? 0,
                        Description = (c.RegNumber ?? string.Empty) + (c.NrSittings != null ? string.Format(" / {0}", c.NrSittings) : string.Empty),
                        ChassisNumber = c.ChassisNumber,

						PrimaryCar = PrimaryUsage.Contains(c.CarUsage ?? 0) == true,
                        SpareCar = SpareUsage.Contains(c.CarUsage ?? 0) == true,

                        DiscontinuationDate = this.GetDate(c.DiscontinuationDate),
						FirstRegistration = this.GetDate(c.FirstRegistrationDate),
                        Active = true
                    };
                    newCar.Active = this.GetActive(newCar.DiscontinuationDate);

                    // ... elo inserisce
                    await this._associateRepo.InsertCarAsync(newCar, conn, tran)
						.ConfigureAwait(false);
                }

                //  crea la lista di quelli da aggiornare
                var toUpdate = (from c in assCars
                                where assCarsIBsc.Contains(c.BsCarId) == true
                                select c);
                foreach (var c in toUpdate)
                {
                    // cerca il mezzo tra i vecchi
                    var carToUpdt = oldCars.Where(x => x.BsCarId == c.BsCarId).FirstOrDefault();
                    if (carToUpdt != null)
                    {
						// aggiorna semrpe
						carToUpdt.RegNumber = c.RegNumber;
                        carToUpdt.NrSittings = c.NrSittings ?? 0;
                        carToUpdt.Description = (c.RegNumber ?? string.Empty) + (c.NrSittings != null ? string.Format(" / {0}", c.NrSittings) : string.Empty);
                        carToUpdt.ChassisNumber = c.ChassisNumber;

                        carToUpdt.PrimaryCar = PrimaryUsage.Contains(c.CarUsage ?? 0) == true;
                        carToUpdt.SpareCar = SpareUsage.Contains(c.CarUsage ?? 0) == true;

                        carToUpdt.DiscontinuationDate = this.GetDate(c.DiscontinuationDate);
                        carToUpdt.FirstRegistration = this.GetDate(c.FirstRegistrationDate);
                        carToUpdt.Active = this.GetActive(carToUpdt.DiscontinuationDate);

                        await this._associateRepo.UpdateCarAsync(carToUpdt, conn, tran)
							.ConfigureAwait(false);
                    }
                }
                // aggiorna tuti quelli da cancellare mettendo Active a false
                foreach (var c in carsToDel)
				{
					await this._associateRepo.DeleteCarASync(c.CarId, conn, tran)
						.ConfigureAwait (false);
				}
            }
		}

        private async Task UpdateDriversAsync(
			IList<DriverImport> drivers,
			IDbConnection conn,
			IDbTransaction tran)
		{
            // carica i consorziati dal db
            var oldAssociates = await this._associateRepo.GetAllAssociatesAsync(
                conn, tran)
                .ConfigureAwait(false);

            foreach (var a in oldAssociates)
            {
                // crea la query filtrata degli autisti da improtare
                var assDrivers = drivers.Where(x => x.BsAssociateId == a.BsSupplierCode);
                var assDriversIds = assDrivers.Select(x => x.BsDriverId);

                // carica gli atuisti dal db
                var oldDrivers = await this._associateRepo.GetAssociateDriversAsync(
                    a.AssociateId, conn, tran)
                    .ConfigureAwait(false);

                //  crea la lista di qeulli non più presenti				
                var driversToDel = (from d in oldDrivers
                                 where assDriversIds.Contains(d.BsDriverId) == false
                                 select d);

                // crea la lista dei nuovi e li aggiunge
                var oldDriverIds = (from d in oldDrivers select d.BsDriverId);
                var toAdd = (from d in assDrivers
                             where oldDriverIds.Contains(d.BsDriverId) == false
                             select d);
                foreach (var d in toAdd)
                {
                    // prepara un nuovo autista ....
                    var newDriver = new Driver()
                    {
                        DriverId = Guid.NewGuid(),
                        AssociateId = a.AssociateId,

						BsDriverId = d.BsDriverId,
						FirstName = d.FirstName,
						LastName = d.LastName,
						LicenseNumber = d.LicenseNumber,
						LicenceCategory = d.LicenceCategory,

						DismissionDate = this.GetDate( d.DismissionDate),
                        Active = true
                    };
                    newDriver.Active = this.GetActive(newDriver.DismissionDate);

                    // ... elo inserisce
                    await this._associateRepo.InsertDriverAsync(newDriver, conn, tran)
                        .ConfigureAwait(false);
                }

                //  crea la lista di quelli da aggiornare
                var toUpdate = (from d in assDrivers
                                where assDriversIds.Contains(d.BsDriverId) == true
                                select d);
                foreach (var d in toUpdate)
                {
                    // cerca l'autista tra i vecchi
                    var driverToUpdt = oldDrivers.Where(x => x.BsDriverId == d.BsDriverId).FirstOrDefault();
                    if (driverToUpdt != null)
                    {
                        // aggiorna semrpe
                        driverToUpdt.BsDriverId = d.BsDriverId;
                        driverToUpdt.FirstName = d.FirstName;
                        driverToUpdt.LastName = d.LastName;
                        driverToUpdt.LicenseNumber = d.LicenseNumber;
                        driverToUpdt.LicenceCategory = d.LicenceCategory;

                        driverToUpdt.DismissionDate = this.GetDate(d.DismissionDate);
                        driverToUpdt.Active = this.GetActive(driverToUpdt.DismissionDate);

                        await this._associateRepo.UpdateDriverAsync(driverToUpdt, conn, tran)
                            .ConfigureAwait(false);
                    }
                }
                // aggiorna tuti quelli da cancellare mettendo Active a false
                foreach (var d in driversToDel)
                {
                    await this._associateRepo.DeleteDriverASync(d.DriverId, conn, tran)
                        .ConfigureAwait(false);
                }
            }
        }

        private bool GetActive(
            DateTime? discontinuationDate,
            bool active = true)
        {
            if (discontinuationDate != null
                && discontinuationDate <= DateTime.Today.AddYears(-1)
				)
            {
                active = false;
            }
            return active;
        }
        private DateTime? GetDate(string dateStr)
        {
            DateTime? date = null;
            if (string.IsNullOrEmpty(dateStr) == false)
            {
                if (DateTime.TryParse(dateStr, out DateTime newDate) == true)
                {
                    date = newDate;
                }
            }
            return date;
        }
    }
}
