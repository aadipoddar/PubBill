CREATE PROCEDURE [dbo].[Insert_PurchasePaymentDetail]
	@Id INT,
	@PurchaseId INT,
	@PaymentTypeId INT,
	@Amount MONEY,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[PurchasePaymentDetail] ([PurchaseId], [PaymentModeId], [Amount], [Status])
		VALUES (@PurchaseId, @PaymentTypeId, @Amount, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[PurchasePaymentDetail]
		SET
			PurchaseId = @PurchaseId,
			PaymentModeId = @PaymentTypeId,
			Amount = @Amount,
			Status = @Status
		WHERE Id = @Id;
	END
END