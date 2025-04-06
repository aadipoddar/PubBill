CREATE TABLE [dbo].[DiningTable]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [DiningAreaId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_DiningTable_ToDiningArea] FOREIGN KEY (DiningAreaId) REFERENCES [DiningArea](Id)
)
