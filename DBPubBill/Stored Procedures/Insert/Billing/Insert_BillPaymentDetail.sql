CREATE PROCEDURE [dbo].[Insert_BillPaymentDetail]
	@Id INT,
	@BillId INT,
	@PaymentModeId INT,
	@Amount MONEY,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[BillPaymentDetail]
		(
			BillId,
			PaymentModeId,
			Amount,
			[Status]
		) VALUES
		(
			@BillId,
			@PaymentModeId,
			@Amount,
			@Status
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[BillPaymentDetail]
		SET
			BillId = @BillId,
			PaymentModeId = @PaymentModeId,
			Amount = @Amount,
			[Status] = @Status
		WHERE Id = @Id;
	END
END