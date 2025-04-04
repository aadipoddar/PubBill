CREATE PROCEDURE [dbo].[Update_ProductGroup]
	@Id INT,
	@Name VARCHAR(50),
	@Status BIT
AS
BEGIN
	UPDATE [ProductGroup]
	SET Name = @Name, Status = @Status
	WHERE Id = @Id;
END;