/* ***********************************************************************************************
	Maurizio Battisit
	14/01/2023
	Calcola ua tabella con i costi per ogni corsa
	i corsit NON possono essere sommati perchè nel caso di rimpiazzi deveono essere presi solo quelli del mezzo di interesse
	si deve, cioè fare un DISTINCT o su RunCarId o su OriginalRunCarId
	non si possono lasicare entrambi nella estrazione
*********************************************************************************************** */
CREATE FUNCTION [dbo].[tvf_RunCostDetails]
(
)
RETURNS @Tbl_Costs TABLE
(
	RunId				uniqueidentifier NOT NULL,
	Day					datetime NOT NULL,
	CarNum				int DEFAULT(1),

	RunVariationId		uniqueidentifier NOT NULL,
	RunPeriodId			uniqueidentifier,
	WeekDay				int NOT NULL,
	Suspended			bit NOT NULL DEFAULT 0,
	OutOfPeriod			bit NOT NULL DEFAULT 0,

	RunCarId			uniqueidentifier,
	OriginalRunCarId	uniqueidentifier,
	Replaced			bit NOT NULL DEFAULT(0),

	Km					float NOT NULL DEFAULT(0),
	DayPrice			money NOT NULL  DEFAULT(0),
	KmPrice				money NOT NULL  DEFAULT(0),
	DayForfait			money NOT NULL  DEFAULT(0),
	DayIntegration		money NOT NULL  DEFAULT(0),
	KmCost				money NOT NULL  DEFAULT(0),

	o_DayPrice			money NOT NULL  DEFAULT(0),
	o_KmPrice			money NOT NULL  DEFAULT(0),
	o_DayForfait		money NOT NULL  DEFAULT(0),
	o_DayIntegration	money NOT NULL  DEFAULT(0),
	o_KmCost			money NOT NULL  DEFAULT(0)

	PRIMARY KEY (RunId, Day, CarNum)
)
AS
BEGIN
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
	INSERT @Tbl_Costs
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

	RETURN;
END
