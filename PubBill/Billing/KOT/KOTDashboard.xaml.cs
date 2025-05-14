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
		var selectedLocationIds = locationCheckBoxStackPanel.Children.OfType<CheckBox>()
			.Where(c => (bool)c.IsChecked)
			.Select(c =>
			{
				string name = c.Name;
				string idPart = name[..^8];
				idPart = new string([.. idPart.Reverse().TakeWhile(char.IsDigit).Reverse()]);

				if (int.TryParse(idPart, out int id))
					return id;
				return -1;
			})
			.Where(id => id > 0)
			.ToList();

		foreach (var bill in runningBills)
		{
			if (bill is null) continue;
			if (!selectedLocationIds.Contains(bill.LocationId)) continue;

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
		var printerName = await GetPrinterName(kotOrder);
		var printers = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList();

		if (string.IsNullOrEmpty(printerName) || !printers.Contains(printerName))
		{
			MessageBox.Show($"Printer not found: {printerName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		PrintDialog printDialog = new()
		{
			PrintQueue = new System.Printing.LocalPrintServer().GetPrintQueue(printerName)
		};

		IDocumentPaginatorSource idpSource = await ThermalKOTReceipt.Print(kotOrder);
		printDialog.PrintDocument(idpSource.DocumentPaginator, "KOT Receipt");
	}

	private static async Task<string> GetPrinterName(KOTBillDetailModel kotOrder)
	{
		var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, kotOrder.ProductId);
		var runningBill = await CommonData.LoadTableDataById<RunningBillModel>(TableNames.RunningBill, kotOrder.RunningBillId);
		var diningArea = await CommonData.LoadTableDataById<DiningAreaModel>(TableNames.DiningArea, runningBill.DiningAreaId);
		var diningAreaKitchens = (await CommonData.LoadTableData<DiningAreaKitchenModel>(TableNames.DiningAreaKitchen))
					.Where(x => x.DiningAreaId == diningArea.Id).ToList();
		var allKitchens = await CommonData.LoadTableDataByStatus<KitchenModel>(TableNames.Kitchen);

		foreach (var diningAreaKitchen in diningAreaKitchens)
		{
			var matchingKitchen = allKitchens
				.Where(k => k.Id == diningAreaKitchen.KitchenId && k.KitchenTypeId == product.KitchenTypeId && k.Status)
				.FirstOrDefault();

			if (matchingKitchen is not null && !string.IsNullOrEmpty(matchingKitchen.PrinterName))
			{
				return matchingKitchen.PrinterName;
			}
		}

		MessageBox.Show($"No matching printer found for {product.Name} in {diningArea.Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		return string.Empty;
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
