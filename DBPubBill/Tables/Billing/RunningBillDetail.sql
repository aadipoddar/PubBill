CREATE TABLE [dbo].[RunningBillDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RunningBillId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [Rate] MONEY NOT NULL, 
    [Instruction] VARCHAR(250) NOT NULL, 
    [Discountable] BIT NOT NULL DEFAULT 1, 
    [Cancelled] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_RunningBillDetail_ToBill] FOREIGN KEY (RunningBillId) REFERENCES [RunningBill](Id), 
    CONSTRAINT [FK_RunningBillDetail_ToProduct] FOREIGN KEY (ProductId) REFERENCES [Product](Id)
)
