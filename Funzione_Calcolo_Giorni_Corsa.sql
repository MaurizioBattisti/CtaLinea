USE CtaLineaDb
GO

DECLARE @RunId uniqueidentifier = '5D4862F3-2575-498C-9C82-028EAD995BAD';

BEGIN
	DECLARE @Tbl_Days TABLE
	(
			RunId			uniqueidentifier NOT NULL,
			Day				datetime NOT NULL,
			CarNum			int NOT NULL,

			AssociateId		uniqueidentifier,
			CarId			uniqueidentifier,
			RunCarCostId	uniqueidentifier,
			O_RunCarCostId	uniqueidentifier,

			WeekDay			int NOT NULL,
			Suspended		bit NOT NULL DEFAULT 0,

			PRIMARY KEY (RunId, Day, CarNum)
	);

	WITH CTE_PrimaryCars AS
	(
		SELECT *
			FROM dbo.RunCars c
			WHERE C.CarType = 'P'
	), CTE_ReplacedCars AS
	(
		SELECT *
			FROM dbo.RunCars c
			WHERE C.CarType = 'R'
	)
	INSERT INTO @Tbl_Days
		SELECT d.RunId,
				d.Day,
				ROW_NUMBER() OVER (PARTITION BY d.RunId, d.Day ORDER BY pc.CarId) AS Num,
				pc.AssociateId,
				pc.CarId,

				NULL, NULL,

				d.WeekDay,
				d.Suspended
			FROM [dbo].[tvf_AttendedRunDays](@RunId) d
			INNER JOIN dbo.RunPeriods p
				ON p.RunId = d.RunId
			INNER JOIN CTE_PrimaryCars pc
				ON p.RunPEriodId = pc.RunPeriodId

			;

	SELECT * FROM @Tbl_Days


END