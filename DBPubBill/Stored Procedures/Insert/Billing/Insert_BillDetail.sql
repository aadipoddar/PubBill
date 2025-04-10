CREATE PROCEDURE [dbo].[Insert_BillDetail]
	@Id INT,
	@BillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@Instruction VARCHAR(250),
	@Cancelled BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[BillDetail]
		(
			BillId,
			ProductId,
			Quantity,
			Rate,
			Instruction,
			Cancelled
		) VALUES
		(
			@BillId,
			@ProductId,
			@Quantity,
			@Rate,
			@Instruction,
			@Cancelled
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[BillDetail]
		SET
			BillId = @BillId,
			ProductId = @ProductId,
			Quantity = @Quantity,
			Rate = @Rate,
			Instruction = @Instruction,
			Cancelled = @Cancelled
		WHERE Id = @Id;
	END
END