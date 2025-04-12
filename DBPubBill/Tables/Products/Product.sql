CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(250) NOT NULL, 
    [Code] VARCHAR(50) NOT NULL UNIQUE, 
    [ProductCategoryId] INT NOT NULL, 
    [Rate] MONEY NOT NULL, 
    [TaxId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Product_ToProductCategory] FOREIGN KEY (ProductCategoryId) REFERENCES [ProductCategory](Id), 
    CONSTRAINT [FK_Product_ToTax] FOREIGN KEY (TaxId) REFERENCES [Tax](Id)
)
