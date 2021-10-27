/* *******************************************************************
*	Aithor			Maurizio Battisti
*	Date			20/05/2021
*	Description:	Finalizza i dati appena importati crando le righe 
					nelle tabelle di compentenza di cTA con i dati di tT
******************************************************************* */
CREATE PROCEDURE [dbo].[up_FinalizeServiceImport]
	@ImportId	uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	/*
	DECLARE @Tbl_All AS TABLE (
		ServiceId	int PRIMARY KEY
	);
	-- tutti i servizi importati sono  nella tabella del dettaglio importazione
	INSERT INTO @Tbl_All (ServiceId)
		SELECT ServiceId 
			FROM dbo.ImportDetails 
			WHERE ImportId = @ImportId;
	*/

	DECLARE @Tbl_NewSvc AS TABLE (
		ServiceId	int PRIMARY KEY
	);
	-- i servizi nuovi sono quellic he tra tutti non esistono nella tabella del CTA
	INSERT INTO @Tbl_NewSvc (ServiceId)
		SELECT i.ServiceId 
			FROM dbo.ImportDetails i
			LEFT JOIN CtaServices s
				ON i.ServiceId = s.ServiceId
			WHERE i.ImportId = @ImportId;

	DECLARE @Tbl_ChangedSvc AS TABLE (
		ServiceId	int PRIMARY KEY
	);
	-- i servizi modificati sono quelli che sono presenti da entrambe le parti
	-- TODO: escludere dai modificati qeulli segnati come eliminati
	INSERT INTO @Tbl_ChangedSvc (ServiceId)
		SELECT i.ServiceId 
			FROM dbo.ImportDetails i
			INNER JOIN  CtaServices s
				ON i.ServiceId = s.ServiceId
			WHERE i.ImportId = @ImportId;

	/*
	-- Servizi Eliminati
	DECLARE @Tbl_Deleted AS TABLE (
		ServiceId	int PRIMARY KEY
	);
	*/

	-- nuovi Servizi
	BEGIN
		-- aggiunge una riga in CTA Services per tutti i nuovi servizi
		INSERT INTO dbo.CtaServices
			(ServiceId, RunTypeId)
			SELECT s.ServiceId,
					'TOASS'AS RunTypeId
				FROM @Tbl_NewSvc AS s;

		-- tenta di creare i dati del servizio CTA a partire da quelli della TT
		-- TODO: 

		-- aggiunge una riga alla tabella delle modifiche
		INSERT INTO dbo.CtaServiceLog
			(ServiceId, [USer], Note)
			SELECT s.ServiceId,
					i.[User],
					'Nuova importazione riga' AS Note
				FROM @Tbl_NewSvc AS s
				INNER JOIN dbo.ImportDetails AS id
					ON s.ServiceId = id.ServiceId
				INNER JOIN dbo.Imports AS i
					ON id.ImportId = i.Id;
	END;
	
	-- SERvizi modificati
	BEGIN
		-- Applica le modifiche ai servizi CTA
		-- TODO

		-- Applica le mdoifche al periodo
		--TODO

		-- Inserisce i valori di modifica nella tabella del log 
		-- aggiunge una riga alla tabella delle modifiche
		INSERT INTO dbo.CtaServiceLog
			(ServiceId, [USer], Note)
			SELECT s.ServiceId,
					i.[User],
					'Modifica in fase di importazione' AS Note
				FROM @Tbl_ChangedSvc AS s
				INNER JOIN dbo.ImportDetails AS id
					ON s.ServiceId = id.ServiceId
				INNER JOIN dbo.Imports AS i
					ON id.ImportId = i.Id;
	END;

	-- servizi Eliminati
	/*
	BEGIN 
	END;
	*/

	RETURN 0;
END;
