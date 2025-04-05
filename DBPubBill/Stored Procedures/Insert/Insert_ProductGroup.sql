CREATE PROCEDURE [dbo].[Insert_ProductGroup]
	@Id INT,
	@Name VARCHAR(50),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[ProductGroup] (Name, Status)
		VALUES (@Name, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[ProductGroup]
		SET Name = @Name, Status = @Status
		WHERE Id = @Id;
	END
END;