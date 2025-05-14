CREATE TABLE [dbo].[Kitchen]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [KitchenTypeId] INT NOT NULL,
    [PrinterName] VARCHAR(100) NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Kitchen_ToKitchenType] FOREIGN KEY (KitchenTypeId) REFERENCES [KitchenType](Id), 
)
