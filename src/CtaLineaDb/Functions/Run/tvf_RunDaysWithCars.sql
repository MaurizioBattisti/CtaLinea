/* *****************************************************************************
	Maurizio Battisti
	13/01/2023
	Calcola i giorni di una corsa considerando anche i mezzi e i relativi mezzi sostituiti
******************************************************************************/
CREATE FUNCTION [dbo].[tvf_RunDaysWithCars]
(
	@RunId	uniqueidentifier
)
RETURNS @Tbl_Days TABLE
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
)
AS
BEGIN
	-- riempie la tabella con i dati dei mezzi
	WITH CTE_PRimaryCars AS
	(
		SELECT c.*
			FROM dbo.RunCars c
			WHERE c.CarType = 'P'
	)
	INSERT INTO @Tbl_Days
		(RunId, Day, CarNum,
		RunVariationId, RunPeriodId,
		WeekDay, Suspended, OutOfPeriod,
		RunCarId, OriginalRunCarId, Replaced)
		SELECT d.RunId, d.Day, 
				ROW_NUMBER() OVER (PARTITION BY d.RunId, d.Day ORDER BY d.RunPeriodId, c.RunCarId, rd.ReplacedRunCarId) AS Num,
				d.RunVariationId, d.RunPeriodId,
				d.WeekDay,d. Suspended, d.OutOfPeriod,
				COALESCE(rd.ReplacedRunCarId, c.RunCarId) AS RunCarId, 
				c.RunCarId  AS OriginalRunCarId, 
				CASE WHEN rd.ReplacedRunCarId IS NULL THEN 0 ELSE 1 END AS Replaced
			FROM [dbo].[tvf_RunDays] (@RunId) AS d
			LEFT JOIN CTE_PRimaryCars c
				ON d.RunPeriodId = c.RunPeriodId
			LEFT JOIN dbo.RunCarReplacements r
				ON r.RunPeriodId = d.RunPeriodId
				AND d.Day BETWEEN r.StartDate AND r.EndDate
			LEFT JOIN dbo.RunCarReplacementDetails rd
				ON r.CarReplacementId = rd.CarReplacementId
				AND rd.OriginaRunCarId = c.RunCarId
			;

	RETURN;
END