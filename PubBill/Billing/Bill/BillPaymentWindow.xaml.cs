using System.Collections.ObjectModel;
using System.Windows;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for BillPaymentWindow.xaml
/// </summary>
public partial class BillPaymentWindow : Window
{
	private readonly BillWindow _billWindow;
	private readonly BillModel _billModel;
	private readonly RunningBillModel _runningBillModel;
	private static ObservableCollection<CartModel> _allCart;
	private readonly List<BillPaymentModeModel> _paymentModels = [];
	private decimal _totalAmount;

	public BillPaymentWindow(BillWindow billWindow, BillModel billModel, RunningBillModel runningBillModel, ObservableCollection<CartModel> allCart)
	{
		InitializeComponent();

		_billWindow = billWindow;
		_billModel = billModel;
		_runningBillModel = runningBillModel;
		_allCart = allCart;

		amountDataGrid.ItemsSource = _paymentModels;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentMode);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;

		_totalAmount = await BillWindowHelper.CalculateBillTotal(_allCart, _runningBillModel);
		totalTextBox.Text = _totalAmount.FormatIndianCurrency();

		CalculateTotal();
	}

	#region DataGridEvents
	private void addButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(amountTextBox.Text)) return;
		var selectedPaymentMode = paymentModeComboBox.SelectedItem as PaymentModeModel;

		if (_paymentModels.Any(x => x.PaymentModeId == selectedPaymentMode.Id))
			_paymentModels.FirstOrDefault(x => x.PaymentModeId == selectedPaymentMode.Id).Amount += decimal.Parse(amountTextBox.Text);

		else _paymentModels.Add(new BillPaymentModeModel
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

	#region Saving
	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		int billId = await BillData.InsertBill(_billModel);
		if (billId == 0)
		{
			MessageBox.Show("Failed to insert bill data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		await InsertBillDetails(billId);

		await InsertPaymentDetails(billId);

		// Change Running Table Status
		await RunningBillData.InsertRunningBill(_runningBillModel);
		Close();
	}

	private static async Task InsertBillDetails(int billId)
	{
		foreach (var cart in _allCart)
		{
			var productDetail = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, cart.ProductId);
			await BillData.InsertBillDetail(new BillDetailModel
			{
				Id = 0,
				BillId = billId,
				ProductId = cart.ProductId,
				Quantity = cart.Quantity,
				Rate = cart.Rate,
				CGST = productDetail.CGSTPercent,
				SGST = productDetail.SGSTPercent,
				IGST = productDetail.IGSTPercent,
				Instruction = cart.Instruction,
				Cancelled = cart.Cancelled,
				Status = true
			});
		}
	}

	private async Task InsertPaymentDetails(int billId)
	{
		foreach (var payment in _paymentModels)
			await BillData.InsertBillPaymentDetail(new BillPaymentDetailModel
			{
				Id = 0,
				BillId = billId,
				PaymentModeId = payment.PaymentModeId,
				Amount = payment.Amount,
				Status = true
			});
	}
	#endregion

	private void Window_Closed(object sender, EventArgs e) =>
		_billWindow.Close();
}
