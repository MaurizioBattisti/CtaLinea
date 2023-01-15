/* ***********************************************************************************************
	Maurizio Battisti
	14/01/2023
	Ricalcola i giorni delle corse indicate nella tabella dei ricalcoli
*********************************************************************************************** */
CREATE PROCEDURE [dbo].[uo_RecalcRunDays_Massive]
(
	@ProcessCount		int = 0
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RecCount int = 1;
	DECLARE @RunId	uniqueidentifier;
	SET @ProcessCount = COALESCE(@ProcessCount, 0);

	WHILE ( @ProcessCount = 0 
		OR  @RecCount <= @ProcessCount)
	BEGIN
		SET @RunId = NULL;
		SELECT TOP 1 @RunId = RunId FROM dbo.Runs_NeedsDayRecalc;
		IF @RunId IS NULL BREAK;

		-- esegue il ricalcolo
		EXEC [dbo].[uo_RecalcRunDays] @RunId;
    	PRINT @RecCount;

		DELETE FROM dbo.Runs_NeedsDayRecalc WHERE RunId = @RunId;
		SET @RecCount  = @RecCount  + 1
	END

	RETURN 0;
END