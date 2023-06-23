/* ***************************************************************************************
	Maurizio Battisti
	18/06/2023
	Restituisce una lista con una riga per ogni corsa con tutte le sue incongreunze desritte
*************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetRunIncongruence]
	@RunId			uniqueidentifier = NULL,
	@StartDate		date = NULL,
	@EndDAte		date = NULL,
	@WhatToCheck	varchar(MAX) = NULL
AS
BEGIN
	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, @RunId;

	WITH CTE_Inc AS
	(
		SELECT d.RunId,
			STRING_AGG(d.IncongreunceDescr, '; ') AS IncongreunceDescr
			FROM [dbo].[tvf_RunIncongruence](@RunId, @StartDate, @EndDAte, @WhatToCheck) d
			GROUP BY d.RunId
	)
	SELECT r.*,
			inc.IncongreunceDescr
		FROM CTE_Inc inc
		INNER JOIN [dbo].[vw_Runs] r
			ON inc.RunId = r.RunId;

	RETURN 0;
END
