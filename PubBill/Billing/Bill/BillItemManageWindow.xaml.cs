using System.Windows;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for BillItemManageWindow.xaml
/// </summary>
public partial class BillItemManageWindow : Window
{
	private readonly CartModel _cartItem;
	private readonly bool _kotCart;
	private decimal _discountPercent;
	private readonly decimal _originalDiscountPercent;

	public BillItemManageWindow(CartModel cartItem, decimal discountPercent, bool kotCart = true)
	{
		InitializeComponent();

		_cartItem = cartItem;
		_kotCart = kotCart;
		_discountPercent = discountPercent;
		_originalDiscountPercent = discountPercent;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		quantityTextBox.Text = _cartItem.Quantity.ToString();
		instructionsTextBox.Text = _cartItem.Instruction;
		discountableCheckBox.IsChecked = _cartItem.Discountable;
		cancelledCheckBox.IsChecked = _cartItem.Cancelled;
		selfDiscountCheckBox.IsChecked = _cartItem.SelfDiscount;
		discountPercentTextBox.Text = _cartItem.DiscPercent.ToString("N2");

		instructionsTextBox.Focus();

		if (!_kotCart)
			instructionsTextBox.IsReadOnly = true;

		await UpdateFinancialDetails();
	}

	private async Task UpdateFinancialDetails(bool assignValues = false)
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
		_discountPercent = _cartItem.SelfDiscount ? _discountPercent : _originalDiscountPercent;
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
		discountPercentTextBox.Text = _discountPercent.ToString();
		discountAmountTextBox.Text = discountAmount.FormatIndianCurrency();
		afterDiscountTextBox.Text = afterDiscount.FormatIndianCurrency();

		cgstPercentTextBox.Text = $"{cgstPercent:N2}%";
		sgstPercentTextBox.Text = $"{sgstPercent:N2}%";

		cgstAmountTextBox.Text = cgstAmount.FormatIndianCurrency();
		sgstAmountTextBox.Text = sgstAmount.FormatIndianCurrency();

		finalAmountTextBox.Text = finalAmount.FormatIndianCurrency();

		// Update Cart Item
		if (assignValues)
		{
			_cartItem.Quantity = int.Parse(quantityTextBox.Text);
			_cartItem.Rate = rate;
			_cartItem.BaseTotal = baseTotal;
			_cartItem.Instruction = instructionsTextBox.Text;
			_cartItem.Discountable = discountableCheckBox.IsChecked ?? false;
			_cartItem.SelfDiscount = selfDiscountCheckBox.IsChecked ?? false;
			_cartItem.DiscPercent = _discountPercent;
			_cartItem.DiscAmount = discountAmount;
			_cartItem.AfterDiscount = afterDiscount;
			_cartItem.CGSTPercent = cgstPercent;
			_cartItem.CGSTAmount = cgstAmount;
			_cartItem.SGSTPercent = sgstPercent;
			_cartItem.SGSTAmount = sgstAmount;
			_cartItem.IGSTPercent = igstPercent;
			_cartItem.IGSTAmount = igstAmount;
			_cartItem.Total = finalAmount;
			_cartItem.Cancelled = cancelledCheckBox.IsChecked ?? false;
		}
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

	#region Discount
	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private async void discountPercentTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		selfDiscountCheckBox.IsChecked = _cartItem.SelfDiscount = true;

		if (decimal.Parse(discountPercentTextBox.Text) == _originalDiscountPercent)
			selfDiscountCheckBox.IsChecked = _cartItem.SelfDiscount = false;

		if (!decimal.TryParse(discountPercentTextBox.Text, out decimal percent))
		{
			discountPercentTextBox.Text = "0.00";
			percent = 0;
		}

		percent = Math.Clamp(percent, 0, 100);
		if (percent != decimal.Parse(discountPercentTextBox.Text))
			discountPercentTextBox.Text = percent.ToString("N2");

		_discountPercent = percent;

		await UpdateFinancialDetails();
	}

	private async void selfDiscountCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		_discountPercent = _originalDiscountPercent;
		await UpdateFinancialDetails();
	}

	private async void selfDiscountCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		_discountPercent = _originalDiscountPercent;
		await UpdateFinancialDetails();
	}
	#endregion

	private async void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		await UpdateFinancialDetails(true);

		DialogResult = true;
		Close();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}
