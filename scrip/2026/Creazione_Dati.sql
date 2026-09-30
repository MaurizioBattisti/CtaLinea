USE CtaLineaDb
GO


/* ******** CALENDARI ************* */
GO

-- calendario annuale con le feste comandate
IF NOT EXISTS (
	SELECT 1
		FROM dbo.CalendarHolidays h
		WHERE h.CalendarId = 1
			AND h.Holiday = '2026-11-01'
	)
BEGIN
	-- inserisce le feste comandate
	WITH CTE_Holydays AS
	(
		(
			SELECT h.CalendarId,
					DATEADD(YEAR, 1, h.Holiday) AS HolidayDate,
					h.HolidayDescription
				FROM Dbo.CalendarHolidays h
				WHERE h.CalendarId = 1
					AND h.Holiday >= '2025-09-04'
					AND h.Holiday <= '2026-08-31'
					-- esclude pasqua e pasquetta
					AND h.Holiday <> '2026-04-05'
					AND h.Holiday <> '2026-04-06'
		) UNION (
			SELECT 1 AS CalendarId,
					'2027-03-28' AS HolidayDate,
					'Pasqua' AS HolidayDescription
		) UNION (
			SELECT 1 AS CalendarId,
					'2027-03-29' AS HolidayDate,
					'Pasquetta' AS HolidayDescription
		)
	)
	INSERT INTO dbo.CalendarHolidays
		(CalendarId, Holiday, HolidayDescription)
		SELECT h.CalendarId,
			h.HolidayDate,
			h.HolidayDescription
		FROM CTE_Holydays h
END
GO

-- inserisce i periodi dei caledari
IF NOT EXISTS (
	SELECT 1
		FROM dbo.CalendarPeriods p
		WHERE p.CalendarId = 4
		AND p.StartDate = '2026-09-10'
	)
BEGIN
	WITH CTE_DAta AS
	(
		(
			-- invernale
			SELECT 4 AS CalendarId,
				'2026-09-10' AS StartDate,
				'2027-06-25' AS EndDAte,
				'Invernale 26/27' AS Note
		) UNION (
			-- esitvo
			SELECT 10 AS CalendarId,
				'2026-09-10' AS StartDate,
				'2027-06-08' AS EndDAte,
				'Scolastico 26/27' AS Note
		) UNION (
			-- scolastico
			SELECT 7 AS CalendarId,
				'2027-06-26' AS StartDate,
				'2027-09-09' AS EndDAte,
				'Estivo 26/27' AS Note
		)
	)
	INSERT INTO dbo.CalendarPeriods
			(CalendarId, StartDate, EndDate, Note)
		SELECT  d.CalendarId, d.StartDate, d.EndDAte, d.Note
			FROM CTE_DAta d
		;
END
GO

DECLARE @Tbl_Dates AS TABLE (
	HolyDay			Date,
	Descr			VARCHAR(50)
);
INSERT INTO @Tbl_Dates (HolyDay, Descr)
VALUES ('2026-11-02', '+ un gg Ognissanti'),	 
	 ('2026-12-07', 'Ponte Immacolata'),
	 ('2026-12-23', 'vacanze di Natale'), 
	 ('2026-12-24', 'vacanze di Natale'), 
	 ('2026-12-28', 'vacanze di Natale'), 
	 ('2026-12-29', 'vacanze di Natale'), 
	 ('2026-12-30', 'vacanze di Natale'),
	 ('2026-12-31', 'vacanze di Natale'),
	 ('2027-01-04', 'vacanze di Natale'),
	 ('2027-01-05', 'vacanze di Natale'), 
	 ('2027-02-08', 'vacanze di Carnevale'),
	 ('2027-02-09', 'vacanze di Carnevale'),
	 ('2027-03-25', 'vacanze di Pasqua'),
	 ('2027-03-26', 'vacanze di Pasqua'),
	 ('2027-03-30', 'vacanze di Pasqua'),
	 ('2027-03-31', 'vacanze di Pasqua'),
	 ('2027-04-29', 'Festa dei lavoratori'),
	 ('2027-04-30', 'Festa dei lavoratori')
	 ;

IF NOT EXISTS (
	SELECT 1
		FROM dbo.CalendarHolidays h
		INNER JOIN @Tbl_Dates d
		ON h.CalendarId = 10
			AND h.Holiday = d.HolyDay
	)
-- aggiorna i giorni del cal calendario scolastico
BEGIN
	INSERT INTO dbo.CalendarHolidays
		(CalendarId, Holiday, HolidayDescription)
		SELECT 
			10 AS CalendarId,
			d.HolyDay AS HolyDay,
			d.Descr AS HolidayDescription
			FROM @Tbl_Dates d;
END
GO

/* *************** FINE  CALENDARI  **************** */
GO

IF NOT EXISTS (
	SELECT 1
		FROM dbo.Contracts c
		WHERE c.ContractId = 3
	)
BEGIN
	SET IDENTITY_INSERT dbo.Contracts ON;

	WITH CTE_Data AS
	(
	(
			SELECT 3 AS ContractId,
				'Extraurbano 26/27' AS ContractName,
				'Appalto extraurbano 2026 /27' AS ContractDescription,
				'2026-09-01' AS sTartDate,
				'2027-08-31' AS EndDAte
		) UNION (
			SELECT 4 AS ContractId,
				'Urbano 26/27' AS ContractName,
				'Appalto Urbano 2026 /27' AS ContractDescription,
				'2026-09-01' AS sTartDate,
				'2027-08-31' AS EndDAte
		)
	)
	INSERT INTO dbo.Contracts
		(ContractId, ContractName, ContractDescription, StartDate, EndDate)
		SELECT d.ContractId,
				d.ContractName,
				d.ContractDescription,
				d.sTartDate,
				d.EndDAte
			FROM CTE_Data d

	SET IDENTITY_INSERT dbo.Contracts OFF;
END
GO


-- CREA la tabelal di improtaizoen dei dati
-- dati da importare a mano con copia  / incolla da excel
-- DROP TABLE import.Oly_2627
-- GO
IF OBJECT_ID('import.Oly_2627', 'U') IS NULL
BEGIN
	CREATE tABLE import.Oly_2627
	(
		ContractName			VARCHAR(MAX),

		LineNumber				VARCHAR(MAX),
		RunNumber				INT,
		StartTime				TIME,
		EndTime					TIME,
		Path					VARCHAR(MAX),
		Frequency				VARCHAR(MAX),

		Km_day					DECIMAL(18, 4),
		Sittings				INT,
		DaysCount				INT,
		Km_Tot					DECIMAL(18, 4),

		NrRow					VARCHAR(MAX),
		Esito					VARCHAR(MAX),

		VectorName				VARCHAR(MAX),
		Primary_Car				VARCHAR(MAX),
		Primary_CarSittings		INT,

		Spare_Car				VARCHAR(MAX),
		Spare_CarSittings			INT,
		Spare2_Car				VARCHAR(MAX),
		Spare2_CarSittings		INT,
		Support_Car				VARCHAR(MAX),
		Support_CarSittings		INT,

		Conferma_Svc_Vector		VARCHAR(MAX),

		Cta_Requency			VARCHAR(MAX),

		Cost					DECIMAL(18, 4),
		AnnualCost				DECIMAL(18, 4),

		Note_Source				VARCHAR(MAX),

		Calendar_1				VARCHAR(MAX),
		Calendar_2				VARCHAR(MAX),
		Calendar_3				VARCHAR(MAX),

		Monday					VARCHAR(MAX),
		Tuesday					VARCHAR(MAX),
		Wednsday				VARCHAR(MAX),
		Thyrsday				VARCHAR(MAX),
		Friday					VARCHAR(MAX),
		Saturday				VARCHAR(MAX),
		Sunday					VARCHAR(MAX)
	);
END
PRINT 'Per incalre i dati aprire la tabella in Edit First 200 Rows andare sulla riga di inserimetno ed incollare il conentuod eglie xcel senz ala primariga';
GO

/* ******************************************************** */
GO


-- elimina la colonna
/*
BEGIN
    -- 1. Rimuove il vincolo di DEFAULT legato alla colonna Id
    ALTER TABLE import.Oly_2627
    DROP CONSTRAINT DF_Oly_2627_Id;

    -- 2. Rimuove la colonna Id
	ALTER TABLE import.Oly_2627
    DROP COLUMN 
        Id
END
GO
*/

-- AGgiunge la clonna ID
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('import.Oly_2627') 
      AND name = 'Id'
)
BEGIN
	ALTER TABLE import.Oly_2627
    ADD 
        Id UNIQUEIDENTIFIER NOT NULL 
			CONSTRAINT DF_Oly_2627_Id DEFAULT NEWID()
END
GO

BEGIN TRAN;
DELETE FROM dbo.Runs WHERE ContractId IN (3, 4);


BEGIN -- inserisce le linee nelal tabella radice di tutto
	PRINT 'Inserisce le  Runs';

	INSERT INTO [dbo].[Runs]
			   ([RunId]
			   ,[ContractId]
			   ,[Extra]
			   ,[ContractRowNumber]
			   ,[StartDate]
			   ,[EndDate]
			   ,[RequestedDays]
			   ,[Note]
			   ,[RunName]
			   ,[Elastibus]
			   ,[LockedDate]
			   ,[LockedNote])
		SELECT	d.Id AS RunId, 
				CASE WHEN d.ContractName = 'extraurbano' 
					THEN 3 -- Extraurbano
					ELSE 4	-- URBANO
				END AS ContractId,
				0 AS Extra,
				NULLIF(
					TRY_CAST(
						CASE 
							WHEN PATINDEX('%[^0-9]%', CAST(d.NrRow AS NVARCHAR(50))) > 0 
							THEN LEFT(CAST(d.NrRow AS NVARCHAR(50)), PATINDEX('%[^0-9]%', CAST(d.NrRow AS NVARCHAR(50))) - 1)
							ELSE CAST(d.NrRow AS NVARCHAR(50))
						END AS INT
					), 
					0
				) AS ContractRowNumber,
				NULL AS StartDate,
				NULL AS EndDAte,
				d.DaysCount AS RequestedDays,
				d.Note_Source AS Note,
				CASE WHEN d.ContractName = 'extraurbano' 
					THEN NULL
					ELSE d.ContractName
				END AS RunName,
				0 AS ElastiBus,
				NULL AS LockedDAte,
				NULL AS LockedNote
			FROM import.Oly_2627 d;
			;
END

BEGIN -- inserisce le varianti della corsa ... che sono necessarie
	PRINT 'Inserisce le variazioni della run';

	 INSERT INTO [dbo].[RunVariations]
			   ([RunVariationId]
			   ,[RunId]
			   ,[StartDate]
			   ,[LineNumber]
			   ,[RunNumber]
			   ,[Path]
			   ,[StartTime]
			   ,[EndTime]
			   ,[Monday]
			   ,[Tuesday]
			   ,[Wednesday]
			   ,[Thursday]
			   ,[Friday]
			   ,[Saturday]
			   ,[Sunday]
			   ,[RequestedFrequency]
			   ,[RequestedCapacity]
			   ,[Km]
			   ,[Note])
		SELECT NEWID() AS RunVariationId,
				d.Id AS RunId,
				NULL AS StartDate,
				CASE 
					WHEN TRY_CAST(d.LineNumber AS INT) IS NOT NULL 
						THEN CAST(d.LineNumber AS INT)
					ELSE d.RunNumber 
				END AS LineNumber,
				CASE 
					WHEN TRY_CAST(d.LineNumber AS INT) IS NULL 
						THEN d.LineNumber
					ELSE CAST(d.RunNumber AS VARCHAR(MAX))
				END AS RunNumber,
				d.Path AS Path,
				d.StartTime AS StartTime,
				d.EndTime AS EndTime,
				1 AS Monday,
				1 AS Tuesday,
				1 AS Wednesday,
				1 AS Thursday,
				1 AS Friday,
				1 AS Saturday,
				1 AS Sunday,
				d.Frequency AS RequestedFrequency,
			   d.Sittings AS RequestedCapacity,
			   d.Km_day AS Km,
			   NULL AS Note
			FROM import.Oly_2627 d 
			;
END

BEGIN  -- Aggiunge i calnedari alle runs
	PRINT 'Aggiuge i calendari alle runs';

	WITH CTE_Cals AS
	(
		(
			-- calendario 1
			SELECT d.Id AS RunId,
					c.CalendarId,
					CASE WHEN COALESCE(d.Monday, '') = 'X' THEN 1 ELSE 0 END AS Monday,
					CASE WHEN COALESCE(d.Tuesday, '') = 'X' THEN 1 ELSE 0 END AS Tuesday,
					CASE WHEN COALESCE(d.Wednsday, '') = 'X' THEN 1 ELSE 0 END AS Wednsday,
					CASE WHEN COALESCE(d.Thyrsday, '') = 'X' THEN 1 ELSE 0 END AS Thursday,
					CASE WHEN COALESCE(d.Friday, '') = 'X' THEN 1 ELSE 0 END AS Friday,
					CASE WHEN COALESCE(d.Saturday, '') = 'X' THEN 1 ELSE 0 END AS Saturday,
					CASE WHEN COALESCE(d.Sunday, '') = 'X' THEN 1 ELSE 0 END AS Sunday
					
				FROM import.Oly_2627 d
				INNER JOIN dbo.Calendars c
					ON d.Calendar_1 = c.CalendarName
		) UNION (
			-- calendario 2
			SELECT d.Id AS RunId,
					c.CalendarId,
					CASE WHEN COALESCE(d.Monday, '') = 'X' THEN 1 ELSE 0 END AS Monday,
					CASE WHEN COALESCE(d.Tuesday, '') = 'X' THEN 1 ELSE 0 END AS Tuesday,
					CASE WHEN COALESCE(d.Wednsday, '') = 'X' THEN 1 ELSE 0 END AS Wednsday,
					CASE WHEN COALESCE(d.Thyrsday, '') = 'X' THEN 1 ELSE 0 END AS Thursday,
					CASE WHEN COALESCE(d.Friday, '') = 'X' THEN 1 ELSE 0 END AS Friday,
					CASE WHEN COALESCE(d.Saturday, '') = 'X' THEN 1 ELSE 0 END AS Saturday,
					CASE WHEN COALESCE(d.Sunday, '') = 'X' THEN 1 ELSE 0 END AS Sunday
					
				FROM import.Oly_2627 d
				INNER JOIN dbo.Calendars c
					ON d.Calendar_2 = c.CalendarName
		) UNION (
			-- calendario 3
			SELECT d.Id AS RunId,
					c.CalendarId,
					CASE WHEN COALESCE(d.Monday, '') = 'X' THEN 1 ELSE 0 END AS Monday,
					CASE WHEN COALESCE(d.Tuesday, '') = 'X' THEN 1 ELSE 0 END AS Tuesday,
					CASE WHEN COALESCE(d.Wednsday, '') = 'X' THEN 1 ELSE 0 END AS Wednsday,
					CASE WHEN COALESCE(d.Thyrsday, '') = 'X' THEN 1 ELSE 0 END AS Thursday,
					CASE WHEN COALESCE(d.Friday, '') = 'X' THEN 1 ELSE 0 END AS Friday,
					CASE WHEN COALESCE(d.Saturday, '') = 'X' THEN 1 ELSE 0 END AS Saturday,
					CASE WHEN COALESCE(d.Sunday, '') = 'X' THEN 1 ELSE 0 END AS Sunday
					
				FROM import.Oly_2627 d
				INNER JOIN dbo.Calendars c
					ON d.Calendar_3 = c.CalendarName
		)	
	)
	INSERT INTO [dbo].[RunVariationCalendars]
			   ([RunVariationId]
			   ,[CalendarId]
			   ,[Exclusion]
			   ,[Monday]
			   ,[Tuesday]
			   ,[Wednesday]
			   ,[Thursday]
			   ,[Friday]
			   ,[Saturday]
			   ,[Sunday])
		SELECT rv.RunVariationId AS RunVariationId,
				d.CalendarId AS CalendarId,
				0 AS Exclusion,
				d.Monday AS Monday,
				d.Tuesday AS Tuesday, 
				d.Wednsday AS Wednesday,
				d.Thursday AS Thursday,
				d.Friday AS Friday,
				d.Saturday AS Saturday,
			   d.Sunday AS Sunday
			FROM CTE_Cals d
			INNER JOIN dbo.RunVariations rv
				ON rv.RunId = d.RunId
			;
END

BEGIN -- Aggiunge i periodi 
	PRINT 'Aggiunge i eprioi';

	INSERT INTO [dbo].[RunPeriods]
			   ([RunPeriodId]
			   ,[RunId]
			   ,[StartDate]
			   ,[EndDate]
			   ,[Monday]
			   ,[Tuesday]
			   ,[Wednesday]
			   ,[Thursday]
			   ,[Friday]
			   ,[Saturday]
			   ,[Sunday]
			   ,[Note]
			   ,[RepeatType]
			   ,[RepeatPattern])
		SELECT NEWID() AS RunPeriodId,
				d.Id AS RunId,
				NULL AS StartDate,
				NULL AS EndDate,
			   1 AS Monday,
			   1 AS Tuesday,
			   1 AS Wednesday,
			   1 AS Thursday,
			   1 AS Friday,
			   1 AS Saturday,
			   1 AS Sunday,
			   NULL AS Note,
			   'A' AS RepeatType,
			   NULL AS RepeatPattern
			FROM import.Oly_2627 d
END

BEGIN --  aggiunge i mezzi titolairi
	PRINT 'Aggiuge i meziz titolari';

	WITH CTE_PrimaryCars AS
	(
		SELECT DISTINCT 
				d.Id,
				COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) AS AssociateId,
				COALESCE (pc.CarId, p_tc.CarId) AS CarId
			FROM import.Oly_2627 d
			LEFT JOIN dbo.Associates a
				ON d.VectorName = a.Description
			LEFT JOIN import.Associate_Translate at
				ON d.VectorName = at.Oly_Descr
			LEFT JOIN dbo.Associates ta
				ON at.A_Descr = ta.Description
			LEFT JOIN dbo.Cars pc
				ON REPLACE(TRANSLATE(d.Primary_Car, ' -_.,:', '      '), ' ', '')
					= REPLACE(TRANSLATE(pc.RegNumber, ' -_.,:', '      '), ' ', '')
			LEFT JOIN import.Car_Translate p_ct
				ON REPLACE(TRANSLATE(d.Primary_Car, ' -_.,:', '      '), ' ', '')
					= REPLACE(TRANSLATE(p_ct.Oly_RegNum, ' -_.,:', '      '), ' ', '')
			LEFT JOIN dbo.Cars p_tc
				ON p_ct.CarId = p_tc.CarId
			WHERE COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) IS NOT NULL
				AND COALESCE (pc.CarId, p_tc.CarId) IS NOT NULL
	)
	INSERT INTO [dbo].[RunCars]
           ([RunCarId]
           ,[RunPeriodId]
           ,[CarType]
           ,[AssociateId]
           ,[CarId]
           ,[Note]
           ,[DriverId])
		SELECT NEWID() AS RunCarId,
				rp.RunPeriodId,
				'P' AS CarType,
				c.AssociateId,
				c.CarId,
				NULL AS Note,
				NULL AS DriverId
			FROM CTE_PrimaryCars c
			INNER JOIN dbo.RunPeriods rp
				ON c.Id= rp.RunId
END

BEGIN --  Aggiunge le scorte e i mezzi di supporto
	PRINT 'Aggiunge le scorte e i mezzi di supporoto';

	WITH CTE_SparesCars AS
	(
		(
			-- prima scorta
			SELECT DISTINCT 
					d.Id,
					COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) AS AssociateId,
					COALESCE (pc.CarId, p_tc.CarId) AS CarId
				FROM import.Oly_2627 d
				LEFT JOIN dbo.Associates a
					ON d.VectorName = a.Description
				LEFT JOIN import.Associate_Translate at
					ON d.VectorName = at.Oly_Descr
				LEFT JOIN dbo.Associates ta
					ON at.A_Descr = ta.Description
				LEFT JOIN dbo.Cars pc
					ON REPLACE(TRANSLATE(d.Spare_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(pc.RegNumber, ' -_.,:', '      '), ' ', '')
				LEFT JOIN import.Car_Translate p_ct
					ON REPLACE(TRANSLATE(d.Spare_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(p_ct.Oly_RegNum, ' -_.,:', '      '), ' ', '')
				LEFT JOIN dbo.Cars p_tc
					ON p_ct.CarId = p_tc.CarId
				WHERE COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) IS NOT NULL
					AND COALESCE (pc.CarId, p_tc.CarId) IS NOT NULL
		) UNION (
			-- seconda scorta
			SELECT DISTINCT 
					d.Id,
					COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) AS AssociateId,
					COALESCE (pc.CarId, p_tc.CarId) AS CarId
				FROM import.Oly_2627 d
				LEFT JOIN dbo.Associates a
					ON d.VectorName = a.Description
				LEFT JOIN import.Associate_Translate at
					ON d.VectorName = at.Oly_Descr
				LEFT JOIN dbo.Associates ta
					ON at.A_Descr = ta.Description
				LEFT JOIN dbo.Cars pc
					ON REPLACE(TRANSLATE(d.Spare2_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(pc.RegNumber, ' -_.,:', '      '), ' ', '')
				LEFT JOIN import.Car_Translate p_ct
					ON REPLACE(TRANSLATE(d.Spare2_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(p_ct.Oly_RegNum, ' -_.,:', '      '), ' ', '')
				LEFT JOIN dbo.Cars p_tc
					ON p_ct.CarId = p_tc.CarId
				WHERE COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) IS NOT NULL
					AND COALESCE (pc.CarId, p_tc.CarId) IS NOT NULL
		) UNION (
			-- mezzo di supporto
			SELECT DISTINCT 
					d.Id,
					COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) AS AssociateId,
					COALESCE (pc.CarId, p_tc.CarId) AS CarId
				FROM import.Oly_2627 d
				LEFT JOIN dbo.Associates a
					ON d.VectorName = a.Description
				LEFT JOIN import.Associate_Translate at
					ON d.VectorName = at.Oly_Descr
				LEFT JOIN dbo.Associates ta
					ON at.A_Descr = ta.Description
				LEFT JOIN dbo.Cars pc
					ON REPLACE(TRANSLATE(d.Support_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(pc.RegNumber, ' -_.,:', '      '), ' ', '')
				LEFT JOIN import.Car_Translate p_ct
					ON REPLACE(TRANSLATE(d.Support_Car, ' -_.,:', '      '), ' ', '')
						= REPLACE(TRANSLATE(p_ct.Oly_RegNum, ' -_.,:', '      '), ' ', '')
				LEFT JOIN dbo.Cars p_tc
					ON p_ct.CarId = p_tc.CarId
				WHERE COALESCE(pc.ASsociateId, p_tc.AssociateId, a.AssociateID, ta.ASsociateId) IS NOT NULL
					AND COALESCE (pc.CarId, p_tc.CarId) IS NOT NULL
		)
	)
	INSERT INTO [dbo].[RunCars]
           ([RunCarId]
           ,[RunPeriodId]
           ,[CarType]
           ,[AssociateId]
           ,[CarId]
           ,[Note]
           ,[DriverId])
		SELECT NEWID() AS RunCarId,
				rp.RunPeriodId,
				'S' AS CarType,
				c.AssociateId,
				c.CarId,
				NULL AS Note,
				NULL AS DriverId
			FROM CTE_SparesCars c
			INNER JOIN dbo.RunPeriods rp
				ON c.Id= rp.RunId
END


BEGIN -- inserisce i costi dei mezzi
	PRINT 'Inserisce le tariffe';

	INSERT INTO [dbo].[RunCarCosts]
			   ([RunCarCostId]
			   ,[RunCarId]
			   ,[StartDAte]
			   ,[DayPrice]
			   ,[KmPrice]
			   ,[KmPriceExtra]
			   ,[DayIntegration]
			   ,[DayForfait]
			   ,[MinDayCost]
			   ,[MaxDayCost]
			   ,[SimulationName])
		SELECT NEWID() AS RunCarCostId,
				rc.RunCarId AS RunCarId,
				NULL AS StartDAte,
				d.Cost AS DayPrice,
				0 AS KmPrice,
				3 AS KmPriceExtra,
				NULL AS DayIntegration,
				NULL AS DayForfait,
				NULL AS MinDayCost,
				NULL AS MaxDayCost,

				NULL AS SimulationName
			FROM import.Oly_2627 d
			INNER JOIN dbo.RunPeriods rp
				ON d.Id = rp.RunId
			INNER JOIN dbo.RunCars rc
				ON rp.RunPeriodId = rc.RunPeriodId
				AND rc.CarType = 'P'
			WHERE d.Cost IS NOT NULL
			;
END

-- aggiunge tutte le corse alla tabella da ricalcoalre
INSERT INTO dbo.Runs_NeedsDayRecalc
	(RunId)
	SELECT d.Id
		FROM import.Oly_2627 d
		;

-- ricalcola tutti i gironi
EXEC [dbo].[uo_RecalcRunDays_Massive];

SELECT *
	FROM dbo.vw_Runs r
	WHERE r.ContractId IN (3, 4);

ROLLBACK;
-- COMMIT;
