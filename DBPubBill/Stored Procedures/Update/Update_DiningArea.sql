CREATE PROCEDURE [dbo].[Update_DiningArea]
	@Id INT,
	@Name VARCHAR(50),
	@LocationId INT,
	@Status BIT
AS
BEGIN
	UPDATE [dbo].[DiningArea]
	SET Name = @Name, LocationId = @LocationId, Status = @Status
	WHERE Id = @Id;
END;