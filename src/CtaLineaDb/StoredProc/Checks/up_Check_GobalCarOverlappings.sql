/* ***********************************************************************************
	Maurizio Battisit
	01/07/2023
	Calcolo delle sovrapposizioni globali
*********************************************************************************** */
CREATE PROCEDURE [dbo].[up_Check_GobalCarOverlappings]
	@StartDate	 DATE  = NULL,
	@EndDate	 	 DATE = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	IF @StartDate IS NULL  SET @StartDate = [dbo].[fn_GetStartDate](COALESCE(@EndDate, GETDATE()));
	IF @EndDate IS NULL  SET @EndDate = [dbo].[fn_GetEndtDate](COALESCE(@StartDate, GETDATE()));

	WITH CTE_Hours AS
	(
		SELECT n.RunVariationId,
  			  MIN(n.Hour) AS MinHour,
  			  MAX(n.Hour) AS MaxHour
  		  FROM dbo.RunNodes n
  		  GROUP BY n.RunVariationId
	), CTE_Overlap AS
	(
		SELECT rd.RunId,
  		  rd.Day,
  		  COALESCE(h.MinHour, v.StartTime) AS StartTime,
  		  COALESCE(h.MaxHour, v.EndTime) AS EndTime,
		 rd.RunVariationId,
  		  rd.RunCarId,
  		  c.CarId,
  		  c.CarType
		FROM dbo.RunDays rd
		INNER JOIN dbo.RunVariations v
  		  ON Rd.RunVariationId = v.RunVariationId
		INNER JOIN dbo.RunCars c
  		  ON Rd.RunCarId = c.RunCarId
		LEFT JOIN CTE_Hours h
  		  ON h.RunVariationId = v.RunVariationId
		WHERE rd.OutOfPeriod = 0
  		  AND rd.Suspended = 0
		 AND rd.Day BETWEEN @StartDate AND @EndDate
	), CTE_Data_All AS 
	(
		SELECT o.RunId AS f_RunId,
				o.RunVariationId AS f_RunVariationId,
				o.CarType AS f_CarTyp,
				o.StartTime AS f_StartTime,
				o.EndTime AS f_EndTime,

				o2.RunId AS s_RunId,
				o2.RunVariationId AS s_RunVariationId,
				o2.CarType AS s_CarTyp,
				o2.StartTime AS s_StartTime,
				o2.EndTime AS s_EndTime,

				MIN(o2.Day) AS MinDay,
				MAX(o2.Day) AS MaxDay,
				COUNT(o2.Day) AS DayCount
	
			FROM  CTE_Overlap o
			INNER JOIN CTE_Overlap o2
  			  -- non conisdera se stesso
  			  ON o.RunCarId <> o2.RunCarId
  			  -- solo lo stesso mezzo
  			  AND o.CarId = o2.CarId
  			  -- nello steso giorno
  			  AND o.Day = o2.Day
  			  -- con gli orari che si sovrappongono
  			  AND o.StartTime <o2.EndTime
  			  AND o.EndTime > o2.StartTime
  			  -- verifica la modalità del mezzo
  			  AND (o.CarType IN ( 'P', 'R')
  				  OR o.CarType = 'S' AND o2.CarType IN ( 'P', 'R')
  				  )
		GROUP BY o.RunId, o.RunVariationId,
				o.CarType, o.StartTime, o.EndTime,
				o2.RunId, o2.RunVariationId,
				o2.CarType, o2.StartTime, o2.EndTime
	)
	SELECT d.f_RunId AS o_RunId,
			d.f_RunVariationId AS o_RunVariationId,
			d.s_RunId AS RunId,
			d.s_RunVariationId AS RunVariationId,

			run_o.CtaRunId AS o_CtaRunId,
			contr_o.ContractName AS o_ContractName,
			run_o.ContractRowNumber AS o_ContractRowNumber,
			run_o.RunName AS o_RunName,

			run.CtaRunId,
			contr.ContractName,
			run.ContractRowNumber,
			run.RunName,

			vars_o.LineNumber AS o_LineNumber,
			vars_o.RunNumber AS o_RunNumber,
			vars_o.Path AS o_Path,

			vars.LineNumber,
			vars.RunNumber,
			vars.Path,

			d.f_CarTyp AS o_CarType,
			d.s_CarTyp AS CarType,
			d.f_StartTime AS o_StartTime,
			d.f_EndTime AS o_EndTime,
			d.s_StartTime AS StartTime,
			d.s_EndTime AS EndTime,

			d.MinDay,
			d.MaxDay,
			d.DayCount

		FROM CTE_Data_All d
			INNER JOIN dbo.Runs run_o
	   			 ON d.f_runId = run_o.RunId
			INNER JOIN dbo.Runs run
				ON d.s_RunId = run.RunId
			INNER JOIN dbo.Contracts AS contr
				ON run.ContractId = contr.ContractId
			INNER JOIN dbo.Contracts AS contr_o
				ON run_o.ContractId = contr_o.ContractId
			INNER JOIN dbo.RunVariations vars
   				ON d.s_RunVariationId = vars.RunVariationId
			INNER JOIN dbo.RunVariations vars_o
   				ON d.f_RunVariationId = vars_o.RunVariationId
		;

	RETURN 0;
END
