CREATE TABLE [dbo].[BillPaymentDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BillId] INT NOT NULL, 
    [PaymentModeId] INT NOT NULL, 
    [Amount] MONEY NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_BillPaymentDetail_ToBill] FOREIGN KEY (BillId) REFERENCES [Bill](Id), 
    CONSTRAINT [FK_BillPaymentDetail_ToPaymentMode] FOREIGN KEY (PaymentModeId) REFERENCES [PaymentMode](Id)
)
