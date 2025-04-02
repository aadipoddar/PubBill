CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Name]       VARCHAR (100) NOT NULL,
    [Password]   SMALLINT      NOT NULL UNIQUE,
    [LocationId] INT           NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1,
    [Bill]       BIT           NOT NULL DEFAULT 0, 
    [KOT] BIT NOT NULL DEFAULT 0, 
    [Inventory] BIT NOT NULL DEFAULT 0, 
    [Admin] BIT NOT NULL DEFAULT 0, 
)