using System.Collections.ObjectModel;

namespace PubBill.Billing.Bill;

internal static class BillWindowHelper
{
	internal static async Task<decimal> CalculateBillTotal(ObservableCollection<CartModel> allCart, RunningBillModel runningBillModel)
	{
		decimal baseTotal = CalculateBaseTotal(allCart, []);

		decimal discountPercent = runningBillModel.DiscPercent;
		decimal discountAmout = baseTotal * (discountPercent / 100);

		decimal productTax = await CalculatProductTotalTax(allCart, []);
		decimal subTotal = baseTotal - discountAmout + productTax;

		decimal servicePercent = runningBillModel.ServicePercent;
		decimal serviceAmount = subTotal * (servicePercent / 100);

		return subTotal + serviceAmount;
	}

	internal static decimal CalculateBaseTotal(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal total = 0;

		total += allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);
		total += kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);

		return total;
	}

	internal static async Task<decimal> CalculatProductTotalTax(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		await CalculatProductCGST(allCart, kotCart) +
		await CalculatProductSGST(allCart, kotCart) +
		await CalculatProductIGST(allCart, kotCart);

	internal static async Task<decimal> CalculatProductCGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal cgst = 0;
		foreach (var item in allCart)
		{
			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			cgst += product.CGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			cgst += product.CGSTAmount * item.Quantity;
		}

		return cgst;
	}

	internal static async Task<decimal> CalculatProductSGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal sgst = 0;
		foreach (var item in allCart)
		{
			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			sgst += product.SGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			sgst += product.SGSTAmount * item.Quantity;
		}

		return sgst;
	}

	internal static async Task<decimal> CalculatProductIGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal igst = 0;
		foreach (var item in allCart)
		{
			var product = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, item.ProductId);
			igst += product.IGSTAmount * item.Quantity;
		}

		foreach (var item in kotCart)
		{
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
}
