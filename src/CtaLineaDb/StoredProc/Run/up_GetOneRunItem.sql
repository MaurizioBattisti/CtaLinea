/* *****************************************************************************
	Maurizio Battisti
	21/12/2022
	Restituisce una serie di risultati con i  dati di una singola corsa
	crea tutto l'albero dei risultati 
***************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetOneRunItem]
	@RunId		uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	-- tabella corse (run)
	SELECT r.* FROM dbo.Runs r WHERE r.RunId = @RunId;
	-- varianti (RunVariations)
	SELECT v.* FROM dbo.RunVariations v WHERE v.RunId = @RunId;
	-- periodi della corsa (RunPEriods)
	SELECT p.* FROM dbo.RunPeriods p WHERE p.RunId = @RunId;
	-- giorni addizionali
	SELECT r.* FROM dbo.RunAdditionalDays r WHERE r.RunId = @RunId;

	-- calendari delle varianti
	SELECT vc.RunVariationId,
			vc.CalendarId
		FROM dbo.RunVariationCalendars vc
		INNER JOIN dbo.RunVariations v
			ON vc.RunVariationId =v.RunVariationId 
		WHERE v.RunId = @RunId;

	-- nodi delle variatnti
	SELECT n.*,
			cp.Description AS CollectionPointDescription
		FROM dbo.RunVariations v 
		INNER JOIN dbo.RunNodes n
			ON n.RunVariationId =v.RunVariationId 
		INNER JOIN dbo.CollectionPoints cp
			ON cp.CollectionPointId = n.CollectionPointId
		WHERE v.RunId = @RunId;
	-- mezzi 
	SELECT c.* 
		FROM dbo.RunCars c 
		INNER JOIN dbo.RunPeriods p 
			ON c.RunPEriodId = p.[RunPeriodId] 
		WHERE p.RunId = @RunId;
	-- costi dei mezzi
	SELECT cc.* 
		FROM dbo.RunCarCosts cc 
		INNER JOIN dbo.RunCars c 
			ON cc.RunCarId = c.RunCarId
		INNER JOIN dbo.RunPeriods p 
			ON c.RunPEriodId = p.[RunPeriodId] 
		WHERE p.RunId = @RunId;
	
	-- sostituzioni dei mezzi
	SELECT r.*
		FROM dbo.RunCarReplacements r
		INNER JOIN dbo.RunPeriods p
			ON r.RunPeriodId = p.[RunPeriodId]
		WHERE p.RunId = @RunId;
	-- dettaglio sostituzioni
	SELECT d.*
		FROM dbo.RunCarReplacementDetails d
		INNER JOIN dbo.RunCarReplacements r
			ON d.CarReplacementId = r.CarReplacementId
		INNER JOIN dbo.RunPeriods p
			ON r.RunPeriodId = p.[RunPeriodId]
		WHERE p.RunId = @RunId;

	-- Sospensioni (RunSuspensions=
	SELECT s.* FROM dbo.RunSuspensions s WHERE s.RunId = @RunId;

	-- tutti i lockup
	-- contratt
	SELECT DISTINCT c.* 
		FROM dbo.Contracts c
		INNER JOIN dbo.Runs r
			ON r.ContractId = c.ContractId
		WHERE r.RunId = @RunId;

	-- calendari
	/* Non servono più i calenadri
	SELECT DISTINCT c.* 
		FROM dbo.Calendars c
		INNER JOIN dbo.RunVariationCalendars vc
			ON vc.CalendarId = c.CalendarId
		INNER JOIN dbo.RunVariations v
			ON v.RunVariationId = vc.RunVariationId
		WHERE v.RunId = @RunId;
	*/

	-- ditte
	SELECT DISTINCT a.*
		FROM dbo.Associates a
		INNER JOIN dbo.RunCars c 
			ON a.AssociateId = c.AssociateId
		INNER JOIN dbo.RunPeriods p 
			ON c.RunPEriodId = p.[RunPeriodId] 
		WHERE p.RunId = @RunId;
	
	-- mezzi
	SELECT DISTINCT car.*
		FROM dbo.Cars car
		INNER JOIN dbo.RunCars c 
			ON car.CarId = c.CarId
		INNER JOIN dbo.RunPeriods p 
			ON c.RunPEriodId = p.[RunPeriodId] 
		WHERE p.RunId = @RunId;

	RETURN 0;
END

