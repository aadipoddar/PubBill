CREATE PROCEDURE [dbo].[Insert_KOTBillDetail]
	@Id INT,
	@RunningBillId INT,
	@ProductId INT,
	@Quantity INT,
	@Instruction VARCHAR(250),
	@Status BIT,
	@Cancelled BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[KOTBillDetail]
		(
			RunningBillId,
			ProductId,
			Quantity,
			Instruction,
			[Status],
			Cancelled
		) VALUES
		(
			@RunningBillId,
			@ProductId,
			@Quantity,
			@Instruction,
			@Status,
			@Cancelled
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[KOTBillDetail]
		SET
			RunningBillId = @RunningBillId,
			ProductId = @ProductId,
			Quantity = @Quantity,
			Instruction = @Instruction,
			[Status] = @Status,
			Cancelled = @Cancelled
		WHERE Id = @Id;
	END
END