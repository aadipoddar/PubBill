using System.Collections.ObjectModel;
using System.Windows;

namespace PubBill.Billing.KOT;

/// <summary>
/// Interaction logic for RunningKOTWindow.xaml
/// </summary>
public partial class RunningKOTWindow : Window
{
	#region Timers
	private static int RefreshTimer => (int)Application.Current.Resources[SettingsKeys.RefreshKOTTimer];

	private SmartRefreshManager _refreshManager;

	private void InitializeTimers()
	{
		_refreshManager = new SmartRefreshManager(
			refreshAction: RefreshScreen,
			interval: TimeSpan.FromMinutes(RefreshTimer)
		);
		_refreshManager.Start();
	}

	#endregion

	private readonly KOTDashboard _kotDashboard;
	private readonly DiningTableModel _table;
	private readonly RunningBillModel _runningTable;
	private static readonly ObservableCollection<CartModel> _allCart = [];

	public RunningKOTWindow(KOTDashboard kOTDashboard, DiningTableModel table, RunningBillModel runningTable)
	{
		InitializeComponent();

		_kotDashboard = kOTDashboard;
		_table = table;
		_runningTable = runningTable;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		InitializeTimers();

		var diningArea = await CommonData.LoadTableDataById<DiningAreaModel>(TableNames.DiningArea, _table.DiningAreaId);

		diningAreaTextBox.Text = diningArea.Name;
		diningTableTextBox.Text = _table.Name;

		var runningTime = DateTime.Now - _runningTable.BillStartDateTime;
		runningTimeTextBox.Text = runningTime.ToString("hh\\:mm");

		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, _runningTable.UserId);
		userTextBox.Text = user.Name;

		await LoadDataGrid();
	}

	private async Task LoadDataGrid()
	{
		_allCart.Clear();

		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningTable.Id);
		if (runningBillDetails is null || runningBillDetails.Count == 0) Close();

		foreach (var billDetail in runningBillDetails)
		{
			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, billDetail.ProductId);
			_allCart.Add(new CartModel()
			{
				ProductId = billDetail.ProductId,
				ProductName = product.Name,
				Quantity = billDetail.Quantity,
				Rate = product.Rate,
				Instruction = billDetail.Instruction,
				Cancelled = billDetail.Cancelled
			});
		}

		cartDataGrid.ItemsSource = _allCart;
	}

	public async Task RefreshScreen()
	{
		try
		{
			InactivityMonitor.Instance.BeginRefreshOperation();
			await LoadDataGrid();
		}
		finally
		{
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_refreshManager.Stop();
		_refreshManager.Dispose();
		_kotDashboard.Show();
	}
}
