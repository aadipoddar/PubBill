﻿CREATE TABLE [dbo].[Person]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(250) NOT NULL,
    [Number] VARCHAR(10) NOT NULL UNIQUE,
    [Loyalty] BIT NOT NULL DEFAULT 0
)
