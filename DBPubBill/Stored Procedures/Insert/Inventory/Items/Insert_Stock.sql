CREATE PROCEDURE [dbo].[Insert_Stock]
	@Id INT,
	@RawMaterialId INT,
	@Quantity DECIMAL(7, 3),
	@Type VARCHAR(20),
	@PurchaseId INT,
	@TransactionDT DATETIME
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Stock] ([RawMaterialId], [Quantity], [Type], [PurchaseId], [TransactionDT])
		VALUES (@RawMaterialId, @Quantity, @Type, @PurchaseId, @TransactionDT)
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Stock]
		SET [RawMaterialId] = @RawMaterialId,
			[Quantity] = @Quantity,
			[Type] = @Type,
			[PurchaseId] = @PurchaseId,
			[TransactionDT] = @TransactionDT
		WHERE [Id] = @Id
	END
END