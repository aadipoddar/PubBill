CREATE PROCEDURE [dbo].[Insert_RunningBillDetail]
	@Id INT,
	@RunningBillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@Instruction VARCHAR(250)
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
			Instruction
		) VALUES
		(
			@RunningBillId,
			@ProductId,
			@Quantity,
			@Rate,
			@Instruction
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
			Instruction = @Instruction
		WHERE Id = @Id;
	END
END