CREATE TABLE [dbo].[DiningAreaKitchen]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DiningAreaId] INT NOT NULL, 
    [KitchenId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_DiningAreaKitchen_ToDiningArea] FOREIGN KEY (DiningAreaId) REFERENCES [DiningArea](Id), 
    CONSTRAINT [FK_DiningAreaKitchen_ToKitchen] FOREIGN KEY (KitchenId) REFERENCES [Kitchen](Id)
)
