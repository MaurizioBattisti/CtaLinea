/* ******************************************************************
	Maurizio Battisti
	29/09/2023
	aggiunge i giorni alla tabella dei gironi elastibus
****************************************************************** */
CREATE PROCEDURE [dbo].[up_AddElastibusDay]
	@RunCtaId		INT,
	@Date			DATE,
	@Km				REAL,
	@PeopleCount	INT
AS
bEGIN
	MERGE dbo.RunElastibusDays AS t
	USING (
		SELECT r.RunId ,
				@Date AS Day, @Km AS Km, @PeopleCount AS PeopleCount
			FROM dbo.Runs r
			WHERE r.CtaRunId = @RunCtaId
				AND r.Elastibus = 1
	) AS s
	ON  t.RunId = s.RunId
	AND  t.Day = s.Day
	WHEN NOT MATCHED  THEN
		INSERT (RunId, Day, Km, PeopleCount)
		VALUES (s.RunId, s.Day, s.Km, s.PeopleCount)
	WHEN MATCHED THEN 
		UPDATE 
			SET Km = s.Km,
			PeopleCount = s.PeopleCount
	;
	RETURN;	
END
