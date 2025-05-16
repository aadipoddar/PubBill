namespace PubBillLibrary.Models.Billing;

public class BillModel
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
	public int EntryPaid { get; set; }
	public string Remarks { get; set; }
	public DateTime BillDateTime { get; set; }
}

public class BillDetailModel
{
	public int Id { get; set; }
	public int BillId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public decimal BaseTotal { get; set; }
	public string Instruction { get; set; }
	public bool Discountable { get; set; }
	public bool SelfDiscount { get; set; }
	public decimal DiscPercent { get; set; }
	public decimal DiscAmount { get; set; }
	public decimal AfterDiscount { get; set; }
	public decimal CGSTPercent { get; set; }
	public decimal CGSTAmount { get; set; }
	public decimal SGSTPercent { get; set; }
	public decimal SGSTAmount { get; set; }
	public decimal IGSTPercent { get; set; }
	public decimal IGSTAmount { get; set; }
	public decimal Total { get; set; }
	public bool Cancelled { get; set; }
	public bool Status { get; set; } = true;
}