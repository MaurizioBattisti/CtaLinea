/* ****************************************************************************
	Maurizio Battisti
	27/05/2023
	restituisce una lista di corese con una indicazione sul nodo in coincidenza
**************************************************************************** */
CREATE FUNCTION [dbo].[tvf_oincidenceNodes]
(
	@CollectionPointId	varchar(20) NULL
)
RETURNS @Tbl_Result TABLE
(
	RunId				uniqueidentifier NOT NULL PRIMARY KEY,
	CoincidenceState	Varchar(MAX)
)
AS
BEGIN
	WITH CTE_VarNodes AS
	(
		SELECT v.RunId,
				(CASE WHEN n.CoincidenceDescr IS NOT NULL AND n.CollectionPointId = @CollectionPointId THEN 2
					WHEN n.CoincidenceDescr IS NOT NULL THEN 1
					ELSE NULL
				END) AS CoincidenceStatus
			FROM Dbo.RunNodes n
			INNER JOIN dbo.RunVariations v
				ON n.RunVariationId = v.RunVariationId
	), CTE_VarNodes_Coinc AS
	(
		SELECT n.RunId,
			MAX(n.CoincidenceStatus) AS CoincidenceStatus
			FROM CTE_VarNodes n
			WHERE n.CoincidenceStatus IS NOT NULL
			GROUP BY n.RunId	
	)
	INSERT @Tbl_Result
		(RunId, CoincidenceState)
		SELECT n.RunId,
			CASE WHEN n.CoincidenceStatus = 1 THEN 'si'
				WHEN n.CoincidenceStatus = 2 THEN 'si, corrispondente'
			END AS Coinc
			FROM CTE_VarNodes_Coinc n;

	RETURN;
END
