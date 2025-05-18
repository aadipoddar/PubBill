﻿namespace PubBillLibrary.Data.Inventory.Purchase;

public static class PurchaseData
{
	public static async Task<int> InsertPurchase(PurchaseModel purchase) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertPurchase, purchase)).FirstOrDefault();

	public static async Task InsertPurchaseDetail(PurchaseDetailModel purchaseDetail) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertPurchaseDetail, purchaseDetail);

	public static async Task InsertPurchasePaymentDetail(PurchasePayementDetailModel purchasePaymentDetail) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertPurchasePaymentDetail, purchasePaymentDetail);
}