/* *******************************************************************
	Maurizio Battisti
	24/01/2023
	Aggiunge le corse richieste alla lista di qeulle che necessitano un ricalcolo
******************************************************************* */
CREATE PROCEDURE [dbo].[uo_RunsNeedRecalc]
	@RunId			uniqueidentifier = NULL,
	@CalendarId		int = NULL,
	@ContractId		int = NULL,
	@TagId			int = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	-- verifica che i parametri siano tutti a null
	-- se sono tutti null inserisce tutte le  corse nella tabella delle necessità di ricalcoo
	if @RunId IS NULL
		AND @CalendarId IS NULL
		AND @ContractId IS NULL
		AND @TagId IS NULL
	BEGIN
		WITH CTE_UniqueRuns AS
		(
			SELECT DISTINCT r.RunId
				FROM dbo.Runs  r
				LEFT JOIN dbo.Runs_NeedsDayRecalc n
					ON r.RunId = n.RunId
				WHERE n.RunId IS NULL
		)
		INSERT INTO dbo.Runs_NeedsDayRecalc
			(RunId)
			SELECT r.RunId
				FROM CTE_UniqueRuns r;
		
	END
	ELSE
	BEGIN
		WITH CTE_Runs AS
		(
			-- uion di tutti i dati
			(
				SELECT r.RunId
					FROM dbo.Runs  r
					WHERE r.RunId = @RunId
			) UNION (
				SELECT v.RunId 
					FROM dbo.RunVariations v
					INNER JOIN dbo.RunVariationCalendars vc
						ON  v.RunVariationId = vc.RunVariationId
					INNER JOIN [dbo].[tvf_CalendarTreeTopDown](@CalendarId) c
						ON vc.CalendarId = c.CalendarId
			) UNION (
				SELECT r.RunId
					FROM dbo.Runs  r
					WHERE r.ContractId = @ContractId
			) UNION (
				SELECT t.RunId
					FROM dbo.RunTags t
					WHERE t.TagId = @TagId
			)
		), CTE_UniqueRuns AS
		(
			SELECT DISTINCT r.RunId
				FROM CTE_Runs  r
				LEFT JOIN dbo.Runs_NeedsDayRecalc n
					ON r.RunId = n.RunId
				WHERE n.RunId IS NULL
		)
		INSERT INTO dbo.Runs_NeedsDayRecalc
			(RunId)
			SELECT r.RunId
				FROM CTE_UniqueRuns r;
	END

	RETURN 0;
END
