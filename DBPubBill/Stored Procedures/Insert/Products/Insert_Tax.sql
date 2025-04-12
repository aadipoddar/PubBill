﻿CREATE PROCEDURE [dbo].[Insert_Tax]
	@Id INT,
	@Code VARCHAR(50),
	@CGST DECIMAL(5, 2),
	@SGST DECIMAL(5, 2),
	@IGST DECIMAL(5, 2),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Tax] (Code, CGST, SGST, IGST, [Status])
		VALUES (@Code, @CGST, @SGST, @IGST, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Tax]
		SET Code = @Code, CGST = @CGST, SGST = @SGST, IGST = @IGST, [Status] = @Status
		WHERE Id = @Id;
	END
END;