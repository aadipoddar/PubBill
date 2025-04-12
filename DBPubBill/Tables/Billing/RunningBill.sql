﻿CREATE TABLE [dbo].[RunningBill]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [LocationId] INT NOT NULL, 
    [DiningAreaId] INT NOT NULL, 
    [DiningTableId] INT NOT NULL, 
    [PersonId] INT NULL, 
    [TotalPeople] INT NOT NULL DEFAULT 1, 
    [DiscPercent] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [DiscReason] VARCHAR(250) NOT NULL, 
    [ServicePercent] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Remarks] VARCHAR(250) NOT NULL, 
    [BillStartDateTime] DATETIME NOT NULL DEFAULT (((getdate() AT TIME ZONE 'UTC') AT TIME ZONE 'India Standard Time')), 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_RunningTable_ToUser] FOREIGN KEY (UserId) REFERENCES [User](Id), 
    CONSTRAINT [FK_RunningTable_ToLocation] FOREIGN KEY (LocationId) REFERENCES [Location](Id), 
    CONSTRAINT [FK_RunningTable_ToDiningAreaId] FOREIGN KEY (DiningAreaId) REFERENCES [DiningArea](Id), 
    CONSTRAINT [FK_RunningTable_ToDiningTable] FOREIGN KEY (DiningTableId) REFERENCES [DiningTable](Id), 
    CONSTRAINT [FK_RunningTable_ToPerson] FOREIGN KEY (PersonId) REFERENCES [Person](Id), 
)
