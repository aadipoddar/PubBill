CREATE PROCEDURE [dbo].[Insert_Kitchen]
	@Id INT,
	@Name VARCHAR(50),
	@KitchenTypeId INT,
	@PrinterName VARCHAR(20),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Kitchen] ([Name], [KitchenTypeId], [PrinterName], [Status])
		VALUES (@Name, @KitchenTypeId, @PrinterName, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Kitchen]
		SET [Name] = @Name,
			[KitchenTypeId] = @KitchenTypeId,
			[PrinterName] = @PrinterName,
			[Status] = @Status
		WHERE [Id] = @Id;
	END
END;