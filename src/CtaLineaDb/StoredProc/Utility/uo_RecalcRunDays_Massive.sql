/* ***********************************************************************************************
	Maurizio Battisti
	14/01/2023
	Ricalcola i giorni delle corse indicate nella tabella dei ricalcoli
*********************************************************************************************** */
CREATE PROCEDURE [dbo].[uo_RecalcRunDays_Massive]
(
	@ProcessCount		int = 0,
	@RunId				uniqueidentifier = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RecCount int = 1;
	DECLARE @The_RunId	uniqueidentifier;
	SET @ProcessCount = COALESCE(@ProcessCount, 0);

	IF @RunId IS NOT NULL 
	BEGIN
		SET @ProcessCount = 1;
	END;

	WHILE ( @ProcessCount = 0 
		OR  @RecCount <= @ProcessCount)
	BEGIN
		SET @The_RunId = NULL;
		SELECT TOP 1 @The_RunId = RunId FROM dbo.Runs_NeedsDayRecalc WHERE (@RunId IS NULL OR RunId = @RunId);
		IF @The_RunId IS NULL BREAK;

		-- esegue il ricalcolo
		EXEC [dbo].[uo_RecalcRunDays] @The_RunId;
    	PRINT @RecCount;

		DELETE FROM dbo.Runs_NeedsDayRecalc WHERE RunId = @The_RunId;
		SET @RecCount  = @RecCount  + 1
	END

	RETURN 0;
END