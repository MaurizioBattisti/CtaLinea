/* **************************************************************
	Maurizio Battisti
	30/07/2023
	Decodifica i valori di sessione
************************************************************** */
CREATE FUNCTION [dbo].[tvf_Session_DecodeInfo]()
RETURNS @Tbl_Args TABLE
(
	Id		int IDENTITY,
	value	varchar(MAX)
)
AS
BEGIN
	DECLARE @Data varchar(MAX);
	SET @Data = CAST(CONTEXT_INFO() AS varchar(MAX));
	SET @Data = COALESCE(@Data, ';;;');

	INSERT INTO @Tbl_Args (value)
	SELECT value
		FROM STRING_SPLIT(@Data, ';')

	RETURN;
END
