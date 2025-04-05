using System.Windows;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for AdminPanel.xaml
/// </summary>
public partial class AdminPanel : Window
{
	private readonly Dashboard _dashboard;

	public AdminPanel(Dashboard dashboard)
	{
		InitializeComponent();
		_dashboard = dashboard;
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_dashboard.Show();
		Close();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductGroupPage();

	private void manageProductGroupButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductGroupPage();

	private void manageProductButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductPage();

	private void manageProductCategoryButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new ProductCategoryPage();

	private void manageDiningAreasButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new DiningAreaPage();

	private void manageDiningTablesButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new DiningTablePage();

	private void manageUsersButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new UserPage();

	private void manageLocationsButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new LocationPage();

	private void managePersonButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new PersonPage();

	private void managePaymentModeButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new PaymentModePage();

	private void sqlEditorButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new SqlEditorPage();

	private void settingsButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new SettingsPage();
}
