CREATE PROCEDURE [dbo].[Insert_Stock]
	@Id INT,
	@RawMaterialId INT,
	@Quantity DECIMAL(7, 3),
	@Type VARCHAR(20),
	@PurchaseId INT,
	@TransactionDate DATE
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Stock] ([RawMaterialId], [Quantity], [Type], [PurchaseId], [TransactionDate])
		VALUES (@RawMaterialId, @Quantity, @Type, @PurchaseId, @TransactionDate)
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Stock]
		SET [RawMaterialId] = @RawMaterialId,
			[Quantity] = @Quantity,
			[Type] = @Type,
			[PurchaseId] = @PurchaseId,
			[TransactionDate] = @TransactionDate
		WHERE [Id] = @Id
	END
END