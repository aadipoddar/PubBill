﻿CREATE TABLE [dbo].[DiningArea]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [LocationId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_DiningArea_ToLocation] FOREIGN KEY (LocationId) REFERENCES [Location](Id)
)
