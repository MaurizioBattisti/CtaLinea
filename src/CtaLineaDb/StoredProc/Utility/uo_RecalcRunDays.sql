/* *********************************************************************************	
	Maurizio Battisti
	14/01/2023
	aggiorna il contenuto della tabella di supporto con i giorni della corsa
********************************************************************************* */
CREATE PROCEDURE [dbo].[uo_RecalcRunDays]
(
	@RunId		uniqueidentifier
)
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	DECLARE @Tbl_Days TABLE
	(
		RunId				uniqueidentifier NOT NULL,
		Day					datetime NOT NULL,
		CarNum				int DEFAULT(1),

		RunVariationId		uniqueidentifier NOT NULL,
		RunPeriodId			uniqueidentifier,
		WeekDay				int NOT NULL,
		Suspended			bit NOT NULL DEFAULT 0,
		OutOfPeriod			bit NOT NULL DEFAULT 0,

		RunCarId			uniqueidentifier,
		OriginalRunCarId	uniqueidentifier,
		Replaced			bit NOT NULL DEFAULT(0),

		PRIMARY KEY (RunId, Day, CarNum)
	);
	INSERT INTO @Tbl_Days
		SELECT *
			FROM [dbo].[tvf_RunDaysWithCars] (@RunId);

	-- elimina le righe non più necessarie
	DELETE d fROM dbo.RunDays d
	LEFT JOIN @Tbl_Days dd
		ON d.RunId = dd.RunId
		AND d.Day = dd.Day
		AND d.CarNum = dd.CarNum
	WHERE dd.RunCarId IS NULL
		AND d.RunId = @RunId;

	-- aggiorna i dati
	UPDATE dbo.RunDays
		SET RunVariationId = dd.RunVariationId,
			RunPeriodId = dd.RunPeriodId,
			WeekDay = dd.WeekDay,
			Suspended = dd.Suspended,
			OutOfPeriod = dd.OutOfPeriod,
			RunCarId = dd.RunCarId,
			OriginalRunCarId = dd.OriginalRunCarId,
			Replaced = dd.Replaced
		FROM dbo.Rundays d
		INNER JOIN @Tbl_Days dd
			ON d.RunId = dd.RunId
			AND d.Day = dd.Day
			AND d.CarNum = dd.CarNum;

	-- inserisce i nuovi
	INSERT INTO dbo.RunDays
			(RunId, Day, CarNum,
			RunVariationId, RunPeriodId, WeekDay,
			Suspended, OutOfPeriod,
			RunCarId, OriginalRunCarId, Replaced)
		SELECT 
			dd.RunId, dd.Day, dd.CarNum,
			dd.RunVariationId, dd.RunPeriodId, dd.WeekDay,
			dd.Suspended, dd.OutOfPeriod,
			dd.RunCarId, dd.OriginalRunCarId, dd.Replaced
			FROM @Tbl_Days dd
		LEFT JOIN dbo.RunDays d
			ON d.RunId = dd.RunId
			AND d.Day = dd.Day
			AND d.CarNum = dd.CarNum
		WHERE d.RunId IS NULL;

	RETURN 0;
END
