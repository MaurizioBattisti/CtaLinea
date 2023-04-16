/* *****************************************************************************
    Maurizio Battisti
    16/04/2023
    carica un lenco con le sovrapposizioni dun runperiod id 
    nel periodo indicato
    con tutti gli altri runperiod id dove lo stesso meszzo viene usato contemporanemanete
    indicando l'orario
***************************************************************************** */
CREATE PROCEDURE [dbo].[up_Check_CarOverlappings]
	@RunCarId   uniqueidentifier,
	@StartDate	DATE,
    @EndDate	DATE 
AS
BEGIN
    DECLARE @RunId uniqueidentifier = NULL;
    -- recupera l'id della corsa  di base
    SELECT @RunId = rp.RunId
    FROM dbo.RunCars rc
    INNER JOIN dbo.RunPeriods rp
        ON rp.RunPeriodId = rc.RunPeriodId;

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, @RunId;

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
    )
    SELECT DISTINCT    
	     run.CtaRunId,
	     contr.ContractName,
	     run.ContractRowNumber,
	     run.RunName,
	     vars.LineNumber,
	     vars.RunNumber,
	     vars.Path,

   	     o2.Day,
   	     o2.CarType,
   	     o2.StartTime,
   	     o2.EndTime

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
	    INNER JOIN dbo.Runs run
		    ON o2.RunId = run.RunId
	    INNER JOIN dbo.Contracts AS contr
		    ON run.ContractId = contr.ContractId
	    INNER JOIN dbo.RunVariations vars
		    ON o2.RunVariationId = vars.RunVariationId

        WHERE (@RunCarId IS NULL
   	     OR o.RunCarId = @RunCarId)
        ;
	
    RETURN 0;
END
