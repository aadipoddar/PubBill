using System.Windows;

using PubBill.Admin.Dining;
using PubBill.Admin.Inventory;
using PubBill.Admin.Kitchen;
using PubBill.Admin.Product;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for AdminPanel.xaml
/// </summary>
public partial class AdminPanel : Window
{
	private readonly LoginWindow _loginWindow;

	public AdminPanel(LoginWindow loginWindow)
	{
		InitializeComponent();
		_loginWindow = loginWindow;
	}

	private void Window_Loaded(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductGroupPage();

	private void manageDiningAreasButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new DiningAreaPage();
	private void manageDiningTablesButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new DiningTablePage();
	private void manageDiningAreaKitchenButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new DiningAreaKitchenPage();

	private void manageKitchenTypesButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new KitchenTypePage();
	private void manageKitchenButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new KitchenPage();

	private void manageProductGroupButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductGroupPage();
	private void manageProductButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductPage();
	private void manageProductCategoryButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductCategoryPage();

	private void manageRawMaterialCategoryButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new RawMaterialCategoryPage();
	private void manageRawMaterialButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new RawMaterialPage();

	private void manageUsersButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new UserPage();
	private void manageLocationsButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new LocationPage();
	private void managePersonButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new PersonPage();
	private void managePaymentModeButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new PaymentModePage();
	private void manageTaxButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new TaxPage();

	private void sqlEditorButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new SqlEditorPage();
	private void settingsButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new SettingsPage();


	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}
