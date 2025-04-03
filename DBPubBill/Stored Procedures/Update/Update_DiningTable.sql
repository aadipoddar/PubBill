CREATE PROCEDURE [dbo].[Update_DiningTable]
	@Id INT,
	@Name VARCHAR(50),
	@DiningAreaId INT,
	@Status BIT
AS
BEGIN
	UPDATE [dbo].[DiningTable]
	SET
		Name = @Name,
		DiningAreaId = @DiningAreaId,
		Status = @Status
	WHERE Id = @Id;
END;