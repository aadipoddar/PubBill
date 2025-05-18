CREATE TABLE [dbo].[Purchase]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BillNo] VARCHAR(50) NOT NULL, 
    [SupplierId] INT NOT NULL, 
    [BillDate] DATE NOT NULL, 
    [Remarks] VARCHAR(250) NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Purchase_ToSupplier] FOREIGN KEY (SupplierId) REFERENCES [Supplier](Id)
)
