USE CtaLineaDb
GO

DECLARE @RunId uniqueidentifier = '5D4862F3-2575-498C-9C82-028EAD995BAD';

BEGIN
	-- calcola i valori dei grioni della settimana
	DECLARE @Monday int = DATEPART(dw, '20221226');
	DECLARE @Tuesday int = DATEPART(dw, '20221227');
	DECLARE @Wednesday int = DATEPART(dw, '20221228');
	DECLARE @Thursday  int = DATEPART(dw, '20221229');
	DECLARE @Friday int = DATEPART(dw, '20221230');
	DECLARE @Saturday int = DATEPART(dw, '20221224');
	DECLARE @Sunday int= DATEPART(dw, '20221225');

	DECLARE @StartDate Date;
	DECLARE @EndDAte AS DAte;

	DECLARE @Tbl_Days TABLE
	(
			RunId			uniqueidentifier NOT NULL,
			Day				datetime NOT NULL,
			CarNum			int NOT NULL,

			AssociateId		uniqueidentifier,
			CarId			uniqueidentifier,
			CarType			char(1), 
			RunCarCostId	uniqueidentifier,

			WeekDay			int NOT NULL,
			Suspended		bit NOT NULL DEFAULT 0,

			PRIMARY KEY (RunId, Day, CarNum)
	);

	SELECT @StartDate = CASE WHEN  COALESCE(r.StartDate, c.StartDate) < c.StartDate THEN C.StartDAte ELSE COALESCE(r.StartDate, c.StartDate) END,
			@EndDAte = CASE WHEN  COALESCE(r.EndDate, c.EndDate) > c.EndDate THEN C.EndDate ELSE COALESCE(r.EndDate, c.EndDate) END
		FROM dbo.Runs r
		INNER JOIN Dbo.Contracts c
			ON R.ContractId = c.ContractId
		WHERE r.RunId = @RunId;

	WITH CTE_CarCosts_base AS
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY rcc.RunCarId ORDER BY COALESCE(rcc.StartDate, p.StartDate, @StartDate) ) AS Num,
				COALESCE(rcc.StartDate, p.StartDate, @StartDate) AS StartDAte,
				rcc.RunCarCostId,
				rcc.RunCarId
			FROM dbo.RunCarCosts rcc
			INNER JOIN dbo.RunCars rc
				ON rc.RunCarId = rcc.RunCarId
			INNER JOIN dbo.RunPeriods p
				ON rc.RunPeriodId = p.RunPEriodId
	), CTE_CarCosts AS
	(
		SELECT c.RunCarCostId,
				c.RunCarId,
				c.Num,
				c.StartDAte,
				COALESCE (DATEADD(d, -1,  c_2.StartDate), @EndDAte) AS EndDAte
			FROM CTE_CarCosts_base c
			LEFT JOIN  CTE_CarCosts_base c_2
				ON c_2.num = c.num +1
	), CTE_PrimaryPrReplacementCars AS
	(
		SELECT *
			FROM dbo.RunCars c
			WHERE C.CarType IN ('P', 'R')
	), CTE_ReplacedCars AS
	(
		SELECT c.*,
				r.CarReplacementId,
				r.StartDate,
				r.EndDate
			FROM CTE_PrimaryPrReplacementCars c
			INNER JOIN dbo.RunCarReplacementDetails crd
				ON crd.ReplacedRunCarId = c.RunCarId
			INNER JOIN dbo.RunCarReplacements r
				ON crd.CarReplacementId = r.CarReplacementId
			WHERE C.CarType = 'R'
	)
	-- INSERT INTO @Tbl_Days
		SELECT d.RunId,
				d.Day,
				ROW_NUMBER() OVER (PARTITION BY d.RunId, d.Day ORDER BY pc.CarId) AS Num,
				pc.AssociateId,
				pc.CarId,
				pc.CarType,
				cc.RunCarCostId, 

				p.RunPEriodId,
				pc.RunCarId,
				org.CarReplacementId,


				(CASE WHEN org.CarReplacementId IS NULL THEN 0 ELSE 1 END) AS  ReplacedCar,
				d.WeekDay,
				d.Suspended
			FROM [dbo].[tvf_AttendedRunDays](@RunId) d
			INNER JOIN dbo.RunPeriods p
				ON p.RunId = d.RunId
			INNER JOIN CTE_PrimaryPrReplacementCars pc
				ON p.RunPEriodId = pc.RunPeriodId
			LEFT JOIN CTE_CarCosts AS cc
				ON cc.RunCarId = pc.RunCarId
				AND d.day BETWEEN cc.StartDAte AND cc.EndDAte
			LEFT JOIN dbo.RunCarReplacementDetails org
				ON pc.RunCarId = org.OriginaRunCarId
			WHERE (
					(p.Monday =1 AND d.WeekDay = @Monday)
					OR (p.Tuesday =1 AND d.WeekDay = @Tuesday)
					OR (p.Wednesday =1 AND d.WeekDay = @Wednesday)
					OR (p.Thursday =1 AND d.WeekDay = @Thursday)
					OR (p.Friday =1 AND d.WeekDay = @Friday)
					OR (p.Saturday =1 AND d.WeekDay = @Saturday)
					OR (p.Sunday =1 AND d.WeekDay = @Sunday)
				)
			;
			

		-- SELECT * FROM CTE_ReplacedCars;


	-- SELECT * FROM @Tbl_Days;





END