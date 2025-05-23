﻿CREATE PROCEDURE [dbo].[Insert_Purchase]
	@Id INT OUTPUT,
	@BillNo VARCHAR(50),
	@SupplierId INT,
	@BillDate DATE,
	@CDPercent DECIMAL(5, 2),
	@CDAmount MONEY,
	@Remarks VARCHAR(250),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Purchase] ([BillNo], [SupplierId], [BillDate], [CDPercent], [CDAmount], [Remarks], [Status])
		VALUES (@BillNo, @SupplierId, @BillDate, @CDPercent, @CDAmount, @Remarks, @Status);

		SET @Id = SCOPE_IDENTITY();
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Purchase]
		SET
			BillNo = @BillNo,
			SupplierId = @SupplierId,
			BillDate = @BillDate,
			CDPercent = @CDPercent,
			CDAmount = @CDAmount,
			Remarks = @Remarks,
			Status = @Status
		WHERE Id = @Id;
	END

	SELECT @Id AS Id;
END