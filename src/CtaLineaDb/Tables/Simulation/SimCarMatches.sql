/* ********************************************************************
	Maurizio Battisti
	07/10/2023
	Tabella che contiene i mezzi da considerare per
	l'assegnazione delle tariffe durante la simulaizone
******************************************************************** */
CREATE TABLE [dbo].[SimCarMatches]
(
	[Sim_CarMAtchId] UNIQUEIDENTIFIER NOT NULL , 
    [CarId] UNIQUEIDENTIFIER NOT NULL, 

    PRIMARY KEY ([Sim_CarMAtchId], [CarId])
)
