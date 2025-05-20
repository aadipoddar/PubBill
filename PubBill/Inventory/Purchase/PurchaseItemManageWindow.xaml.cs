using System.Windows;

namespace PubBill.Inventory.Purchase;

/// <summary>
/// Interaction logic for PurchaseItemManageWindow.xaml
/// </summary>
public partial class PurchaseItemManageWindow : Window
{
	private readonly PurchaseCartModel _cartItem;

	public PurchaseItemManageWindow(PurchaseCartModel cartItem)
	{
		InitializeComponent();

		_cartItem = cartItem;
	}

	#region LoadData
	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var rawMaterial = await CommonData.LoadTableDataById<RawMaterialModel>(TableNames.RawMaterial, _cartItem.RawMaterialId);
		productNameTextBox.Text = rawMaterial.Name;
		productCodeTextBox.Text = rawMaterial.Code;
		productRateTextBox.Text = rawMaterial.MRP.ToString();

		quantityTextBox.Text = _cartItem.Quantity.ToString();
		discountPercentTextBox.Text = _cartItem.DiscPercent.ToString();

		cgstPercentTextBox.Text = _cartItem.CGSTPercent.ToString();
		sgstPercentTextBox.Text = _cartItem.SGSTPercent.ToString();
		igstPercentTextBox.Text = _cartItem.IGSTPercent.ToString();

		UpdateFinancialDetails();
	}

	private void UpdateFinancialDetails(bool assignValues = false)
	{
		// Base calculations
		decimal quantity = decimal.Parse(quantityTextBox.Text);
		decimal rate = decimal.Parse(productRateTextBox.Text);
		decimal baseTotal = rate * quantity;

		// Discount calculations
		decimal discPercent = decimal.Parse(discountPercentTextBox.Text);
		decimal discountAmount = baseTotal * (discPercent / 100);
		decimal afterDiscount = baseTotal - discountAmount;

		// Tax calculations
		decimal cgstPercent = decimal.Parse(cgstPercentTextBox.Text);
		decimal sgstPercent = decimal.Parse(sgstPercentTextBox.Text);
		decimal igstPercent = decimal.Parse(igstPercentTextBox.Text);

		decimal cgstAmount = afterDiscount * (cgstPercent / 100);
		decimal sgstAmount = afterDiscount * (sgstPercent / 100);
		decimal igstAmount = afterDiscount * (igstPercent / 100);

		// Total calculation
		decimal finalAmount = afterDiscount + cgstAmount + sgstAmount + igstAmount;

		// Display all values in the UI
		baseTotalTextBox.Text = baseTotal.FormatIndianCurrency();
		discountPercentTextBox.Text = discPercent.ToString();
		discountAmountTextBox.Text = discountAmount.FormatIndianCurrency();
		afterDiscountTextBox.Text = afterDiscount.FormatIndianCurrency();

		cgstPercentTextBox.Text = cgstPercent.ToString();
		sgstPercentTextBox.Text = sgstPercent.ToString();
		igstPercentTextBox.Text = igstPercent.ToString();

		cgstAmountTextBox.Text = cgstAmount.FormatIndianCurrency();
		sgstAmountTextBox.Text = sgstAmount.FormatIndianCurrency();
		igstAmountTextBox.Text = igstAmount.FormatIndianCurrency();

		finalAmountTextBox.Text = finalAmount.FormatIndianCurrency();

		// Update Cart Item
		if (assignValues)
		{
			_cartItem.Quantity = decimal.Parse(quantityTextBox.Text);
			_cartItem.Rate = rate;
			_cartItem.BaseTotal = baseTotal;
			_cartItem.DiscPercent = discPercent;
			_cartItem.DiscAmount = discountAmount;
			_cartItem.AfterDiscount = afterDiscount;
			_cartItem.CGSTPercent = cgstPercent;
			_cartItem.CGSTAmount = cgstAmount;
			_cartItem.SGSTPercent = sgstPercent;
			_cartItem.SGSTAmount = sgstAmount;
			_cartItem.IGSTPercent = igstPercent;
			_cartItem.IGSTAmount = igstAmount;
			_cartItem.Total = finalAmount;
		}
	}
	#endregion

	#region Quantity
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void quantityMinusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(-1);

	private void quantityPlusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(1);

	private void quantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
		UpdateQuantity(0);

	private void UpdateQuantity(decimal change)
	{
		if (_cartItem is null)
			return;

		decimal newQty = decimal.Parse(quantityTextBox.Text) + change;

		quantityTextBox.Text = Math.Max(0, newQty).ToString();

		UpdateFinancialDetails();
	}
	#endregion

	#region Discount
	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void percentTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		if (!decimal.TryParse(discountPercentTextBox.Text, out decimal discountPercent))
		{
			discountPercentTextBox.Text = "0.00";
			discountPercent = 0;
		}
		discountPercent = Math.Clamp(discountPercent, 0, 100);
		if (discountPercent != decimal.Parse(discountPercentTextBox.Text))
			discountPercentTextBox.Text = discountPercent.ToString();

		if (!decimal.TryParse(cgstPercentTextBox.Text, out decimal cgstPercent))
		{
			cgstPercentTextBox.Text = "0.00";
			cgstPercent = 0;
		}
		cgstPercent = Math.Clamp(cgstPercent, 0, 100);
		if (cgstPercent != decimal.Parse(cgstPercentTextBox.Text))
			cgstPercentTextBox.Text = cgstPercent.ToString();

		if (!decimal.TryParse(sgstPercentTextBox.Text, out decimal sgstPercent))
		{
			sgstPercentTextBox.Text = "0.00";
			sgstPercent = 0;
		}
		sgstPercent = Math.Clamp(sgstPercent, 0, 100);
		if (sgstPercent != decimal.Parse(sgstPercentTextBox.Text))
			sgstPercentTextBox.Text = sgstPercent.ToString();

		if (!decimal.TryParse(igstPercentTextBox.Text, out decimal igstPercent))
		{
			igstPercentTextBox.Text = "0.00";
			igstPercent = 0;
		}
		igstPercent = Math.Clamp(igstPercent, 0, 100);
		if (igstPercent != decimal.Parse(igstPercentTextBox.Text))
			igstPercentTextBox.Text = igstPercent.ToString();

		UpdateFinancialDetails();
	}

	#endregion

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		UpdateFinancialDetails(true);

		DialogResult = true;
		Close();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}