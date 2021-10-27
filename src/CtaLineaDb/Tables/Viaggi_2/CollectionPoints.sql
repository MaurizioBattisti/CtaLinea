/* ************************************************************************************************************
	TT chiama questi dati Nodi
************************************************************************************************************ */
CREATE TABLE [dbo].[CollectionPoints]
(
	[CollectionPointId] VARCHAR(20) NOT NULL PRIMARY KEY, 
    [Description] VARCHAR(200) NULL, 
    [Address] VARCHAR(200) NULL, 
    [City] VARCHAR(200) NULL, 
    [ZipCode] VARCHAR(10) NULL, 
    [Latitude] DECIMAL(14, 10) NULL, 
    [Longitude] DECIMAL(14, 10) NULL, 
    [CollectionPointType] VARCHAR(20) NULL
)
