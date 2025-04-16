using System.Collections.ObjectModel;

namespace PubBill.Billing.Bill;

internal static class BillWindowHelper
{
	internal static async Task<decimal> CalculateBillTotal(ObservableCollection<CartModel> allCart, RunningBillModel runningBillModel, int entryPaid)
	{
		decimal baseTotal = CalculateBaseTotal(allCart, []);

		decimal discountPercent = runningBillModel.DiscPercent;
		decimal discountAmout = baseTotal * (discountPercent / 100);

		decimal productTax = await CalculateProductTotalTax(allCart, []);
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

	internal static async Task<decimal> CalculateProductTotalTax(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		await CalculateProductCGST(allCart, kotCart) +
		await CalculateProductSGST(allCart, kotCart) +
		await CalculateProductIGST(allCart, kotCart);

	internal static async Task<decimal> CalculateProductCGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal cgst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			cgst += product.CGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			cgst += product.CGSTAmount * item.Quantity;
		}

		return cgst;
	}

	internal static async Task<decimal> CalculateProductSGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal sgst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			sgst += product.SGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			sgst += product.SGSTAmount * item.Quantity;
		}

		return sgst;
	}

	internal static async Task<decimal> CalculateProductIGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal igst = 0;
		foreach (var item in allCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			igst += product.IGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			igst += product.IGSTAmount * item.Quantity;
		}

		return igst;
	}

	internal static async Task<bool> CheckKOT(RunningBillModel runningBillModel)
	{
		var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(runningBillModel.Id);
		return kotOrders.Any(k => k.Status);
	}

	internal static async Task<decimal> CalculateProductTotalTax(List<BillDetailModel> billItems) =>
		await CalculateProductCGST(billItems) +
		await CalculateProductSGST(billItems) +
		await CalculateProductIGST(billItems);

	internal static async Task<decimal> CalculateProductCGST(List<BillDetailModel> billItems)
	{
		decimal cgst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			cgst += product.CGSTAmount * item.Quantity;
		}

		return cgst;
	}

	internal static async Task<decimal> CalculateProductSGST(List<BillDetailModel> billItems)
	{
		decimal sgst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			sgst += product.SGSTAmount * item.Quantity;
		}
		return sgst;
	}

	internal static async Task<decimal> CalculateProductIGST(List<BillDetailModel> billItems)
	{
		decimal igst = 0;
		foreach (var item in billItems)
		{
			if (item.Cancelled)
				continue;

			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			igst += product.IGSTAmount * item.Quantity;
		}
		return igst;
	}

	internal static string GetDiscountString(BillModel bill, List<BillDetailModel> billItems)
	{
		decimal discountAmount = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * (bill.DiscPercent / 100);
		string discountString = $"{bill.DiscPercent}% ({discountAmount.FormatIndianCurrency()})";
		return discountString;
	}

	internal static async Task<string> GetSGSTString(List<BillDetailModel> billItems)
	{
		decimal sgstAmount = await CalculateProductSGST(billItems);

		if (sgstAmount == 0)
			return string.Empty;

		decimal sgstPercent = sgstAmount / billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * 100;
		return $"{sgstPercent:F2}% ({sgstAmount.FormatIndianCurrency()})";
	}

	internal static async Task<string> GetCGSTString(List<BillDetailModel> billItems)
	{
		decimal cgstAmount = await CalculateProductCGST(billItems);

		if (cgstAmount == 0)
			return string.Empty;

		decimal cgstPercent = cgstAmount / billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * 100;
		return $"{cgstPercent:F2}% ({cgstAmount.FormatIndianCurrency()})";
	}

	internal static async Task<string> GetIGSTString(List<BillDetailModel> billItems)
	{
		decimal igstAmount = await CalculateProductIGST(billItems);

		if (igstAmount == 0)
			return string.Empty;

		decimal igstPercent = igstAmount / billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * 100;

		return $"{igstPercent:F2}% ({igstAmount.FormatIndianCurrency()})";
	}

	internal static async Task<string> GetServiceString(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal baseTotal = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity);
		decimal discountAmount = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * (billModel.DiscPercent / 100);

		decimal subTotal = baseTotal - discountAmount;

		decimal productTax = await CalculateProductTotalTax(billItems);
		subTotal += productTax;

		decimal servicePercent = billModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		if (serviceAmount == 0)
			return string.Empty;

		return $"{servicePercent:F2}% ({serviceAmount.FormatIndianCurrency()})";
	}

	internal static async Task<decimal> CalculateBillTotal(BillModel billModel, List<BillDetailModel> billItems)
	{
		decimal baseTotal = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity);
		decimal discountAmount = billItems.Where(x => !x.Cancelled).Sum(item => item.Rate * item.Quantity) * (billModel.DiscPercent / 100);

		decimal subTotal = baseTotal - discountAmount;

		decimal productTax = await CalculateProductTotalTax(billItems);
		subTotal += productTax;

		decimal servicePercent = billModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		subTotal += serviceAmount;

		subTotal -= billModel.EntryPaid;

		return subTotal;
	}
}
