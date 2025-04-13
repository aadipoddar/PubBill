namespace PubBillLibrary.Models.Billing;

public class RunningBillModel
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public int LocationId { get; set; }
	public int DiningAreaId { get; set; }
	public int DiningTableId { get; set; }
	public int? PersonId { get; set; } = null;
	public int TotalPeople { get; set; }
	public decimal DiscPercent { get; set; }
	public string DiscReason { get; set; }
	public decimal ServicePercent { get; set; }
	public string Remarks { get; set; }
	public DateTime BillStartDateTime { get; set; }
	public int? BillId { get; set; } = null;
	public bool Status { get; set; }
}

public class RunningBillDetailModel
{
	public int Id { get; set; }
	public int RunningBillId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public string Instruction { get; set; }
	public bool Cancelled { get; set; }
}