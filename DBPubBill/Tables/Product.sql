﻿CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(250) NOT NULL, 
    [Code] VARCHAR(50) NOT NULL UNIQUE, 
    [Rate] MONEY NOT NULL, 
    [ProductCategoryId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Product_ToProductCategory] FOREIGN KEY (ProductCategoryId) REFERENCES [ProductCategory](Id)
)
