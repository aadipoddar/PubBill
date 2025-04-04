﻿namespace PubBillLibrary.Models;

public class BillModel
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public int LocationId { get; set; }
	public int DiningAreaId { get; set; }
	public int DiningTableId { get; set; }
	public int PersonId { get; set; }
	public int TotalPeople { get; set; }
	public decimal AdjAmount { get; set; }
	public string AdjReason { get; set; }
	public string Remarks { get; set; }
	public decimal Total { get; set; }
	public int PaymentModeId { get; set; }
	public DateTime BillDateTime { get; set; }
}

public class BillDetailModel
{
	public int Id { get; set; }
	public int BillId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public string Instruction { get; set; }
}

public class CartModel
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Rate { get; set; }
	public decimal Total => Quantity * Rate;
	public string Instruction { get; set; }
}