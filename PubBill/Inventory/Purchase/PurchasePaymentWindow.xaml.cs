using System.Collections.ObjectModel;
using System.Windows;

namespace PubBill.Inventory.Purchase;

/// <summary>
/// Interaction logic for PurchasePaymentWindow.xaml
/// </summary>
public partial class PurchasePaymentWindow : Window
{
	private readonly ObservableCollection<PurchaseCartModel> _cart;
	private readonly List<BillPaymentModeModel> _paymentModels = [];
	private readonly PurchaseModel _purchaseModel;
	private readonly PurchaseWindow _purchaseWindow;
	private decimal _totalAmount;

	public PurchasePaymentWindow(PurchaseModel purchaseModel, ObservableCollection<PurchaseCartModel> cart, PurchaseWindow purchaseWindow)
	{
		InitializeComponent();

		_purchaseModel = purchaseModel;
		_cart = cart;
		_purchaseWindow = purchaseWindow;

		amountDataGrid.ItemsSource = _paymentModels;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentMode);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;

		_totalAmount = _cart.Sum(x => x.Total) - _purchaseModel.CDAmount;
		totalTextBox.Text = _totalAmount.FormatIndianCurrency();

		CalculateTotal();
	}

	private void CalculateTotal()
	{
		amountDataGrid.Items.Refresh();

		var collected = _paymentModels.Sum(x => x.Amount);
		collectedTextBox.Text = collected.FormatIndianCurrency();

		var remaining = _totalAmount - collected;

		amountTextBox.Text = Math.Round(remaining, 2).ToString();

		if (remaining == 0)
			saveButton.IsEnabled = true;
		else
			saveButton.IsEnabled = false;
	}

	#region DataGridEvents
	private void addButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(amountTextBox.Text)) return;
		var selectedPaymentMode = paymentModeComboBox.SelectedItem as PaymentModeModel;

		if (_paymentModels.Any(x => x.PaymentModeId == selectedPaymentMode.Id))
			_paymentModels.FirstOrDefault(x => x.PaymentModeId == selectedPaymentMode.Id).Amount += decimal.Parse(amountTextBox.Text);

		else
			_paymentModels.Add(new BillPaymentModeModel
			{
				PaymentModeId = selectedPaymentMode.Id,
				PaymentModeName = selectedPaymentMode.Name,
				Amount = decimal.Parse(amountTextBox.Text)
			});

		CalculateTotal();
	}

	private void amountTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void amountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		var collected = _paymentModels.Sum(x => x.Amount);
		var remaining = _totalAmount - collected;

		if (string.IsNullOrEmpty(amountTextBox.Text)) return;

		if (decimal.Parse(amountTextBox.Text) > remaining)
			CalculateTotal();
	}

	private void amountDataGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (amountDataGrid.SelectedItem is BillPaymentModeModel selectedPayment)
			_paymentModels.Remove(selectedPayment);
		CalculateTotal();
	}
	#endregion

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
