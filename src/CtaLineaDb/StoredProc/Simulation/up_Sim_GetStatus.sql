/* ********************************************************************
	Maurizio Battisti
	07/10/2023
	Restituisce los tato di copertura della simulazione
******************************************************************** */
CREATE PROC [dbo].[up_Sim_GetStatus]
	@SimulationName		varchar(50),
	@ContractId			int,
	@StartDate			Date,
	@EndDate			date
AS
BEGIN
	WITH CTE_Totals AS
	(
		SELECT 
				COUNT(CASE WHEN data.Simulated = 1 THEN data.Simulated END) AS SimulatedCount,
				COUNT(*) AS TotalCount
			FROM [dbo].[tvf_Sim_Base] (@SimulationName, @ContractId, @StartDate, @EndDate) AS data
	)
	SELECT t.*,
			t.SimulatedCount * 100.0 / CASE WHEN t.TotalCount = 0 THEN  1 ELSE t.TotalCount END AS CoveragePerc
		FROM CTE_Totals t
		;

	RETURN 0;
END
