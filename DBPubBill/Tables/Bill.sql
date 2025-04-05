CREATE TABLE [dbo].[Bill]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [LocationId] INT NOT NULL, 
    [DiningAreaId] INT NOT NULL, 
    [DiningTableId] INT NOT NULL, 
    [PersonId] INT NOT NULL, 
    [TotalPeople] INT NOT NULL DEFAULT 1, 
    [AdjAmount] MONEY NOT NULL DEFAULT 0, 
    [AdjReason] VARCHAR(250) NOT NULL, 
    [Remakrs] VARCHAR(250) NOT NULL, 
    [Total] MONEY NOT NULL, 
    [BillDateTime] DATETIME NOT NULL DEFAULT (((getdate() AT TIME ZONE 'UTC') AT TIME ZONE 'India Standard Time')), 
    CONSTRAINT [FK_Bill_ToUser] FOREIGN KEY (UserId) REFERENCES [User](Id), 
    CONSTRAINT [FK_Bill_ToLocation] FOREIGN KEY (LocationId) REFERENCES [Location](Id), 
    CONSTRAINT [FK_Bill_ToDiningAreaId] FOREIGN KEY (DiningAreaId) REFERENCES [DiningArea](Id), 
    CONSTRAINT [FK_Bill_ToDiningTable] FOREIGN KEY (DiningTableId) REFERENCES [DiningTable](Id), 
    CONSTRAINT [FK_Bill_ToPerson] FOREIGN KEY (PersonId) REFERENCES [Person](Id)
)
