CREATE PROCEDURE [dbo].[Insert_User]
	@Id INT,
	@Name VARCHAR(100),
	@Password SMALLINT,
	@LocationId INT,
	@Status BIT,
	@Bill BIT,
	@KOT BIT,
	@Inventory BIT,
	@Admin BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[User] (Name, Password, LocationId, Status, Bill, KOT, Inventory, Admin)
		VALUES (@Name, @Password, @LocationId, @Status, @Bill, @KOT, @Inventory, @Admin);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[User]
		SET Name = @Name,
			Password = @Password,
			LocationId = @LocationId,
			Status = @Status,
			Bill = @Bill,
			KOT = @KOT,
			Inventory = @Inventory,
			Admin = @Admin
		WHERE Id = @Id;
	END
END;