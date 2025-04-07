using System.Windows;

namespace PubBill.Billing.KOT;

/// <summary>
/// Interaction logic for RunningKOTWindow.xaml
/// </summary>
public partial class RunningKOTWindow : Window
{
	private readonly KOTDashboard _kotDashboard;
	private readonly DiningTableModel _table;
	private readonly RunningBillModel _runningTable;

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
		cartDataGrid.Items.Clear();

		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningTable.Id);
		cartDataGrid.ItemsSource = runningBillDetails;
	}

	private void Window_Closed(object sender, EventArgs e) => _kotDashboard.Show();
}
