/* *************************************************************************************************
	Maurizio Battisti
	21/01/2024
	Lista delle varianti divisa per giorno e 
 **************************************************************************************************/
CREATE VIEW [dbo].[Vw_RunVariants_ByDay]
AS
	WITH CTE_Vars AS
	(
		SELECT v.RunId,
				v.RunVariationId,
				COALESCE(v.StartDate, '19000101') AS StartDate,
				v.LineNumber,
				v.RunNumber,
				v.RequestedFrequency,
				v.Path,
				v.RequestedCapacity,
				v.Km,
				v.Note,
				v.StartTime,
				v.EndTime,
				v.Monday,
				v.Tuesday,
				v.Wednesday,
				v.Thursday,
				v.Friday,
				v.Saturday,
				v.Sunday,
				ROW_NUMBER() OVER (PARTITION BY v.RunId ORDER BY v.StartDate ASC) AS Number
			FROM dbo.RunVariations v
	), CTE_Var2 AS
	(
		SELECT v1.*,
				CASE WHEN v2.StartDate IS NULL
					THEN '99991231'
					ELSE DATEADD(dAY, -1, v2.StartDate ) 
				END AS EndDAte
			FROM CTE_Vars v1
			LEFT JOIN CTE_Vars v2
				ON v1.RunId = v2.RunId
				AND v1.Number = v2.Number -1
	)
	SELECT 	v.*,
			d.Day
		FROM dbo.RunDays d
		INNER JOIN CTE_Var2 v
			ON d.RunId = v.RunId
			AND d.Day BETWEEN v.StartDate AND V.EndDAte;
