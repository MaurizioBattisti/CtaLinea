/* *******************************************************
	Maurizio Battisti
	29/09/2023
	Restituisce i costi associati ai mezzi 
	selezionando tra quelli reali e quellid i una particoalre simulazione
******************************************************* */
CREATE FUNCTION [dbo].[tvf_Costs]
(
	@SimulationName		Varchar(50) =NULL	
)
RETURNS @Tbl_Costs TABLE
(
	RunId				uniqueidentifier NOT NULL,
	CarNum				int NOT NULL,
	Day					date NOT NULL,
	RunVariationId		uniqueidentifier NOT NULL,
	RunCarId			uniqueidentifier NOT NULL,
	OriginalRunCarId	uniqueidentifier,
			
	Km					real,
	KmBase				money,
	KmContract			money,
	KmExtra				money,
	ExtraType			int,-- 1 = Extra captioato totale
							-- 2 = Extra captiolato parziale
							-- 0 = NON extra captialto
	
	KmPrice				money,
	KmPriceExtra		money,
	DayPrice			money,
	DayForfait			money,
	DayIntegration		money,

	o_KmPrice			money,
	o_KmPriceExtra		money,
	o_DayPrice			money,
	o_DayForfait		money,
	o_DayIntegration	money
)
AS
BEGIN
	
	WITH CTE_CarCosts_base AS 
	(
		SELECT cc.RunCarCostId,
				cc.RunCarId,
				COALESCE(cc.StartDate, '20000101') AS StartDate,
				ROW_NUMBER() OVER (PARTITION BY cc.RunCarId ORDER BY COALESCE(cc.STartDate, '20000101') ) AS Num,
				cc.KmPrice,
				cc.KmPriceExtra,
				cc.DayPrice,
				COALESCE(cc.DayForfait, 0) AS DayForfait,
				COALESCE(cc.DayIntegration, 0) AS DayIntegration,
				COALESCE(cc.MinDayCost, 0) AS MinDayCost,
				COALESCE(cc.MaxDayCost, 0) AS MaxDayCost
			FROM dbo.RunCarCosts cc
			WHERE (@SimulationName IS NULL 
					AND cc.SimulationName IS NULL)
				OR cc.SimulationName = @SimulationName
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
				cc.DayIntegration,
				cc.MinDayCost,
				cc.MaxDayCost
			FROM CTE_CarCosts_base cc
			LEFT JOIN CTE_CarCosts_base cc2
				ON cc.RunCarId = cc2.RunCarId
				AND cc.num + 1 = cc2.num
	), CTE_Km AS
	(
		SELECT v.RunVariationId,
				CASE 
					WHEN r.Elastibus = 1
						THEN 0
					WHEN r.Extra = 1 
						THEN 1
					WHEN COALESCE(v.Km, 0)- COALESCE (fv.Km, 0) <> 0
						THEN 2
					ELSE 0
				END AS ExtraType,	-- 0 = No, 1 = SI, 2 = Parziale
			
				-- considera se è elastibus
				CASE WHEN r.Elastibus = 1 THEN COALESCE(v.Km, 0) ELSE COALESCE(v.Km, 0) END AS Km,
				CASE WHEN r.Elastibus = 1 THEN COALESCE(v.Km, 0) ELSE COALESCE (fv.Km, 0) END AS KmBase,

				CASE 
				WHEN r.Elastibus = 1  THEN COALESCE (fv.Km, 0)
					WHEN r.Extra = 1 AND r.Elastibus = 0 THEN 0 
					ELSE COALESCE (fv.Km, 0)
				END KmContract,
				CASE 
					WHEN r.Elastibus = 1  THEN 0
					WHEN r.Extra = 1 THEN COALESCE(v.Km, 0) 
					ELSE COALESCE(v.Km, 0)- COALESCE (fv.Km, 0)
				END KmExtra
			FROM dbo.RunVariations v
			INNER JOIN dbo.Runs r
				ON v.RunId = r.RunId
			INNER JOIN dbo.RunVariations fv
				ON r.RunId = fv.RunId
				AND fv.StartDate IS NULL
	), CTE_Days AS
	(
		(
			SELECT d.*,
					0 AS UseKm,
					0 AS Km
				FROM dbo.RunDays d
				INNER JOIN Dbo.runs r
					ON d.RunId = r.RunId
				WHERE r.Elastibus = 0 OR @SimulationName IS NOT NULL
		) UNION (
			SELECT d.*,
					r.Elastibus AS UseKm,
					ed.Km AS Km
				FROM dbo.RunDays d
				INNER JOIN Dbo.runs r
					ON d.RunId = r.RunId
				INNER JOIN dbo.RunElastibusDays ed
					ON ed.RunId = d.RunId
					AND ed.Day = d.Day
				WHERE r.Elastibus = 1 AND  @SimulationName IS NULL
		)
	), CTE_Final_Costs AS
	(
		SELECT d.RunId,
				d.CarNum,
				d.Day,
				d.RunVariationId,
				d.RunCarId,
				d.OriginalRunCarId,
			
				CASE WHEN d.UseKm = 1 THEN d.Km ELSE k.Km END AS Km,
				CASE WHEN d.UseKm = 1 THEN d.km ELSE k.KmBase END AS KmBase,
				CASE WHEN d.UseKm = 1 THEN d.Km ELSE k.KmContract END AS KmContract,
				k.KmExtra,
				k.ExtraType,	-- 1 = Extra captioato totale
								-- 2 = Extra captiolato parziale
								-- 0 = NON extra captialto
				costs.KmPrice,
				costs.KmPriceExtra,
				-- ora considera sempre i costi di algriono , integrazione e forfait
				--  CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayPrice END AS DayPrice,
				-- CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayForfait END AS DayForfait,
				-- CASE WHEN k.ExtraType = 1 THEN 0 ELSE costs.DayIntegration END AS DayIntegration,
				costs.DayPrice AS DayPrice,
				costs.DayForfait AS DayForfait,
				costs.DayIntegration AS DayIntegration,

				o_costs.KmPrice AS o_KmPrice,
				o_costs.KmPriceExtra AS o_KmPriceExtra,

				-- ora considera sempre i costi di algriono , integrazione e forfait
				o_costs.DayPrice AS o_DayPrice,
				o_costs.DayForfait AS o_DayForfait,
				o_costs.DayIntegration AS o_DayIntegration,
				-- CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayPrice END AS o_DayPrice,
				-- CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayForfait END AS o_DayForfait,
				-- CASE WHEN k.ExtraType = 1 THEN 0 ELSE o_costs.DayIntegration END AS o_DayIntegration,

				-- riporta i valori di  costo minimo e massimo
				-- use coalesce to normalioze values
				costs.MinDayCost AS MinCost,
				costs.MaxDayCost AS MaxCost,
				o_costs.MinDayCost AS o_MinCost,
				o_costs.MaxDayCost AS o_MaxCost

			FROM CTE_Days d
			INNER JOIN CTE_Km k
				ON d.RunVariationId = k.RunVariationId
			INNER JOIN CTE_CarCosts costs
				ON d.RunCarId = costs.RunCarId
				AND d.Day BETWEEN costs.StartDate AND costs.EndDate
			INNER JOIN CTE_CarCosts o_costs
				ON d.OriginalRunCarId = o_costs.RunCarId
				AND d.Day BETWEEN o_costs.StartDate AND o_costs.EndDate
	), CTE_Final_Costs_Tot AS
	(
		SELECT fc.RunId,
				fc.CarNum,
				fc.Day,
				fc.RunVariationId,
				fc.RunCarId,
				fc.OriginalRunCarId,
			
				fc.Km,
				fc.KmBase,
				fc.KmContract,
				fc.KmExtra,
				fc.ExtraType,	-- 1 = Extra captioato totale
								-- 2 = Extra captiolato parziale
								-- 0 = NON extra captialto
				fc.KmPrice,
				fc.KmPriceExtra,
				fc.DayPrice,
				fc.DayForfait,
				fc.DayIntegration,
				fc.KmPrice + fc.KmPriceExtra +fc.DayPrice + COALESCE(fc.DayForfait, 0) +  COALESCE(fc.DayIntegration, 0) AS c_DayTot,

				fc.o_KmPrice,
				fc.o_KmPriceExtra,
				fc.o_DayPrice,
				fc.o_DayForfait,
				fc.o_DayIntegration,
				fc.o_KmPrice + fc.o_KmPriceExtra +fc.o_DayPrice + COALESCE(fc.o_DayForfait, 0) +  COALESCE(fc.o_DayIntegration, 0) AS o_DayTot,
				
				-- normalizza i valori minimo e massimo
				CASE WHEN fc.MinCost < 0 THEN 0 ELSE fc.MinCost END  AS MinCost,
				CASE WHEN fc. MaxCost <= 0 THEN 1000000000 ELSE fc.MaxCost END AS MaxCost,
				CASE WHEN fc.o_MinCost < 0 THEN 0 ELSE fc.o_MinCost END  AS o_MinCost,
				CASE WHEN fc. o_MaxCost <= 0 THEN 1000000000 ELSE fc.o_MaxCost END AS o_MaxCost

			FROM CTE_Final_Costs fc
	)
	INSERT @Tbl_Costs(
			RunId, CarNum, Day,
			RunVariationId, RunCarId, OriginalRunCarId, 
			Km, KmBase, KmContract, KmExtra, ExtraType,
			KmPrice, KmPriceExtra, DayPrice, DayForfait, DayIntegration,
			o_KmPrice, o_KmPriceExtra, o_DayPrice, o_DayForfait, o_DayIntegration
			)
		SELECT fc.RunId,
				fc.CarNum,
				fc.Day,
				fc.RunVariationId,
				fc.RunCarId,
				fc.OriginalRunCarId,
			
				fc.Km,
				fc.KmBase,
				fc.KmContract,
				fc.KmExtra,
				fc.ExtraType,	-- 1 = Extra captioato totale
								-- 2 = Extra captiolato parziale
								-- 0 = NON extra captialto
				fc.KmPrice,
				fc.KmPriceExtra,
				fc.DayPrice,
				fc.DayForfait,
				(CASE WHEN fc.c_DayTot < fc.MinCost
						THEN COALESCE(fc.DayIntegration, 0) + (fc.MinCost - fc.c_DayTot)
					WHEN fc.c_DayTot > fc.MaxCost
						THEN COALESCE(fc.DayIntegration, 0) + (fc.MaxCost - fc.c_DayTot)
					ELSE fc.DayIntegration
				END) AS DayIntegration,

				fc.o_KmPrice,
				fc.o_KmPriceExtra,
				fc.o_DayPrice,
				fc.o_DayForfait,
				(CASE WHEN fc.o_DayTot < fc.o_MinCost
						THEN COALESCE(fc.o_DayIntegration, 0) + (fc.o_MinCost - fc.o_DayTot)
					WHEN fc.o_DayTot > fc.o_MaxCost
						THEN COALESCE(fc.o_DayIntegration, 0) + (fc.o_MaxCost - fc.o_DayTot)
					ELSE fc.o_DayIntegration
				END) AS o_DayIntegration

			FROM CTE_Final_Costs_Tot fc
			;
	RETURN;
END