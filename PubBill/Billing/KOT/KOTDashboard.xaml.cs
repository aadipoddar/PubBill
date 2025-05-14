using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PubBill.Billing.KOT;

/// <summary>
/// Interaction logic for KOTWindow.xaml
/// </summary>
public partial class KOTDashboard : Window
{
	#region Timers

	private static int RefreshTimer => (int)Application.Current.Resources[SettingsKeys.RefreshKOTTimer];

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

	private readonly LoginWindow _loginWindow;

	public KOTDashboard(LoginWindow loginWindow)
	{
		InitializeComponent();
		_loginWindow = loginWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		await CreateKOTComponents.CreateLocationCheckBoxes(locationCheckBoxStackPanel, this);
		await CreateKOTComponents.CreateLocationExpanders(areasStackPanel, locationCheckBoxStackPanel, this);
		await PrintOrders();

		InitializeTimers();
		await RefreshScreen();

		_loginWindow.Hide();
	}

	public async Task RefreshScreen()
	{
		try
		{
			InactivityMonitor.Instance.BeginRefreshOperation();

			await CreateKOTComponents.CreateLocationExpanders(areasStackPanel, locationCheckBoxStackPanel, this);
			await PrintOrders();
		}
		finally
		{
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	private async Task PrintOrders()
	{
		_refreshManager?.Stop();
		var runningBills = await CommonData.LoadTableDataByStatus<RunningBillModel>(TableNames.RunningBill);

		foreach (var bill in runningBills)
		{
			if (bill is null) continue;

			var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(bill.Id);
			kotOrders = [.. kotOrders.Where(x => x.Status)];

			foreach (var kotOrder in kotOrders)
			{
				await PrintKOT(kotOrder);
				await ChangeKOTBillStatus(kotOrder);
			}
		}
		_refreshManager?.Start();
	}

	private static async Task PrintKOT(KOTBillDetailModel kotOrder)
	{
		PrintDialog printDialog = new();
		IDocumentPaginatorSource idpSource = await ThermalKOTReceipt.Print(kotOrder);
		printDialog.PrintDocument(idpSource.DocumentPaginator, "KOT Receipt");
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
			Status = false,
			Cancelled = kOTBillDetail.Cancelled
		});
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_refreshManager?.Stop();
		_refreshManager?.Dispose();
		_loginWindow.Show();
	}
}
