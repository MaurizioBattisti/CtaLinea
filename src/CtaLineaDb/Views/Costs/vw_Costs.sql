/* ****************************************************************************
	Maurizio Battisit
	21/01/2023
	Lista dei costi senza raggruppameni
**************************************************************************** */
CREATE VIEW [dbo].[vw_Costs]
AS 
WITH CTE_CarCosts_base AS 
(
	SELECT cc.RunCarCostId,
			cc.RunCarId,
			COALESCE(cc.STartDate, '20000101') AS StartDate,
			ROW_NUMBER() OVER (PARTITION BY cc.RunCarCostId ORDER BY COALESCE(cc.STartDate, '20000101') ) AS Num,
			cc.KmPrice,
			cc.DayPrice,
			COALESCE(cc.DayForfait, 0) AS DayForfait,
			COALESCE(cc.DayIntegration, 0) AS DayIntegration
		FROM dbo.RunCarCosts cc
), CTE_CarCosts AS
(
	SELECT cc.RunCarCostId,
			cc.RunCarId,
			cc.StartDate, 
			COALESCE(DATEADD(d, -1, cc2.STartDate), '9999-12-31') AS EndDate,
			cc.KmPrice,
			cc.DayPrice,
			cc.DayForfait,
			cc.DayIntegration
		FROM CTE_CarCosts_base cc
		LEFT JOIN CTE_CarCosts_base cc2
			ON cc.num +1 = cc2.num
)
SELECT d.*,
		COALESCE(v.Km, 0) AS Km,
		costs.DayPrice,
		costs.KmPrice,
		costs.DayForfait,
		costs.DayIntegration,
		COALESCE(v.Km, 0) * costs.KmPrice AS KmCost,

		o_costs.DayPrice AS o_DayPrice,
		o_costs.KmPrice AS o_KmPrice,
		o_costs.DayForfait AS o_DayForfait,
		o_costs.DayIntegration AS o_DayIntegration,
		COALESCE(v.Km, 0) * o_costs.KmPrice AS o_KmCost
	FROM Dbo.RunDays d
	INNER JOIN dbo.RunVariations v
		ON d.RunVariationId = v.RunVariationId
	INNER JOIN CTE_CarCosts costs
		ON d.RunCarId = costs.RunCarId
		AND d.Day BETWEEN costs.StartDate AND costs.EndDate
	INNER JOIN CTE_CarCosts o_costs
		ON d.OriginalRunCarId = o_costs.RunCarId
		AND d.Day BETWEEN o_costs.StartDate AND o_costs.EndDate
	;
