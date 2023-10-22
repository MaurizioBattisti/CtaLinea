/* *************************************************************************
	Maurizio Battisti
	14/10/2023
	Assegna i costi fissi in modo da poterli azzerare
************************************************************************* */
CREATE PROCEDURE [dbo].[up_SimulationSetDayCosts]
	@SimulationName		varchar(50),
	@DayPrice			money = 0,
	@DayForfait			money = NULL,
	@DayIntegration		money = NULL
AS
BEGIN
	UPDATE dbo.RunCarCosts
		SET DayPrice = COALESCE( @DayPrice, 0),
			DayForfait = @DayForfait,
			DayIntegration = @DayIntegration
		WHERE SimulationName = @SimulationName;
	
	RETURN 0;
END
