/* ******************************************************************
	Maurizio Battisti
	30/09/2023
	restituisce la lista delle simulaizoni in archivio
****************************************************************** */
CREATE VIEW [dbo].[vw_SimulationNames]
AS
SELECT DISTINCT rcc.SimulationName
	FROM dbo.RunCarCosts rcc
	WHERE rcc.SimulationName IS NOT NULL;
