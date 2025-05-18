CREATE TABLE [dbo].[PurchasePaymentDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PurchaseId] INT NOT NULL, 
    [PaymentModeId] INT NOT NULL, 
    [Amount] MONEY NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_PurchasePaymentDetail_ToBill] FOREIGN KEY (PurchaseId) REFERENCES [Purchase](Id), 
    CONSTRAINT [FK_PurchasePaymentDetail_ToPaymentMode] FOREIGN KEY (PaymentModeId) REFERENCES [PaymentMode](Id)
)
