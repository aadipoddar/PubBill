namespace PubBillLibrary.Data;

public static class PaymentModeData
{
	public static async Task InsertPaymentMode(PaymentModeModel paymentMode) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertPaymentMode, paymentMode);
}
