namespace PubBillLibrary.Models;

public class PubEntryTransactionModel
{
	public int Id { get; set; }
	public int PersonId { get; set; }
	public string PersonName { get; set; }
	public string PersonNumber { get; set; }
	public bool PersonLoyalty { get; set; }
	public int Male { get; set; }
	public int Female { get; set; }
	public int Cash { get; set; }
	public int Card { get; set; }
	public int UPI { get; set; }
	public int Amex { get; set; }
	public DateTime DateTime { get; set; }
	public int LocationId { get; set; }
}

public class PubEntryAdvanceModel
{
	public int Id { get; set; }
	public int LocationId { get; set; }
	public int PersonId { get; set; }
	public DateTime DateTime { get; set; }
	public DateTime AdvanceDate { get; set; }
	public int Booking { get; set; }
	public string ApprovedBy { get; set; }
	public int UserId { get; set; }
	public int TransactionId { get; set; }
}

public class PubEntryAdvanceDetailModel
{
	public int Id { get; set; }
	public int AdvanceId { get; set; }
	public int PaymentModeId { get; set; }
	public int Amount { get; set; }
}