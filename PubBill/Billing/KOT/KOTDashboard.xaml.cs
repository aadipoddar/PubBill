using System.Windows;

namespace PubBill.Billing.KOT;

/// <summary>
/// Interaction logic for KOTWindow.xaml
/// </summary>
public partial class KOTDashboard : Window
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

	private readonly LoginWindow _loginWindow;

	public KOTDashboard(LoginWindow loginWindow)
	{
		InitializeComponent();
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();
		await RefreshScreen();
		_loginWindow.Hide();
	}

	public async Task RefreshScreen()
	{
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();
			await CreateKOTComponents.CreateLocationExpanders(areasStackPanel, this);
			await PrintOrders();
		}
		finally
		{
			// Always make sure to end the refresh operation, even if an error occurs
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	private static async Task PrintOrders()
	{
		var runningBills = await CommonData.LoadTableDataByStatus<RunningBillModel>(TableNames.RunningBill);

		foreach (var bill in runningBills)
		{
			if (bill is null) continue;

			var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(bill.Id);
			foreach (var kotOrder in kotOrders)
			{
				// TODO - Print
				await ChangeKOTBillStatus(kotOrder);
			}
		}
	}

	private static async Task ChangeKOTBillStatus(KOTBillDetailModel kOTBillDetail)
	{
		await KOTData.InsertKOTBillDetail(new KOTBillDetailModel()
		{
			Id = kOTBillDetail.Id,
			RunningBillId = kOTBillDetail.RunningBillId,
			ProductId = kOTBillDetail.ProductId,
			Quantity = kOTBillDetail.Quantity,
			Instruction = kOTBillDetail.Instruction,
			Status = false
		});
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_refreshManager.Stop();
		_refreshManager.Dispose();
		_loginWindow.Show();
	}
}
