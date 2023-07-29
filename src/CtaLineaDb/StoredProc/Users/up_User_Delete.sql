/* ***************************************************************************
	Maurizio Battisti
	15/05/2023
	Elimina un utente
*************************************************************************** */
CREATE PROCEDURE [dbo].[up_User_Delete]
	@UserName		varchar(128)
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	DECLARE @IsADmin		bit = 0;
	DECLARE @OtherAdmins	bit = 0;
	-- verifica se l'utente è un amministratore
	SELECT @IsADmin = 1 
		FROM [dbo].[Meta_Users] u
		INNER JOIN [dbo].[Meta_Roles] r
			ON u.UserName = r.UserName
		WHERE r.RoleId = 'USERS'
			AND u.Interactive = 1
			AND u.Expiration IS NULL
			AND u.UserName = @UserName
		;

	-- controlla se ci sono altir amministrotri attivi
	SELECT @OtherAdmins = 1
		FROM [dbo].[Meta_Users] u
		INNER JOIN [dbo].[Meta_Roles] r
			ON u.UserName = r.UserName
		WHERE u.UserName <> @UserName
			AND r.RoleId = 'USERS'
			AND u.Interactive = 1
			AND u.Expiration IS NULL
		;
	
	IF @IsADmin = 1
		AND @OtherAdmins = 0
	BEGIN
		RAISERROR ('Non è possibile eliminare l''ultimo amministrtatore attivo', 16, 1);
		RETURN -1;
	END;

	DELETE FROM [dbo].[Meta_Users] WHERE UserName = @UserName;

	RETURN 0;
END
