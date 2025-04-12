CREATE PROCEDURE [dbo].[Insert_BillDetail]
	@Id INT,
	@BillId INT,
	@ProductId INT,
	@Quantity INT,
	@Rate MONEY,
	@CGST DECIMAL(5,2),
	@SGST DECIMAL(5,2),
	@IGST DECIMAL(5,2),
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
			CGST,
			SGST,
			IGST,
			Instruction,
			Cancelled
		) VALUES
		(
			@BillId,
			@ProductId,
			@Quantity,
			@Rate,
			@CGST,
			@SGST,
			@IGST,
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
			CGST = @CGST,
			SGST = @SGST,
			IGST = @IGST,
			Instruction = @Instruction,
			Cancelled = @Cancelled
		WHERE Id = @Id;
	END
END