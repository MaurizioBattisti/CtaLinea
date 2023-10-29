/* ********************************************************************
	Maurizio Battisti
	07/10/2023
	imposta i costi di simulaizone
******************************************************************** */
CREATE PROCEDURE [dbo].[up_Sim_SetCosts]
	@SimulationName		varchar(50),
	@ContractId			int,
	@StartDate			Date,
	@EndDate			date,

	-- sistema di filtri
	@MinCapacity		int = NULL,
	@MaxCapacity		int = NULL,

	@Sim_CarMAtchId		uniqueidentifier = NULL,

	-- valori per il prezzo al km
	@BasePriceLimit		money = NULL,
	@BasePriceNewVAl	money = NULL,
	@BasePriceAddVal	money = NULL,
	@BasePriceFacto		float = NULL,
	-- valori per il prezzo al km extra
	@ExtraPriceLimit	money = NULL,
	@ExtraPriceNewVAl	money = NULL,
	@ExtraPriceAddVal	money = NULL,
	@ExtraPriceFacto	float = NULL,
	
	-- valori predefiniti per il costo al chilometro
	@DefKmPrice			money,
	@DefKmExtraPrice	money,

	@Overwrite			bit = 0
AS
bEGIN
	DECLARE @Tbl_ActOn AS TABLE
	(
		[RunId]				UNIQUEIDENTIFIER NOT NULL, 
		[RunPeriodId]		UNIQUEIDENTIFIER NOT NULL, 
		[RunCarId]			UNIQUEIDENTIFIER NOT NULL, 
		[RunCarCostId]		UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 

		[DayPrice]			MONEY NOT NULL DEFAULT 0 , 
		[KmPrice]			MONEY NOT NULL DEFAULT 0 , 
		[KmPriceExtra]		MONEY NOT NULL DEFAULT 0 , 

		[DayIntegration]	MONEY NULL , 
		[DayForfait]		MONEY NULL, 
    
		[SimulationName]	VARCHAR(50) NULL, 

		[Simulated]			bit DEFAULT 0
	);

	IF @DefKmPrice IS NULL OR @DefKmPrice < 0 SET @DefKmPrice = 0;
	IF @DefKmExtraPrice IS NULL OR @DefKmExtraPrice < 0 SET @DefKmExtraPrice = 0;

	-- crea la platea di elementi su cui lavorare
	INSERT INTO @Tbl_ActOn
	SELECT data.RunId, data.RunPeriodId, data.RunCarId,
			COALESCE(data.RunCarCostId, NEWID()),
			data.DayPrice, data.KmPrice, data.KmPriceExtra,
			data.DayIntegration, data.DayForfait,
			data.SimulationName, data.Simulated
		FROM [dbo].[tvf_Sim_Base] (@SimulationName, @ContractId, @StartDate, @EndDate) AS data
		INNER JOIN dbo.Runs r
			ON r.RunId = data.RunId
		INNER JOIN dbo.vw_RunVariations v
			ON r.RunId = v.RunId
			AND v.VariationStartDate IS NULL
		WHERE (@MinCapacity IS NULL
				OR v.RequestedCapacity  >= @MinCapacity)
			AND (@MaxCapacity IS NULL
				OR v.RequestedCapacity  <= @MaxCapacity)

	IF @Sim_CarMAtchId IS NOT NULL
	BEGIN
		DELETE d FROM @Tbl_ActOn d
			INNER JOIN dbo.RunCars c
				ON d.RunCarId = c.RunCarId
			LEFT JOIN dbo.SimCarMatches l
				ON l.CarId = c.CarId
				AND l.Sim_CarMAtchId = @Sim_CarMAtchId
			WHERE l.CarId IS NULL;

		-- elimina i dati del match
		DELETE FROM dbo.SimCarMatches
			WHERE Sim_CarMAtchId = @Sim_CarMAtchId;
	END

	-- applica le variaizoni alla platea
	UPDATE @Tbl_ActOn
		SET KmPrice = CASE 
						WHEN @BasePriceLimit IS NULL
							THEN KmPrice 
						WHEN KmPrice < @BasePriceLimit 
							THEN COALESCE(@BasePriceNewVAl, KmPrice + @BasePriceAddVal, KmPrice *@BasePriceFacto, KmPrice) 
						ELSE KmPrice 
					END,
			KmPriceExtra = CASE 
						WHEN @ExtraPriceLimit IS NULL	
							THEN KmPriceExtra
						WHEN KmPriceExtra < @ExtraPriceLimit 
							THEN COALESCE(@ExtraPriceNewVal, KmPriceExtra + @ExtraPriceAddVal, KmPriceExtra *@ExtraPriceFacto, KmPriceExtra) 
						ELSE KmPriceExtra 
					END
		;

	-- assegna un valore di default aqueli che non ce l'hanno
	UPDATE @Tbl_ActOn 
		SET KmPrice = CASE WHEN KmPrice <= 0 OR @Overwrite = 1 THEN @DefKmPrice ELSE KmPrice END,
			KmPriceExtra = CASE WHEN KmPriceExtra <= 0 OR @Overwrite = 1 THEN @DefKmExtraPrice ELSE KmPriceExtra END
		;

	-- esegue il merge per assegnare i valri
	MERGE dbo.RunCarCosts AS t
		USING  @Tbl_ActOn AS s
		ON  t.RunCarCostId = s.RunCarCostId
	WHEN NOT MATCHED  THEN
		INSERT (RunCarCostId, RunCarId, StartDate, KmPrice, KmPriceExtra, DayPrice, DayForfait, DayIntegration, SimulationName)
		VALUES (s.RunCarCostId, s.RunCarId, NULL, s.KmPrice, s.KmPriceExtra, s.DayPrice, s.DayForfait, s.DayIntegration, s.SimulationName)
	WHEN MATCHED  THEN
		UPDATE 
			SET t.KmPrice =  s.KmPrice, 
				t.KmPriceExtra = s.KmPriceExtra, 
				t.DayPrice = s.DayPrice, 
				t.DayForfait = s.DayForfait, 
				t.DayIntegration = s.DayIntegration
		;


	RETURN 0;
END
