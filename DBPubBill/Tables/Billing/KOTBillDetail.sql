﻿CREATE TABLE [dbo].[KOTBillDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RunningBillId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [Instruction] VARCHAR(250) NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    [Cancelled] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_KOTBillDetail_ToBill] FOREIGN KEY (RunningBillId) REFERENCES [RunningBill](Id), 
    CONSTRAINT [FK_KOTBillDetail_ToProduct] FOREIGN KEY (ProductId) REFERENCES [Product](Id)
)
