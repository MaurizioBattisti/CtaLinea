/* *************************************************************************
	Maurizio Battisti
	14/10/2023
	Rende effettiva una simulazioen a  aprtire da una data
************************************************************************* */
CREATE PROCEDURE [dbo].[up_Simulation_Finalize]
	@SimulationName		varchar(50),
	@StartDate			date = NULL,
	@Overwrite			bit = 0
AS
BEGIN
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( GetDATE() );

	DECLARE @Tbl_Temp AS TABLE
	(
		RunCarId		uniqueidentifier,
		RunCarCostId	uniqueidentifier,
		SimulationName	varchar(50),
		StartDate		Date
	);

	BEGIN TRAN;

	INSERT INTO @Tbl_Temp
		(RunCarId, RunCarCostId,
		StartDate, SimulationName)
		SELECT rcc.RunCarId,
				rcc.RunCarCostId,
				@StartDate, rcc.SimulationName
			FROM dbo.RunCarCosts rcc
			WHERE rcc.SimulationName = @SimulationName;

	IF @Overwrite = 0
	BEGIN
		DECLARE @Count		int = 1;
		WHILE 1 = 1
		BEGIN
			SELECT @Count = COUNT(*)
				FROM @Tbl_Temp t
				INNER JOIN dbo.RunCarCosts rcc
					ON rcc.RunCarId = t.RunCarId
					AND rcc.SimulationName IS NULL
					AND rcc.StartDAte = t.StartDate
		
			IF @Count =  0 BREAK;

			UPDATE @Tbl_Temp 
				SET StartDate = DATEADD(Day, 1, t.StartDate)
				FROM @Tbl_Temp t
				INNER JOIN dbo.RunCarCosts rcc
					ON rcc.RunCarId = t.RunCarId
					AND rcc.SimulationName IS NULL
					AND rcc.StartDAte = t.StartDate
		END
	END
	ELSE
	BEGIN
		-- se deve sovrascrivere, in realtà elimina tutti i dati che verranno sovrascritti
		DELETE rcc 
			FROM dbo.RunCarCosts rcc
			INNER JOIN @Tbl_Temp t
				ON rcc.RunCarId = t.RunCarId
				AND rcc.SimulationName IS NULL
				AND rcc.StartDAte = t.StartDate
	END
	
	UPDATE dbo.RunCarCosts 
		SET StartDAte = t.StartDate,
			SimulationName = NULL
		FROM dbo.RunCarCosts rcc
		INNER JOIN @Tbl_Temp t
			ON t.RunCarCostId = rcc.RunCarCostId;

	COMMIT;

	RETURN 0;
END
