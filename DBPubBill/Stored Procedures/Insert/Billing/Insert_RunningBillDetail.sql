CREATE PROCEDURE [dbo].[Insert_RunningBillDetail]
	@Id INT,
	@RunningBillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@Instruction VARCHAR(250),
	@Discountable BIT,
	@Cancelled BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[RunningBillDetail]
		(
			RunningBillId,
			ProductId,
			Quantity,
			Rate,
			Instruction,
			Discountable,
			Cancelled
		) VALUES
		(
			@RunningBillId,
			@ProductId,
			@Quantity,
			@Rate,
			@Instruction,
			@Discountable,
			@Cancelled
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[RunningBillDetail]
		SET
			RunningBillId = @RunningBillId,
			ProductId = @ProductId,
			Quantity = @Quantity,
			Rate = @Rate,
			Instruction = @Instruction,
			Discountable = @Discountable,
			Cancelled = @Cancelled
		WHERE Id = @Id;
	END
END