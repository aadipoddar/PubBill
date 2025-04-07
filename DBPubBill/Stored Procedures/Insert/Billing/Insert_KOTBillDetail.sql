﻿CREATE PROCEDURE [dbo].[Insert_KOTBillDetail]
	@Id INT,
	@RunningBillId INT,
	@ProductId INT,
	@Quantity INT,
	@Instruction VARCHAR(250)
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[KOTBillDetail]
		(
			RunningBillId,
			ProductId,
			Quantity,
			Instruction
		) VALUES
		(
			@RunningBillId,
			@ProductId,
			@Quantity,
			@Instruction
		);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[KOTBillDetail]
		SET
			RunningBillId = @RunningBillId,
			ProductId = @ProductId,
			Quantity = @Quantity,
			Instruction = @Instruction
		WHERE Id = @Id;
	END
END