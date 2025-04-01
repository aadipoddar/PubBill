CREATE PROCEDURE [dbo].[Update_Location]
	@Id INT,
	@Name VARCHAR(50),
	@Status BIT
AS
BEGIN
	UPDATE [dbo].[Location]
	SET
		Name = @Name,
		Status = @Status
	WHERE Id = @Id
END;