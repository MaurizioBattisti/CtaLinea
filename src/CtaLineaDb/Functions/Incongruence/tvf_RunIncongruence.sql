/* ***************************************************************************************
	Maurizio Battisti
	17/06/2023
	calcola le incongruenze delle corse

	invokation Example

DECLARE @RunId			uniqueidentifier = NULL;
DECLARE @StartDate		date = NULL;
DECLARE @EndDAte		date = NULL;
DECLARE @WhatToCheck	varchar(MAX) = NULL

SELECT d.RunId,
	STRING_AGG(d.IncongreunceDescr, '; ') AS IncongreunceDescr
	FROM [dbo].[tvf_RunIncongruence](@RunId, @StartDate, @EndDAte, @WhatToCheck) d
	GROUP BY d.RunId;
*************************************************************************************** */
CREATE FUNCTION [dbo].[tvf_RunIncongruence]
(
	@RunId			uniqueidentifier = NULL,
	@StartDate		date = NULL,
	@EndDAte		date = NULL,
	@WhatToCheck	varchar(MAX) = NULL
)
RETURNS @Tbl_IncData TABLE
(
	RunId				uniqueidentifier NOT NULL,
	IncType				INT NOT NULL,
	IncongreunceDescr	Varchar(MAX),

	PRIMARY KEY (RunId, IncType)
)
AS
BEGIN
	DECLARE @Tbl_What AS TABLE (
		IncType		int PRIMARY KEY
	);
	IF NOT @WhatToCheck IS NULL
	BEGIN
		INSERT INTO @Tbl_What (IncType)
		SELECT DISTINCT value FROM STRING_SPLIT(@WhatToCheck, ',');
	END

	DECLARE @Tbl_Runs AS TABLE 
	(
		RunId		uniqueidentifier NOT NULL PRIMARY KEY
	);

	SET @StartDate = COALESCE(@StartDate, '20000901');
	SET @EndDAte = COALESCE(@EndDAte, '99990831');

	DECLARE @Tbl_Inc AS TABLE
	(
		RunId		uniqueidentifier NOT NULL,
		IncType		INT NOT NULL,

		PRIMARY KEY (RunId, IncType)
	);

	-- Crea la tabella temporanea con le corse da considerare
	INSERT INTO @Tbl_Runs (RunId)
			SELECT r.RunId
				FROM dbo.Runs r
				WHERE (r.StartDate IS NULL OR r.StartDate <= @EndDate)
					AND (r.EndDate IS NULL OR r.EndDate >= @StartDate)
	;

	--1  Corsa senza nessun mezzo Titolare
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 1)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.Runs r
				LEFT JOIN dbo.RunPeriods rp
					ON R.RunId = rp.RunId
				LEFT JOIN dbo.RunCars rc
					ON rp.RunPEriodId = rc.RunPeriodId
					AND rc.CarType = 'P'
				WHERE rc.CarId IS NULL				
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 1
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	--2  Corsa con giorni senza mezzo Titolare
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 2)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.RunDays r
				INNER JOIN @Tbl_Runs rr
					ON r.RunId = rr.RunId
				LEFT JOIN dbo.RunCars rc
					ON R.RunCarId = rc.RunCarId
				WHERE rc.CarId IS NULL
					AND r.Day BETWEEN @StartDate AND @EndDAte
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 2
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 3 corsa con mezzi senza nessun costo
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 3)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.Runs r
				INNER JOIN dbo.RunPeriods rp
					ON R.RunId = rp.RunId
				INNER JOIN dbo.RunCars rc
					ON rp.RunPEriodId = rc.RunPeriodId
					AND rc.CarType  IN ( 'P', 'R')
				LEFT JOIN dbo.RunCarCosts rcc
					ON rc.RunCarId = rcc.RunCarId
				WHERE rcc.RunCarCostId IS NULL
					OR ( rcc.SimulationName IS NULL					
						AND 0 = (
							COALESCE(rcc.DayPrice, 0) 
							+ COALESCE(rcc.DayForfait, 0) 
							+ COALESCE(rcc.DayIntegration, 0) 
							+ COALESCE(rcc.KmPrice, 0) 
							+ COALESCE(rcc.KmPriceExtra, 0)
							)
						)
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 3
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 4)
	-- 4  Corsa con giorni senza costi
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.RunDays r
				INNER JOIN @Tbl_Runs rr
					ON r.RunId = rr.RunId
				INNER JOIN dbo.RunCars rc
					ON R.RunCarId = rc.RunCarId
				LEFT JOIN dbo.RunCarCosts rcc
					ON rc.RunCarId = rcc.RunCarId
				WHERE r.Day BETWEEN @StartDate AND @EndDAte
					AND (rcc.RunCarCostId IS NULL
						OR (rcc.SimulationName IS NULL
							AND 0 = (
								COALESCE(rcc.DayPrice, 0) 
								+ COALESCE(rcc.DayForfait, 0) 
								+ COALESCE(rcc.DayIntegration, 0) 
								+ COALESCE(rcc.KmPrice, 0) 
								+ COALESCE(rcc.KmPriceExtra, 0)
								)
							)
					)
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 4
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 5 corsa con mezzo con uso scorretto
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 5)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.Runs r
				INNER JOIN dbo.RunPeriods rp
					ON R.RunId = rp.RunId
				INNER JOIN dbo.RunCars rc
					ON rp.RunPEriodId = rc.RunPeriodId
					AND rc.CarType IN ( 'P', 'S')
				INNER JOIN dbo.Cars c
					ON Rc.CarId = c.CarId
				WHERE (c.PrimaryCar = 0 AND rc.CarType = 'P')
					OR (c.SpareCar = 0 AND c.PrimaryCar = 0 AND rc.CarType = 'S')
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 5
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 6 corsa con mezzo non più attivo (venduto)
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 6)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.RunDays r
				INNER JOIN @Tbl_Runs rr
					ON r.RunId = rr.RunId
				INNER JOIN dbo.RunCars rc
					ON R.RunCarId = rc.RunCarId
				INNER JOIN dbo.Cars c
					ON Rc.CarId = c.CarId
				WHERE r.Day BETWEEN @StartDate AND @EndDAte
					AND c.DiscontinuationDate IS NOT NULL
					AND c.DiscontinuationDate <= r.Day
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 6
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 7 Corsa con mezzo  di scorta mancante
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 7)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM Dbo.Runs r
				INNER JOIN dbo.RunPeriods rp
					ON R.RunId = rp.RunId
				LEFT JOIN dbo.RunCars rc
					ON rp.RunPEriodId = rc.RunPeriodId
					AND rc.CarType = 'S'
				WHERE rc.CarId IS NULL				
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 7
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 8 corsa senza calendario 
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 8)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM dbo.RunVariations r
				LEFT JOIN dbo.RunVariationCalendars rvc
					ON rvc.RunVariationId = r.RunVariationId
				WHERE rvc.CalendarId IS NULL				
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 8
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 9 corsa senza giorni della settimana
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 9)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM dbo.RunVariations r
				WHERE r.Monday = 0
					AND r.Tuesday =0
					AND r.Wednesday = 0
					AND r.Thursday = 0
					AND r.Friday = 0
					AND R.Saturday = 0
					AND R.Sunday = 0
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 9
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	--10  corsa senza alcun giorno
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 10)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT r.RunId
				FROM @Tbl_Runs r
				LEFT JOIN dbo.RunDays d
					ON r.RunId = d.RunId
				WHERE  d.Day BETWEEN @StartDate AND @EndDAte
				GROUP BY r.RunId
				HAVING COUNT(*) = 0
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 10
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 11 cosa con Km non indicati sulal corsa  o su una variante
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 11)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM dbo.RunVariations r
				WHERE COALESCE(r.Km, 0) = 0
					
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 11
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 12 cosa con Km non indicati sulal corsa  o su una variante
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 12)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT rv.RunId				
				FROM dbo.RunVariations rv
				INNER JOIN dbo.RunNodes rn
					ON Rv.RunVariationId = rn.RunVariationId
				GROUP BY rv.RunId				
				HAVING COUNT(rn.RunNodeId) < 2
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 12
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 13 corsa con capienza richiesta non indicata
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 13)
	BEGIN 
		WITH CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM dbo.RunVariations r
				WHERE COALESCE(r.RequestedCapacity, 0) <= 0
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 13
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END
	-- 14 corsa con capienza del mezzo inferiore alla richiesta
	IF @WhatToCheck IS NULL OR EXISTS(SELECT 1 FROM @Tbl_What w WHERE w.IncType = 14)
	BEGIN 
		WITH CTE_Data AS
		(
			SELECT d.RunId,
					d.RunVariationId,
					d.Day,
					SUM(c.NrSittings) AS NrSittings
				FROM dbo.RunDays d
				INNER JOIN dbo.RunCars rc
					ON d.RunCarId = rc.RunCarId
				INNER JOIN dbo.Cars c
					ON rc.CarId = c.CarId
				WHERE d.Day BETWEEN @StartDate AND @EndDAte
				GROUP BY d.RunId, d.RunVariationId, d.Day
		), CTE_Inc AS 
		(
			SELECT DISTINCT r.RunId
				FROM dbo.RunVariations r
				INNER JOIN CTE_Data d
					ON r.RunVariationId = d.RunVariationId
				WHERE r.RequestedCapacity < d.NrSittings
		)
		INSERT INTO @Tbl_Inc (RunId, IncType)
			SELECT d.RunId, 14
				FROM CTE_Inc d
				WHERE (@RunId IS NULL OR d.RunId = @RunId);
	END

	-- preprare i risultati
	INSERT INTO @Tbl_IncData
	SELECT d.RunId,
			d.IncType,
			CASE d.IncType
				WHEN 1 THEN 'Corsa senza nessun mezzo titolare'
				WHEN 2 THEN 'Corsa con giorni senza mezzo titolare'
				WHEN 3 THEN 'corsa con mezzi senza nessun costo'
				WHEN 4 THEN 'Corsa con giorni senza costi'
				WHEN 5 THEN 'corsa con mezzo con uso scorretto'
				WHEN 6 THEN 'corsa con mezzo non più attivo (venduto)'
				WHEN 7 THEN 'Corsa con mezzo  di scorta mancante'
				WHEN 8 THEN 'corsa senza calendario '
				WHEN 9 THEN 'corsa senza giorni della settimana'
				WHEN 10 THEN 'corsa senza alcun giorno'
				WHEN 11 THEN 'corsa con Km non indicati sulal corsa  o su una variante'
				WHEN 12 THEN 'corsa con meno di 2 fermate'
				WHEN 13 THEN 'corsa con capienza richiesta non indicata'
				WHEN 14 THEN 'corsa con capienza del mezzo inferiore alla richiesta'
				ELSE 'sconosciuto' 
			END AS Descr
		FROM @Tbl_Inc d;


	RETURN;
END
