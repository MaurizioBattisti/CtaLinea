/* ***************************************************************************************
	Maurizio Battisti
	23/10/2023
	restituisce il nuemro di mezzi titolari e scorta usati per l'appalto indicato nel periodo indicato
*************************************************************************************** */
CREATE PROCEDURE [dbo].[up_Dashboard_GetCarCounts]
	@ContractId			int = NULL,
	@StartDate			date = NULL,
	@EndDate			date = NULL
AS
BEGIN
	IF @ContractId IS NULL SET @ContractId = [dbo].[fn_Session_GetContractId] ();
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, NULL;

	WITH CTE_Runs AS
	(
		SELECT rc.CarId,
  			MAX (CASE rc.CarType WHEN 'P' THEN 1 ELSE 0 END ) AS Use_PRimary,
  			MAX (CASE rc.CarType WHEN 'S' THEN 1 ELSE 0 END ) AS Use_Spare
			FROM dbo.Runs r
			INNER JOIN dbo.RunDays d
				ON r.RunId = d.RunId
			INNER JOIN dbo.RunCars rc
				ON d.RunPEriodId = rc.RunPeriodId

			WHERE r.ContractId = @ContractId
				AND d.Day BETWEEN @StartDate AND @EndDate
				AND d.Suspended = 0
				AND d.OutOfPeriod = 0
				AND rc.CarType IN ('P', 'S')
		GROUP BY rc.CarId
	)
	SELECT
		SUM(d.Use_PRimary) AS NrPrimary,
		SUM(d.Use_Spare) AS NrSpare,
		SUM(CASE WHEN d.Use_PRimary = 1 AND d.Use_Spare = 1 THEN 1 ELSE 0 END) AS NrPrimarySpare,
		COUNT(*) AS UsedCars
		FROM CTE_Runs d
	;

	RETURN 0;
END
