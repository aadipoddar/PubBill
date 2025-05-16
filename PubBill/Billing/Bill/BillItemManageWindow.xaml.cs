using System.Windows;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for BillItemManageWindow.xaml
/// </summary>
public partial class BillItemManageWindow : Window
{
	private readonly CartModel _cartItem;
	private readonly bool _kotCart;
	private readonly decimal _discountPercent;

	public BillItemManageWindow(CartModel cartItem, decimal discountPercent, bool kotCart = true)
	{
		InitializeComponent();

		_cartItem = cartItem;
		_kotCart = kotCart;
		_discountPercent = discountPercent;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		quantityTextBox.Text = _cartItem.Quantity.ToString();
		instructionsTextBox.Text = _cartItem.Instruction;
		discountableCheckBox.IsChecked = _cartItem.Discountable;
		cancelledCheckBox.IsChecked = _cartItem.Cancelled;

		instructionsTextBox.Focus();

		if (!_kotCart)
			instructionsTextBox.IsReadOnly = true;

		await UpdateFinancialDetails();
	}

	private async Task UpdateFinancialDetails()
	{
		var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, _cartItem.ProductId);
		var productTax = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, _cartItem.ProductId);

		productNameTextBox.Text = product.Name;
		productCodeTextBox.Text = product.Code;
		productRateTextBox.Text = product.Rate.FormatIndianCurrency();

		decimal quantity = decimal.Parse(quantityTextBox.Text);
		decimal rate = _cartItem.Rate;

		// Base calculations
		decimal baseTotal = rate * quantity;
		decimal discountAmount = _cartItem.Discountable ? baseTotal * (_discountPercent / 100) : 0;
		decimal afterDiscount = baseTotal - discountAmount;

		// Tax calculations
		decimal cgstPercent = productTax.CGSTPercent;
		decimal sgstPercent = productTax.SGSTPercent;
		decimal igstPercent = productTax.IGSTPercent;

		decimal cgstAmount = afterDiscount * (cgstPercent / 100);
		decimal sgstAmount = afterDiscount * (sgstPercent / 100);
		decimal igstAmount = afterDiscount * (igstPercent / 100);

		// Total calculation
		decimal finalAmount = afterDiscount + cgstAmount + sgstAmount + igstAmount;

		// Display all values in the UI
		baseTotalTextBox.Text = baseTotal.FormatIndianCurrency();
		discountPercentTextBox.Text = $"{_discountPercent:F2}%";
		discountAmountTextBox.Text = discountAmount.FormatIndianCurrency();
		afterDiscountTextBox.Text = afterDiscount.FormatIndianCurrency();

		cgstPercentTextBox.Text = $"{cgstPercent:F2}%";
		sgstPercentTextBox.Text = $"{sgstPercent:F2}%";

		cgstAmountTextBox.Text = cgstAmount.FormatIndianCurrency();
		sgstAmountTextBox.Text = sgstAmount.FormatIndianCurrency();

		finalAmountTextBox.Text = finalAmount.FormatIndianCurrency();
	}

	#region Quantity
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void quantityMinusButton_Click(object sender, RoutedEventArgs e) =>
		await UpdateQuantity(-1);

	private async void quantityPlusButton_Click(object sender, RoutedEventArgs e) =>
		await UpdateQuantity(1);

	private async void quantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
		await UpdateQuantity(0);

	private async Task UpdateQuantity(int change)
	{
		if (_cartItem is null)
			return;

		int newQty = int.Parse(quantityTextBox.Text) + change;

		if (!_kotCart)
			quantityTextBox.Text = Math.Min(Math.Max(0, newQty), _cartItem.Quantity).ToString();

		else
			quantityTextBox.Text = Math.Max(0, newQty).ToString();

		await UpdateFinancialDetails();
	}
	#endregion

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		_cartItem.Quantity = int.Parse(quantityTextBox.Text);
		_cartItem.Instruction = instructionsTextBox.Text;
		_cartItem.Discountable = discountableCheckBox.IsChecked ?? false;
		_cartItem.Cancelled = cancelledCheckBox.IsChecked ?? false;

		DialogResult = true;
		Close();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}
