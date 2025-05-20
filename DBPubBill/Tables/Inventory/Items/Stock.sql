CREATE TABLE [dbo].[Stock]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RawMaterialId] INT NOT NULL, 
    [Quantity] DECIMAL(7, 3) NOT NULL, 
    [Type] VARCHAR(20) NOT NULL, 
    [PurchaseId] INT NULL, 
    [TransactionDate] DATE NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Stock_ToRawMaterial] FOREIGN KEY (RawMaterialId) REFERENCES [RawMaterial](Id), 
    CONSTRAINT [FK_Stock_ToPurchase] FOREIGN KEY (PurchaseId) REFERENCES [Purchase](Id)
)
