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
			cc.KmPriceExtra,
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
			cc.KmPriceExtra,
			cc.DayPrice,
			cc.DayForfait,
			cc.DayIntegration
		FROM CTE_CarCosts_base cc
		LEFT JOIN CTE_CarCosts_base cc2
			ON cc.num +1 = cc2.num
), CTE_Km AS
(
	SELECT v.RunVariationId,
			CASE  WHEN r.Extra = 1 
					THEN 1
				WHEN COALESCE(v.Km, 0)- COALESCE (fv.Km, 0) <> 0
					THEN 2
				ELSE 0
			END AS ExtraType,	-- 0 = No, 1 = SI, 2 = Parziale
			
			COALESCE(v.Km, 0) AS Km,
			COALESCE (fv.Km, 0) AS KmBase,
			CASE 
				WHEN r.Extra = 1 THEN 0 
				ELSE COALESCE (fv.Km, 0)
			END KmContract,
			CASE 
				WHEN r.Extra = 1 THEN COALESCE(v.Km, 0) 
				ELSE COALESCE(v.Km, 0)- COALESCE (fv.Km, 0)
			END KmExtra
		FROM dbo.RunVariations v
		INNER JOIN dbo.Runs r
			ON v.RunId = r.RunId
		INNER JOIN dbo.RunVariations fv
			ON r.RunId = fv.RunId
			AND fv.StartDate IS NULL
)
SELECT d.*,
		k.Km,
		k.KmBase,
		k.KmContract,
		k.KmExtra,
		k.ExtraType,	-- 1 = Extra captioato totale
						-- 2 = Extra captiolato parziale
						-- 0 = NON extra captialto
		costs.KmPrice,
		costs.KmPriceExtra,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayPrice END AS DayPrice,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayForfait END AS DayForfait,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayIntegration END AS DayIntegration,

		o_costs.KmPrice AS o_KmPrice,
		o_costs.KmPriceExtra AS o_KmPriceExtra,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayPrice END AS o_DayPrice,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayForfait END AS o_DayForfait,
		CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayIntegration END AS o_DayIntegration

	FROM Dbo.RunDays d
	INNER JOIN CTE_Km k
		ON d.RunVariationId = k.RunVariationId
	INNER JOIN CTE_CarCosts costs
		ON d.RunCarId = costs.RunCarId
		AND d.Day BETWEEN costs.StartDate AND costs.EndDate
	INNER JOIN CTE_CarCosts o_costs
		ON d.OriginalRunCarId = o_costs.RunCarId
		AND d.Day BETWEEN o_costs.StartDate AND o_costs.EndDate
	;
