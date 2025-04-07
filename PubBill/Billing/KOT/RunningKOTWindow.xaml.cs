using System.Windows;

namespace PubBill.Billing.KOT
{
	/// <summary>
	/// Interaction logic for RunningKOTWindow.xaml
	/// </summary>
	public partial class RunningKOTWindow : Window
	{
		public RunningKOTWindow(KOTDashboard kOTDashboard, DiningTableModel table, RunningBillModel runningTable)
		{
			InitializeComponent();
		}
	}
}
