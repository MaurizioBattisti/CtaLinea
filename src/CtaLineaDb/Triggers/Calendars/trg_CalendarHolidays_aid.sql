/* **************************************************************
	Maurizio Battisti
	04/02/2023
	trigger scatenato dopo la modifcia di un calendario
	segna tutte le crose implicate come da ricalcolrre
************************************************************** */
CREATE TRIGGER [trg_CalendarHolidays_aid]
	ON [dbo].[CalendarHolidays]
	AFTER INSERT, DELETE
AS
BEGIN
	DECLARE @Id int;
	SET NOCOUNT ON
	DECLARE cal_curr CURSOR LOCAL FORWARD_ONLY 
		FOR (( SELECT i.CalendarId FROM inserted i)
			UNION 
			(SELECT d.CalendarId FROM deleted d))
		;

	OPEN cal_curr;
	FETCH NEXT FROM cal_curr INTO @Id; 
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [dbo].[uo_RunsNeedRecalc] @CalendarId= @Id;
		FETCH NEXT FROM cal_curr INTO @Id; 
	END;

	CLOSE cal_curr;  
	DEALLOCATE cal_curr;  
END
