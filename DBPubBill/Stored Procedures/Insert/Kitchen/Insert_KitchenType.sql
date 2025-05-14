CREATE PROCEDURE [dbo].[Insert_KitchenType]
	@Id INT,
	@Name VARCHAR(50),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[KitchenType] ([Name], [Status])
		VALUES (@Name, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[KitchenType]
		SET [Name] = @Name,
			[Status] = @Status
		WHERE [Id] = @Id;
	END
END;