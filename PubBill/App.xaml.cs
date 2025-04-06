using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Win32;

namespace PubBill;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public static bool IsLightTheme { get; private set; } = GetIsLightTheme();
	public static string FooterVersionText { get; set; } = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";

	// Activity monitoring properties
	public static DateTime LastActivityTime { get; private set; } = DateTime.Now;
	private static DispatcherTimer _activityTimer;
	private static TimeSpan _inactivityThreshold = TimeSpan.FromSeconds(60);
	private static TimeSpan _warningThreshold = TimeSpan.FromSeconds(20);
	private static bool _warningDisplayed = false;
	private static Window _warningWindow;

	protected override void OnStartup(StartupEventArgs e)
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Secrets.SyncfusionLicense);

		EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
			new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
		EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
			new RoutedEventHandler(SelectAllText));
		EventManager.RegisterClassHandler(typeof(TextBox), Control.MouseDoubleClickEvent,
			new RoutedEventHandler(SelectAllText));

		// Initialize activity monitoring
		InitializeActivityMonitoring();

		base.OnStartup(e);
	}

	private void InitializeActivityMonitoring()
	{
		// Create and configure the inactivity timer
		_activityTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(10) // Check every 10 second
		};
		_activityTimer.Tick += CheckForInactivity;
		_activityTimer.Start();

		// Register global activity event handlers
		EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseMoveEvent,
			new MouseEventHandler(ActivityDetected));
		EventManager.RegisterClassHandler(typeof(UIElement), UIElement.KeyDownEvent,
			new KeyEventHandler(ActivityDetected));
		EventManager.RegisterClassHandler(typeof(UIElement), UIElement.TouchDownEvent,
			new EventHandler<TouchEventArgs>(ActivityDetected));
		EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseDownEvent,
			new MouseButtonEventHandler(ActivityDetected));
	}

	private void ActivityDetected(object sender, RoutedEventArgs e)
	{
		ResetActivityTimer();
	}

	public static void ResetActivityTimer()
	{
		LastActivityTime = DateTime.Now;

		// If warning window is shown, close it
		if (_warningDisplayed && _warningWindow != null)
		{
			_warningDisplayed = false;
			_warningWindow.Close();
			_warningWindow = null;
		}
	}

	private void CheckForInactivity(object sender, EventArgs e)
	{
		// Skip inactivity check if the active window is the login window
		if (IsLoginWindowActive())
		{
			ResetActivityTimer(); // Keep resetting timer while on login screen
			return;
		}

		TimeSpan inactiveTime = DateTime.Now - LastActivityTime;

		// If we're close to the inactivity threshold but not yet at it
		if (inactiveTime > (_inactivityThreshold - _warningThreshold) && !_warningDisplayed)
		{
			DisplayInactivityWarning();
		}
		// If we've reached the inactivity threshold
		else if (inactiveTime >= _inactivityThreshold)
		{
			HandleInactivity();
		}
	}

	// Check if the active window is the login window
	private bool IsLoginWindowActive()
	{
		var activeWindow = Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
		return activeWindow is LoginWindow;
	}

	private void DisplayInactivityWarning()
	{
		// Skip warning if the active window is the login window
		if (IsLoginWindowActive())
		{
			return;
		}

		_warningDisplayed = true;

		// Create a warning window
		_warningWindow = new Window
		{
			Title = "Inactivity Warning",
			Width = 400,
			Height = 150,
			WindowStyle = WindowStyle.ToolWindow,
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Topmost = true,
			ResizeMode = ResizeMode.NoResize
		};

		// Create content
		var grid = new Grid();
		var textBlock = new TextBlock
		{
			Text = $"You have been inactive for some time.\nThe application will close in {_warningThreshold.TotalSeconds} seconds unless you interact with it.",
			TextAlignment = TextAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(20)
		};
		grid.Children.Add(textBlock);
		_warningWindow.Content = grid;

		// Show the warning
		_warningWindow.Show();

		// Add interaction events to the warning window
		_warningWindow.MouseMove += (s, e) => ResetActivityTimer();
		_warningWindow.KeyDown += (s, e) => ResetActivityTimer();
	}

	private void HandleInactivity()
	{
		// Skip inactivity handling if the active window is the login window
		if (IsLoginWindowActive())
		{
			return;
		}

		// Close any warning window that might be open
		if (_warningWindow != null && _warningWindow.IsVisible)
		{
			_warningWindow.Close();
			_warningWindow = null;
		}

		_warningDisplayed = false;

		var mainWindow = Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

		if (mainWindow is not null && mainWindow is not LoginWindow)
			mainWindow.Close();

		ResetActivityTimer();
	}

	// Configure the inactivity timeout (can be called from settings)
	public static void SetInactivityTimeout(TimeSpan timeout, TimeSpan warningTime)
	{
		_inactivityThreshold = timeout;
		_warningThreshold = warningTime;
	}

	void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
	{
		// Find the TextBox
		DependencyObject parent = e.OriginalSource as UIElement;
		while (parent != null && parent is not TextBox)
			parent = VisualTreeHelper.GetParent(parent);

		if (parent != null)
		{
			var textBox = (TextBox)parent;
			if (!textBox.IsKeyboardFocusWithin)
			{
				// If the text box is not yet focused, give it the focus and
				// stop further processing of this click event.
				textBox.Focus();
				e.Handled = true;
			}
		}

		// Count this as activity
		ResetActivityTimer();
	}

	void SelectAllText(object sender, RoutedEventArgs e)
	{
		if (e.OriginalSource is TextBox textBox)
			textBox.SelectAll();

		// Count this as activity
		ResetActivityTimer();
	}

	private static bool GetIsLightTheme()
	{
		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
			var value = key?.GetValue("AppsUseLightTheme");
			return value is int i && i > 0;
		}
		catch
		{
			return true;
		}
	}
}
