/* ***********************************************************************
	Maurizio Battisti
	31/10/2023
	Cra la vista per visualizzare la lista dei budget salvati
*********************************************************************** */
CREATE VIEW [dbo].[vw_Budget]
AS
SELECT b.*,
		COALESCE(c.ContractName, 'Qualsiasi') AS ContractDescr,
		COALESCE(a.Description, 'Qualsiasi') AS AssociateDescr,	
		COALESCE(cc.Description, 'Qualsiasi') AS CarDescr,
		COALESCE(CAST(r.CtaRunId AS VARCHAR(MAX)), 'Tutte') AS CtaRunId
	FROM dbo.Budgets b
	LEFT JOIN dbo.Contracts c
		ON b.ContractId = c.ContractId
	LEFT  JOIN Dbo.Associates a
		ON a.AssociateId = b.AssociateId
	LEFT JOIN dbo.Cars cc
		ON b.CarId = cc.CarId
	LEFT JOIN dbo.Runs r
		ON b.RunId = r.RunId
		;

