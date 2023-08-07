/* *********************************************************************
	Maurizio Battisti
	30/07/2023
	Restituisce la data corrente
	se il periodo non è quelloa ttivo restituisce la data di fine dei peridoi passati  o al da ta di inizio edi quelli futuri
********************************************************************* */
CREATE FUNCTION [dbo].[fn_Session_GetCurrentDate] ()
RETURNS Date
BEGIN
	DECLARE @CurrentDate	date = GETDATE();
	DECLARE @Start			date;
	DECLARE @End			date;

	SET @Start = dbo.fn_Session_GetPeriodStartDate();
	SET @End = dbo.fn_Session_GetPeriodEndDate();
	
	-- se la data di inzio è nel futuro segna la data di inizio come data corrente
	IF @Start IS NOT NULL AND @Start > @CurrentDate SET @CurrentDate = @Start;
	
	-- se la data di fine è nel passato segna la data di fine come data corrente
	IF @End IS NOT NULL AND @End < @CurrentDate SET @CurrentDate = @End;

	RETURN @CurrentDate;
END