﻿CREATE PROCEDURE [dbo].[Insert_Supplier]
	@Id INT,
	@Name VARCHAR(250),
	@Code VARCHAR(50),
	@GSTNo VARCHAR(20),
	@Phone VARCHAR(20),
	@Email VARCHAR(100),
	@Address VARCHAR(500),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Supplier] ([Name], [Code], [GSTNo], [Phone], [Email], [Address], [Status])
		VALUES (@Name, @Code, @GSTNo, @Phone, @Email, @Address, @Status);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Supplier]
		SET
			Name = @Name,
			Code = @Code,
			GSTNo = @GSTNo,
			Phone = @Phone,
			Email = @Email,
			Address = @Address,
			Status = @Status
		WHERE Id = @Id;
	END
END