CREATE PROCEDURE [dbo].[Update_User]
	@Id INT,
	@Name VARCHAR(100),
	@Password SMALLINT,
	@LocationId INT,
	@Status BIT,
	@Bill BIT,
	@KOT BIT,
	@Admin BIT
AS
BEGIN
	UPDATE [dbo].[User]
	SET
		Name = @Name,
		Password = @Password,
		LocationId = @LocationId,
		Status = @Status,
		Bill = @Bill,
		KOT = @KOT,
		Admin = @Admin
	WHERE Id = @Id
END;