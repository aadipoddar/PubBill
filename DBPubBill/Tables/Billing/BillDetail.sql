CREATE TABLE [dbo].[BillDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BillId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [Rate] MONEY NOT NULL, 
    [CGST] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [SGST] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [IGST] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Instruction] VARCHAR(250) NOT NULL, 
    [Cancelled] BIT NOT NULL DEFAULT 0, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_BillDetail_ToBill] FOREIGN KEY (BillId) REFERENCES [Bill](Id), 
    CONSTRAINT [FK_BillDetail_ToProduct] FOREIGN KEY (ProductId) REFERENCES [Product](Id)
)
