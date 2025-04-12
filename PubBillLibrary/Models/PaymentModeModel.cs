namespace PubBillLibrary.Models;

public class PaymentModeModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public bool Status { get; set; }
}

public class BillPaymentDetailModel
{
	public int Id { get; set; }
	public int BillId { get; set; }
	public int PaymentModeId { get; set; }
	public decimal Amount { get; set; }
	public bool Status { get; set; }
}

public class BillPaymentModeModel
{
	public int PaymentModeId { get; set; }
	public string PaymentModeName { get; set; }
	public decimal Amount { get; set; }
}