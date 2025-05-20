using System.Windows;

namespace PubBill.Inventory.Purchase;

public partial class PurchaseHistoryWindow : Window
{
	public PurchaseHistoryWindow() =>
		InitializeComponent();

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		purchaseDatePicker.SelectedDate = DateTime.Now;
		await LoadPurchaseData();
	}

	private async void purchaseDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadPurchaseData();

	private async Task LoadPurchaseData() =>
		purchaseDataGrid.ItemsSource = await PurchaseData.LoadPurchaseOverviewByDate(DateOnly.FromDateTime(purchaseDatePicker.SelectedDate.Value));

	private async void purchaseDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.OriginalSource is FrameworkElement element && element.DataContext is PurchaseOverviewModel model)
		{
			PurchaseWindow purchaseWindow = new(model);
			purchaseWindow.ShowDialog();
		}

		await LoadPurchaseData();
	}
}