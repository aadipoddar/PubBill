namespace PubBillLibrary.Models.Billing;

public class BillOverviewModel
{
	public int BillId { get; set; }
	public int LocationId { get; set; }
	public string LocationName { get; set; }
	public int DiningAreaId { get; set; }
	public string DiningAreaName { get; set; }
	public int DiningTableId { get; set; }
	public string DiningTableName { get; set; }
	public int UserId { get; set; }
	public string UserName { get; set; }
	public DateTime BillDateTime { get; set; }
	public int? PersonId { get; set; }
	public string PersonName { get; set; }
	public string PersonNumber { get; set; }
	public bool? PersonLoyalty { get; set; }
	public int TotalPeople { get; set; }
	public decimal DiscountPercent { get; set; }
	public decimal DiscReason { get; set; }
	public decimal ServicePercent { get; set; }
	public string Remarks { get; set; }
	public int TotalProducts { get; set; }
	public int TotalQuantity { get; set; }
	public decimal SGSTPercent { get; set; }
	public decimal CGSTPercent { get; set; }
	public decimal IGSTPercent { get; set; }
	public decimal SGSTAmount { get; set; }
	public decimal CGSTAmount { get; set; }
	public decimal IGSTAmount { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal TotalTaxAmount { get; set; }
	public decimal ServiceAmount { get; set; }
	public decimal BaseTotal { get; set; }
	public decimal AfterDiscount { get; set; }
	public decimal AfterTax { get; set; }
	public decimal AfterService { get; set; }
	public int EntryPaid { get; set; }
	public decimal FinalAmount { get; set; }
	public decimal? Cash { get; set; }
	public decimal? Card { get; set; }
	public decimal? UPI { get; set; }
}