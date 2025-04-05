namespace PubBillLibrary.Models;

public class CartModel
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public decimal Total => Quantity * Rate;
	public string Instructions { get; set; }
}