CREATE PROCEDURE [dbo].[Insert_BillDetail]
	@Id INT OUTPUT,
	@BillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@Instruction VARCHAR(250)
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
			Instruction
		) VALUES
		(
			@BillId,
			@ProductId,
			@Quantity,
			@Rate,
			@Instruction
		);
		
		SET @Id = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE [dbo].[BillDetail]
		SET
			ProductId = @ProductId,
			Quantity = @Quantity,
			Rate = @Rate,
			Instruction = @Instruction
		WHERE Id = @Id;
	END
END