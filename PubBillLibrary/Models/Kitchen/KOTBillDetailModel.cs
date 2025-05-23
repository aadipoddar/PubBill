﻿namespace PubBillLibrary.Models.Kitchen;

public class KOTBillDetailModel
{
	public int Id { get; set; }
	public int RunningBillId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public string Instruction { get; set; }
	public bool Status { get; set; }
	public bool Cancelled { get; set; }
}
