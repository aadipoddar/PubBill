using System.Windows;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for TableDashboard.xaml
/// </summary>
public partial class TableDashboard : Window
{
	#region Timers

	private SmartRefreshManager _refreshManager;

	private void InitializeTimers()
	{
		_refreshManager = new SmartRefreshManager(
			refreshAction: RefreshScreen,
			interval: TimeSpan.FromSeconds(20)
		);
		_refreshManager.Start();
	}

	#endregion

	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public TableDashboard(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();
		_user = user;
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();

		if (_user is null)
		{
			MessageBox.Show("User not Valid. Please contact the administrator.", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
			Close();
		}

		userTextBlock.Text = _user.Name;

		var location = await CommonData.LoadTableDataById<LocationModel>(TableNames.Location, _user.LocationId);
		locationTextBlock.Text = location.Name;

		_loginWindow.Hide();

		await RefreshScreen();
	}

	public async Task RefreshScreen()
	{
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();

			dateTimeTextBlock.Text = DateTime.Now.ToString("HH:mm tt");
			await CreateComponents.CreateDiningAreaExpanders(areasStackPanel, _user, this);
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

		#region Start New Running Bill And Status False
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
		#endregion

		#region Set PaymentDetails False
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
		#endregion

		BillWindow billWindow = new(this, runningBillModel);
		billWindow.Show();
		Hide();
	}
}
