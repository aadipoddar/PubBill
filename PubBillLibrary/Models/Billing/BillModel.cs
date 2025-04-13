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
	public decimal CGST { get; set; }
	public decimal SGST { get; set; }
	public decimal IGST { get; set; }
	public string Instruction { get; set; }
	public bool Cancelled { get; set; } = false;
	public bool Status { get; set; } = true;
}

public class CartModel
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public decimal Total => Quantity * Rate;
	public string Instruction { get; set; }
	public bool Cancelled { get; set; }
}