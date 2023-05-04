/* ************************************************************************************
	Maurizio Battisti
	02/05/2023
	Lista dei forfatit multi corsa
************************************************************************************ */
CREATE VIEW [dbo].[vw_MultiRunForfaits]
AS 
WITH CTE_Details AS
(
	SELECT mrfd.ForfaitId,
		COUNT(*) AS RunCount
		FROM dbo.MultiRunForfaitDetails mrfd
		GROUP BY mrfd.ForfaitId
)
SELECT f.ForfaitId,
		f.ContractId,

		f.ForfaitName,
		f.[ForfaitType],
		CASE f.[ForfaitType]
			WHEN 'D' THEN 'Giornaliero'
			WHEN 'M' THEN 'Mensile'
			WHEN 'Y' THEN 'Annuale'
			ELSE 'Non definito'
		 END  AS ForfaitTrpeDescr,
		f.Amount,

		c.ContractName,
		COALESCE(d.RunCount, 0) AS RunCount
	FROM dbo.MultiRunForfait f
	INNER JOIN dbo.Contracts c
		ON F.ContractId = c.ContractId
	LEFT JOIN CTE_Details d
		ON D.ForfaitId = f.ForfaitId;
