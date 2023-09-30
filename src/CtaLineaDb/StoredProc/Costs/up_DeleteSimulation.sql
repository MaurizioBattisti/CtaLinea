/* ******************************************************************
	Maurizio Battisti
	30/09/2023
	Elimina una simulaizone dall'archivio
****************************************************************** */
CREATE PROCEDURE [dbo].[up_DeleteSimulation]
	@SimulationName		varchar(50)
AS
BEGIN
	IF @SimulationName IS NOT NULL 
	BEGIN
		DELETE FROM dbo.RunCarCosts
			WHERE SimulationName = @SimulationName;
	END;
	
	RETURN 0;
END
