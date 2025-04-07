CREATE TABLE [dbo].[BillDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BillId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [Rate] MONEY NOT NULL, 
    [Instruction] VARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_BillDetail_ToBill] FOREIGN KEY (BillId) REFERENCES [Bill](Id), 
    CONSTRAINT [FK_BillDetail_ToProduct] FOREIGN KEY (ProductId) REFERENCES [Product](Id)
)
