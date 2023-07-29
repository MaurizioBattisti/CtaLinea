/* ***************************************************************************
	Maurizio Battisti
	15/05/2023
	Modifica i dati di un utente
*************************************************************************** */
CREATE PROCEDURE [dbo].[up_User_Edit]
	@UserName			varchar(128),
	@Description		varchar(1024) = NULL,
	@Email				varchar(1024) = NULL,
	@Expiration			date = NULL,
	@MustChangePassword	bit = 1,
	@AssociateId		uniqueidentifier = NULL,
	@Roles				varchar(MAX) = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	DECLARE @Tbl_Roles AS TABLE 
	(
		RoleId		varchar(32) PRIMARY KEY
	);
	IF @Roles IS NOT NULL 
	BEGIN
		INSERT INTO @Tbl_Roles (RoleId)
		SELECT DISTINCT TRIM(value) FROM STRING_SPLIT(@Roles, ',');
	END;
	ELSE
	BEGIN
		-- solleva un errore
		RAISERROR ('Non sono stati indicati i ruoli dell''utente', 16, 1);
		RETURN -1;
	END;

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
		-- la modifica coinvolge l'ultimo amministraotre attivo
		IF NOT EXISTS(SELECT 1 FROM @Tbl_Roles WHERE RoleId = 'ADMIN')
		BEGIN
			RAISERROR ('Non si può eliminare il ruolo di admin dall''unico amministartore attivo', 16, 1);
		END;
		IF @Expiration IS NOT NULL
		BEGIN
			RAISERROR ('Non è possibile impostare una scadenza per l''ultimo amministrtatore attivo', 16, 1);
		END;
	END;

	BEGIN TRAN;
	UPDATE [dbo].[Meta_Users]
		SET Description =@Description,
			Email =@Email, 
			Expiration = @Expiration,
			MustChangePassword = @MustChangePassword, 
			AssociateId =@AssociateId
		WHERE UserName = @UserName;

	-- prima di inserirre i nuovi roli elimina i vecchi
	DELETE FROM [dbo].[Meta_Roles] WHERE UserName = @UserName;
	INSERT INTO [dbo].[Meta_Roles]
		([UserName], [RoleId])
		SELECT  @UserName, r.RoleId
			FROM @Tbl_Roles r;
	COMMIT;
	
	RETURN 0;
END
