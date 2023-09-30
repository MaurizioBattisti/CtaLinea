/* ******************************************************************************************
	Maurizio Battisti
	14/01/2023
	calcola il dettaglio dei costi prendendo in considerazione i parametri indicati
****************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCosts]
(
	@ContractId		int = NULL,
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@AssociateId	uniqueidentifier = NULL,
	@CarId			uniqueidentifier = NULL,
	@RunId			uniqueidentifier = NULL,

	@SimulationName	varchar(50) = NULL
)
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, @RunId;

	WITH CTE_BaseCosts AS
	(
		SELECT  DISTINCT c.RunId,
				r.ContractId,
				c.RunCarId,
				rc.AssociateId,
				rc.CarId,
				c.Day,
				c.Km,
				c.KmContract,
				c.KmExtra,
				
				c.KmContract * c.KmPrice AS KmCostContract,
				c.KmExtra * c.KmPriceExtra AS KmCostExtra,

				c.DayPrice,
				c.DayForfait,
				c.DayIntegration
			FROM [dbo].[tvf_Costs](@SimulationName) c
			INNER JOIN dbo.RunDays d
				ON c.RunId = d.RunId
				AND c.CarNum = d.CarNum
				AND c.Day = d.Day
			INNER JOIN dbo.Runs r
				ON c.RunId = r.RunId
			INNER JOIN dbo.RunCars rc
				ON c.RunCarId = rc.RunCarId
			WHERE d.Suspended = 0
				AND d.OutOfPeriod = 0
				AND (@RunId IS NULL
					OR c.RunId = @RunId)
				AND (@ContractId IS  NULL
					OR r.ContractId = @ContractId)
				AND (@AssociateId IS NULL	
					OR rc.AssociateId = @AssociateId)
				AND (@CarId IS NULL
					OR rc.CarId = @CarId)
				AND (@StartDate IS NULL
					OR c.Day >= @StartDate)
				AND (@EndDate IS NULL
					OR c.Day <= @EndDate)
	), CTE_CostsGrouped AS
	(
		SELECT  c.ContractId,
				c.AssociateId,
				c.CarId,

				SUM(c.KmContract) AS Tot_KmContract,
				SUM(c.KmExtra) AS Tot_KmExtra,
				SUM(c.KmCostContract) AS Tot_KmCostContract,
				SUM(c.KmCostExtra) AS Tot_KmCostExtra,

				SUM(c.DayPrice) AS Tot_DayPrice,
				SUM(c.DayForfait) AS Tot_DayForfait,
				SUM(c.DayIntegration) AS Tot_DayIntegration
			FROM CTE_BaseCosts c
			GROUP BY c.ContractId,
				c.AssociateId,
				c.CarId
	)
	SELECT ct.ContractName,
			a.Description AS AssociateDescr,
			car.Description AS CarDescr,
			c.*,
			COALESCE(c.Tot_KmCostContract, 0) +
			COALESCE(c.Tot_KmCostExtra, 0) +
			COALESCE(c.Tot_DayPrice, 0) +
			COALESCE(c.Tot_DayForfait, 0) +
			COALESCE(c.Tot_DayIntegration, 0) AS Tot

		FROM CTE_CostsGrouped c
		INNER JOIN dbo.Contracts ct
			ON c.ContractId = ct.ContractId
		INNER JOIN dbo.Associates a
			ON c.AssociateId = a.AssociateId
		INNER JOIN dbo.Cars car
			ON c.CarId = car.CarId
		ORDER BY ct.ContractName,
				a.Description,
				car.Description
			;
			
	RETURN 0;
END
