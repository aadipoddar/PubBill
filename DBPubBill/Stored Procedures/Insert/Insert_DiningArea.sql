CREATE PROCEDURE [dbo].[Insert_DiningArea]
	@Id INT,
	@Name VARCHAR(50),
	@LocationId INT,
	@Status BIT = 1
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[DiningArea] (Name, LocationId, Status)
		VALUES (@Name, @LocationId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[DiningArea]
		SET Name = @Name, LocationId = @LocationId, Status = @Status
		WHERE Id = @Id;
	END
END