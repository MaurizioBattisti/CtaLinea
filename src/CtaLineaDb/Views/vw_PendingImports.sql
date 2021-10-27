/* ********************************************************************************************
*	Aithor			Maurizio Battisti
*	Date			20/05/2021
*	Description		Mostra le ultime importazioni pendenti  divise per tipo
******************************************************************************************** */
CREATE VIEW [dbo].[vw_PendingImports]
	AS 
WITH CTE_Imports AS (
	SELECT ROW_NUMBER() OVER (PARTITION BY i.ImportDescr ORDER BY i.LastUpdateDate DESC) AS Number,
			i.*
	FROM dbo.Imports AS i
	WHERE i.ImportStatus IN ('PROGRESS')
		AND i.LastUpdateDate > DATEADD(SECOND, -10, SYSDATETIME())
)
SELECT i.*
	FROM CTE_Imports AS i
	WHERE i.Number = 1;

