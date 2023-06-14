/* *************************************************************************
	Maurizio Battisti
	27/05/2023
	creata al vista che restituisce tuti i punti di raccolta con una informaizone in più
	il numero di nodi al quale il collection point è colelgato
************************************************************************** */
CREATE VIEW [dbo].[vw_CollectionPoints]
AS 
WITH CTE_Nods AS
(
	SELECT n.CollectionPointId,
			cOUNT(*) As NodeCount
		FROM dbo.RunNodes n
		GROUP BY n.CollectionPointId
)
SELECT cp.CollectionPointId,
		cp.Description,
		cp.Address,
		cp.City,
		cp.ZipCode,
		cp.CollectionPointType,
		cp.Longitude,
		cp.Latitude,
		n.NodeCount
	FROM dbo.CollectionPoints cp
	LEFT JOIN CTE_Nods n
		ON cp.CollectionPointId = n.CollectionPointId
