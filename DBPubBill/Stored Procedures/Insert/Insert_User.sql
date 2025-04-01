CREATE PROCEDURE [dbo].[Insert_User]
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
	INSERT INTO [dbo].[User] (Name, Password, LocationId, Status, Bill, KOT, Admin)
	VALUES (@Name, @Password, @LocationId, @Status, @Bill, @KOT, @Admin)
END;