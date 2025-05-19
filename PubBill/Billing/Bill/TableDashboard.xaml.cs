using System.Windows;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for TableDashboard.xaml
/// </summary>
public partial class TableDashboard : Window
{
	#region Timers
	private static int RefreshTimer => (int)Application.Current.Resources[SettingsKeys.RefreshBillTimer];

	private SmartRefreshManager _refreshManager;

	private void InitializeTimers()
	{
		_refreshManager = new SmartRefreshManager(
			refreshAction: RefreshScreen,
			interval: TimeSpan.FromSeconds(RefreshTimer)
		);
		_refreshManager.Start();
	}

	#endregion

	private readonly UserModel _user;
	private readonly LocationModel _location;
	private readonly LoginWindow _loginWindow;

	public TableDashboard(UserModel user, LocationModel location, LoginWindow loginWindow)
	{
		InitializeComponent();
		_user = user;
		_location = location;
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();

		if (_user is null)
		{
			MessageBox.Show("User not Valid. Please contact the administrator.", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
		}

		userTextBlock.Text = _user.Name;
		locationTextBlock.Text = _location.Name;

		await RefreshScreen();
		_loginWindow.Hide();
	}

	public async Task RefreshScreen()
	{
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();

			dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
			await CreateComponents.CreateDiningAreaExpanders(areasStackPanel, _user, _location, this);
		}
		finally
		{
			// Always make sure to end the refresh operation, even if an error occurs
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_refreshManager.Stop();
		_refreshManager.Dispose();
		_loginWindow.Show();
	}

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void billButton_Click(object sender, RoutedEventArgs e)
	{
		var billModel = await CommonData.LoadTableDataById<BillModel>(TableNames.Bill, int.Parse(billNoTextBox.Text));
		if (billModel is null)
		{
			MessageBox.Show("Bill not found. Please check the bill number.", "Bill Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (billModel.LocationId != _location.Id)
		{
			MessageBox.Show("Bill not found in this location. Please check the bill number.", "Bill Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (billModel.UserId != _user.Id && !_user.Admin)
		{
			MessageBox.Show("This Bill was done by another user.", "Unauthorized Access", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var runningTables = await CommonData.LoadTableDataByStatus<RunningBillModel>(TableNames.RunningBill);
		runningTables = [.. runningTables.Where(x => x.DiningTableId == billModel.DiningTableId)];
		if (runningTables.Count > 0)
		{
			MessageBox.Show("This Table has a runnning Order Please Close it first.", "Table Already Running", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		RunningBillModel runningBillModel = await StartRunningBill(billModel);
		await SetPaymentDetailsFalse(billModel);

		BillWindow billWindow = new(this, runningBillModel);
		billWindow.Show();
		Hide();
	}

	private static async Task SetPaymentDetailsFalse(BillModel billModel)
	{
		var billPaymentDetails = await BillData.LoadBillPaymentDetailByBillId(billModel.Id);
		billPaymentDetails = [.. billPaymentDetails.Where(x => x.Status)];

		foreach (var item in billPaymentDetails)
			await BillData.InsertBillPaymentDetail(new BillPaymentDetailModel
			{
				Id = item.Id,
				BillId = item.BillId,
				Amount = item.Amount,
				PaymentModeId = item.PaymentModeId,
				Status = false
			});
	}

	private static async Task<RunningBillModel> StartRunningBill(BillModel billModel)
	{
		var billDetails = await BillData.LoadBillDetailByBillId(billModel.Id);
		billDetails = [.. billDetails.Where(x => x.Status)];

		var runningBillModel = new RunningBillModel()
		{
			Id = 0,
			UserId = billModel.UserId,
			LocationId = billModel.LocationId,
			DiningAreaId = billModel.DiningAreaId,
			DiningTableId = billModel.DiningTableId,
			PersonId = billModel.PersonId,
			TotalPeople = billModel.TotalPeople,
			DiscPercent = billModel.DiscPercent,
			DiscReason = billModel.DiscReason,
			ServicePercent = billModel.ServicePercent,
			Remarks = billModel.Remarks,
			BillStartDateTime = billModel.BillDateTime,
			BillId = billModel.Id,
			Status = true
		};

		runningBillModel.Id = await RunningBillData.InsertRunningBill(runningBillModel);

		foreach (var item in billDetails)
		{
			item.Status = false;
			await BillData.InsertBillDetail(item);

			await RunningBillData.InsertRunningBillDetail(new RunningBillDetailModel()
			{
				Id = 0,
				RunningBillId = runningBillModel.Id,
				ProductId = item.ProductId,
				Quantity = item.Quantity,
				Rate = item.Rate,
				Instruction = item.Instruction,
				Cancelled = false
			});
		}

		return runningBillModel;
	}
}
