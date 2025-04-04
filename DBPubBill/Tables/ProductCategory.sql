CREATE TABLE [dbo].[ProductCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [ProductGroupId] INT NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_ProductCategory_ToProductGroup] FOREIGN KEY (ProductGroupId) REFERENCES [ProductGroup](Id) 
)
