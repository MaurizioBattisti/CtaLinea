/* ********************************************************************
	Maurizio Battisti
	07/10/2023
	Seleziona i mezzi validi in base ai chilometri percorsi
******************************************************************** */
CREATE PROCEDURE [dbo].[up_Sim_SelectCarsByTotalKm]
	@SimulationName		varchar(50),
	@ContractId			int,
	@StartDate			Date,
	@EndDate			date,
	
	--id del match da assegnare
	@Sim_CarMAtchId		uniqueidentifier,

	@MinKm				float = NULL,
	@MaxKm				float = NULL,
	@MinKmCContract		float = NULL,
	@MaxKmContract		float = NULL,
	@MinKmExtra			float = NULL,
	@MaxKmExtra			float = NULL
AS
bEGIN
	DECLARE @WhatCheck		int = 0;
	
	-- test in ordine inverso per dare priorità ai chilometir totali
	IF @MinKmExtra IS NOT NULL OR @MaxKmExtra IS NOT NULL  SET @WhatCheck = 3;
	IF @MinKmCContract IS NOT NULL OR @MaxKmContract IS NOT NULL  SET @WhatCheck = 2;
	IF @MinKm IS NOT NULL OR @MaxKm IS NOT NULL  SET @WhatCheck = 1;
		
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );

	IF @WhatCheck > 0
	BEGIN
		-- applica uno dei filtri
		INSERT INTO dbo.SimCarMatches
			(Sim_CarMAtchId, CarId)
			SELECT @Sim_CarMAtchId,
					rc.CarId
				FROM [dbo].[tvf_Costs] (NULL) costs
				INNER JOIN dbo.RunDays d
					ON costs.RunId = d.RunId
					AND costs.Day = d.Day
				INNER JOIN dbo.Runs r
					ON costs.RunId = r.RunId
				INNER JOIN dbo.RunCars rc
					ON costs.RunCarId = rc.RunCarId
				WHERE d.Suspended = 0
					AND d.OutOfPeriod = 0
					AND (@ContractId IS NULL
						OR r.ContractId = @ContractId)
					AND (costs.Day BETWEEN @StartDate AND @EndDate)
				GROUP BY rc.CarId
				HAVING (
						@WhatCheck = 1
						AND  (@MinKm IS NULL
								OR SUM(costs.Km) >=@MinKm)
							AND (@MaxKm IS NULL
								OR SUM(costs.Km) <= @MaxKm)
						)
					OR (
						@WhatCheck = 2
						AND  (@MinKmCContract IS NULL
								OR SUM(costs.KmContract) >=@MinKmCContract)
							AND (@MaxKmContract IS NULL
								OR SUM(costs.KmContract) <= @MaxKmContract)
						)
					OR (
						@WhatCheck = 3
						AND  (@MinKmExtra IS NULL
								OR SUM(costs.KmExtra) >=@MinKmExtra)
							AND (@MaxKmExtra IS NULL
								OR SUM(costs.KmExtra) <= @MaxKmExtra)
						);
	END
	ELSE
	BEGIN
		INSERT INTO dbo.SimCarMatches
			(Sim_CarMAtchId, CarId)
			SELECT DISTINCT 
					@Sim_CarMAtchId,
					rc.CarId
				FROM dbo.RunCars rc;
	END
	
	RETURN 0;
END
