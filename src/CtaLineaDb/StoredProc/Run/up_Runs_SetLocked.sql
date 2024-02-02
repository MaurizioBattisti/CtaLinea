/* *********************************************************
	Maurizio Battisti
	30/01/2024
	Imposta po elimina il blocco delle schede
********************************************************* */
CREATE PROCEDURE [dbo].[up_Runs_SetLocked]
	@RunIds		VARCHAR(MAX),
	@RefDate	Date = NULL,
	@Note		VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @TBl AS TABLE (
		RunId		uniqueidentifier PRIMARY KEY,
		RefDate		Date NULL,
		Note		Varchar(MAX) NULL
	);

	INSERT INTO @TBl 
		(RunId, RefDate, Note)
		SELECT DISTINCT v.value ,
				@RefDate, 
				CASE WHEN @RefDate IS NULL THEN NULL ELSE  @Note END AS Note
			FROM STRING_SPLIT(@RunIds, ',') v

	UPDATE dbo.Runs
		SET LockedDate = t.RefDate,
			LockedNote = t.Note
		FROM dbo.Runs r
		INNER JOIN @Tbl t
			ON r.RunId = t.RunId;

	RETURN 0;
END;
