using System.Collections.ObjectModel;

namespace PubBill.Billing.Bill;

internal static class BillWindowHelper
{
	#region BillWindow
	internal static async Task<decimal> CalculateBillTotal(ObservableCollection<CartModel> allCart, RunningBillModel runningBillModel, int entryPaid)
	{
		decimal baseTotal = CalculateBaseTotal(allCart, []);

		decimal discountPercent = runningBillModel.DiscPercent;
		decimal discountAmout = baseTotal * (discountPercent / 100);

		decimal productTax = await CalculateProductTotalTax(allCart, [], discountPercent);
		decimal subTotal = baseTotal - discountAmout + productTax;

		decimal servicePercent = runningBillModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		return subTotal + serviceAmount - entryPaid;
	}

	internal static decimal CalculateBaseTotal(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal total = 0;

		total += allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);
		total += kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);

		return total;
	}

	internal static async Task<decimal> CalculateProductTotalTax(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart, decimal discountPercent) =>
		await CalculateProductCGST(allCart, kotCart, discountPercent) +
		await CalculateProductSGST(allCart, kotCart, discountPercent) +
		await CalculateProductIGST(allCart, kotCart, discountPercent: discountPercent);

	internal static async Task<decimal> CalculateProductCGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart, decimal discountPercent)
	{
		decimal cgst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			cgst += product.CGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			cgst += product.CGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		return cgst;
	}

	internal static async Task<decimal> CalculateProductSGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart, decimal discountPercent)
	{
		decimal sgst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			sgst += product.SGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			sgst += product.SGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		return sgst;
	}

	internal static async Task<decimal> CalculateProductIGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart, decimal discountPercent)
	{
		decimal igst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			igst += product.IGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (discountPercent / 100));
			igst += product.IGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		return igst;
	}

	internal static async Task<bool> CheckKOT(RunningBillModel runningBillModel)
	{
		var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(runningBillModel.Id);
		return kotOrders.Any(k => k.Status);
	}
	#endregion

	#region BillReceipt
	internal static async Task<decimal> CalculateProductTotalTax(BillModel billModel, List<BillDetailModel> billItems) =>
		await CalculateProductCGST(billModel, billItems) +
		await CalculateProductSGST(billModel, billItems) +
		await CalculateProductIGST(billModel, billItems);

	internal static async Task<decimal> CalculateProductCGST(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal cgst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (billModel.DiscPercent / 100));
			cgst += product.CGSTPercent * afterDiscount / 100 * item.Quantity;
		}

		return cgst;
	}

	internal static async Task<decimal> CalculateProductSGST(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal sgst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (billModel.DiscPercent / 100));
			sgst += product.SGSTPercent * afterDiscount / 100 * item.Quantity;
		}
		return sgst;
	}

	internal static async Task<decimal> CalculateProductIGST(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal igst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			var afterDiscount = item.Rate - (item.Rate * (billModel.DiscPercent / 100));
			igst += product.IGSTPercent * afterDiscount / 100 * item.Quantity;
		}
		return igst;
	}


	internal static string GetDiscountString(BillModel bill, List<BillDetailModel> billItems)
	{
		decimal discountAmount = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * (bill.DiscPercent / 100);
		string discountString = $"{bill.DiscPercent}% ({discountAmount.FormatIndianCurrency()})";
		return discountString;
	}

	internal static async Task<string> GetSGSTString(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal sgstAmount = await CalculateProductSGST(billModel, billItems);

		if (sgstAmount == 0)
			return string.Empty;

		var sgstPercent = sgstAmount / GetTotalAfterDiscount(billModel, billItems) * 100;
		return $"{sgstPercent:F2}% ({sgstAmount.FormatIndianCurrency()})";
	}

	internal static async Task<string> GetCGSTString(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal cgstAmount = await CalculateProductCGST(billModel, billItems);

		if (cgstAmount == 0)
			return string.Empty;

		decimal cgstPercent = cgstAmount / GetTotalAfterDiscount(billModel, billItems) * 100;
		return $"{cgstPercent:F2}% ({cgstAmount.FormatIndianCurrency()})";
	}

	internal static async Task<string> GetIGSTString(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal igstAmount = await CalculateProductIGST(billModel, billItems);

		if (igstAmount == 0)
			return string.Empty;

		decimal igstPercent = igstAmount / GetTotalAfterDiscount(billModel, billItems) * 100;

		return $"{igstPercent:F2}% ({igstAmount.FormatIndianCurrency()})";
	}

	private static decimal GetTotalAfterDiscount(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal baseTotal = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity);
		decimal discountAmount = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * (billModel.DiscPercent / 100);
		decimal total = baseTotal - discountAmount;
		return total;
	}

	internal static async Task<string> GetServiceString(BillModel billModel, List<BillDetailModel> billItems)
	{
		var subTotal = GetTotalAfterDiscount(billModel, billItems);

		decimal productTax = await CalculateProductTotalTax(billModel, billItems);
		subTotal += productTax;

		decimal servicePercent = billModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		if (serviceAmount == 0)
			return string.Empty;

		return $"{servicePercent:F2}% ({serviceAmount.FormatIndianCurrency()})";
	}

	internal static async Task<decimal> CalculateBillTotal(BillModel billModel, List<BillDetailModel> billItems)
	{
		var subTotal = GetTotalAfterDiscount(billModel, billItems);

		decimal productTax = await CalculateProductTotalTax(billModel, billItems);
		subTotal += productTax;

		decimal servicePercent = billModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		subTotal += serviceAmount;

		subTotal -= billModel.EntryPaid;

		return subTotal;
	}
	#endregion
}
