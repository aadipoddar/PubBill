using System.Collections.ObjectModel;

namespace PubBill.Billing.Bill;

internal static class BillWindowHelper
{
	#region BillWindow
	internal static decimal CalculateBaseTotal(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.BaseTotal) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.BaseTotal);

	internal static decimal CalculateDiscountAmount(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.DiscAmount) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.DiscAmount);

	internal static decimal CalculateDiscountPercent(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		var discountAmount = CalculateDiscountAmount(allCart, kotCart);
		var baseTotal = CalculateBaseTotal(allCart, kotCart);

		return discountAmount / baseTotal * 100;
	}

	internal static decimal CalculateAfterDiscountTotal(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.AfterDiscount) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.AfterDiscount);

	internal static decimal CalculateProductTotalTax(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		CalculateProductCGST(allCart, kotCart) +
		CalculateProductSGST(allCart, kotCart) +
		CalculateProductIGST(allCart, kotCart);

	internal static decimal CalculateProductCGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.CGSTAmount) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.CGSTAmount);

	internal static decimal CalculateProductSGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart)
	{
		decimal sgst = 0;

		sgst += allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.SGSTAmount);
		sgst += kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.SGSTAmount);

		return sgst;
	}

	internal static decimal CalculateProductIGST(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.IGSTAmount) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.IGSTAmount);

	internal static decimal CalculateSubTotal(ObservableCollection<CartModel> allCart, ObservableCollection<CartModel> kotCart) =>
		allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total) +
		kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);

	internal static decimal CalculateBillTotal(ObservableCollection<CartModel> allCart, decimal servicePercent, int entryPaid)
	{
		decimal subTotal = CalculateSubTotal(allCart, []);
		decimal serviceAmount = subTotal * (servicePercent / 100);

		return subTotal + serviceAmount - entryPaid;
	}

	internal static async Task<bool> CheckKOT(RunningBillModel runningBillModel)
	{
		var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(runningBillModel.Id);
		return kotOrders.Any(k => k.Status);
	}
	#endregion

	#region BillReceipt
	internal static decimal CalculateBaseTotal(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.BaseTotal);

	internal static decimal CalculateDiscountAmount(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.DiscAmount);

	internal static decimal CalculateDiscountPercent(List<RunningBillDetailModel> billItems)
	{
		var discountAmount = CalculateDiscountAmount(billItems);
		var baseTotal = CalculateBaseTotal(billItems);

		return discountAmount / baseTotal * 100;
	}

	internal static decimal CalculateAfterDiscountTotal(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.AfterDiscount);

	internal static decimal CalculateProductTotalTax(List<RunningBillDetailModel> billItems) =>
		CalculateProductCGST(billItems) +
		CalculateProductSGST(billItems) +
		CalculateProductIGST(billItems);

	internal static decimal CalculateProductCGST(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.CGSTAmount);

	internal static decimal CalculateProductSGST(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.SGSTAmount);

	internal static decimal CalculateProductIGST(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.IGSTAmount);

	internal static decimal CalculateSubTotal(List<RunningBillDetailModel> billItems) =>
		billItems.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);

	internal static decimal CalculateServiceAmount(List<RunningBillDetailModel> billItems, decimal servicePercent) =>
		CalculateSubTotal(billItems) * (servicePercent / 100);

	internal static decimal CalculateBillTotal(List<RunningBillDetailModel> billItems, decimal servicePercent, int entryPaid) =>
		CalculateSubTotal(billItems) + CalculateServiceAmount(billItems, servicePercent) - entryPaid;

	internal static decimal GetCGSTPercent(List<RunningBillDetailModel> billItems) =>
		CalculateProductCGST(billItems) > 0
			? billItems.Where(x => x.CGSTPercent > 0).FirstOrDefault().CGSTPercent
			: 0;

	internal static decimal GetSGSTPercent(List<RunningBillDetailModel> billItems) =>
		CalculateProductSGST(billItems) > 0
			? billItems.Where(x => x.SGSTPercent > 0).FirstOrDefault().SGSTPercent
			: 0;

	internal static decimal GetIGSTPercent(List<RunningBillDetailModel> billItems) =>
		CalculateProductIGST(billItems) > 0
			? billItems.Where(x => x.IGSTPercent > 0).FirstOrDefault().IGSTPercent
			: 0;

	internal static string GetDiscountString(List<RunningBillDetailModel> billItems) =>
		$"{CalculateDiscountPercent(billItems):F2}% ({CalculateDiscountAmount(billItems).FormatIndianCurrency()})";

	internal static string GetCGSTString(List<RunningBillDetailModel> billItems) =>
		$"{GetCGSTPercent(billItems):F2}% ({CalculateProductCGST(billItems).FormatIndianCurrency()})";

	internal static string GetSGSTString(List<RunningBillDetailModel> billItems) =>
		$"{GetSGSTPercent(billItems):F2}% ({CalculateProductSGST(billItems).FormatIndianCurrency()})";

	internal static string GetIGSTString(List<RunningBillDetailModel> billItems) =>
		$"{GetIGSTPercent(billItems):F2}% ({CalculateProductIGST(billItems).FormatIndianCurrency()})";

	internal static string GetServiceString(List<RunningBillDetailModel> billItems, decimal servicePercent) =>
		$"{servicePercent:F2}% ({CalculateServiceAmount(billItems, servicePercent).FormatIndianCurrency()})";
	#endregion
}
