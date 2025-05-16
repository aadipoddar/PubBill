using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PubBill.Common;

/// <summary>
/// Manages application-wide inactivity monitoring and auto-logout functionality
/// </summary>
public class InactivityMonitor
{
	// Singleton instance
	private static InactivityMonitor _instance;

	// Activity monitoring properties
	public DateTime LastActivityTime { get; private set; } = DateTime.Now;
	private DispatcherTimer _activityTimer;
	private TimeSpan _inactivityThreshold = TimeSpan.FromMinutes(5);
	private TimeSpan _warningThreshold = TimeSpan.FromSeconds(20);
	private bool _warningDisplayed = false;
	private Window _warningWindow;

	// Added to track programmatic refreshes
	private bool _isRefreshInProgress = false;

	private InactivityMonitor() =>
		_inactivityThreshold = TimeSpan.FromMinutes((int)Application.Current.Resources[SettingsKeys.InactivityTime]);

	public static InactivityMonitor Instance
	{
		get
		{
			_instance ??= new InactivityMonitor();
			return _instance;
		}
	}

	public void Initialize()
	{
		// Create and configure the inactivity timer
		_activityTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(10) // Check every 10 seconds
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
		// Don't reset the timer if we're in the middle of a refresh operation
		if (_isRefreshInProgress)
			return;

		ResetActivityTimer();
	}

	public void ResetActivityTimer()
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

	// New methods to mark the start and end of refresh operations
	public void BeginRefreshOperation()
	{
		_isRefreshInProgress = true;
	}

	public void EndRefreshOperation()
	{
		_isRefreshInProgress = false;
	}

	private void CheckForInactivity(object sender, EventArgs e)
	{
		// If a refresh is in progress, don't interrupt the inactivity check
		// (This allows inactivity detection to work even during refreshes)

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
	private static bool IsLoginWindowActive()
	{
		var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
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

		var mainWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

		if (mainWindow is not null && mainWindow is not LoginWindow)
			mainWindow.Close();

		ResetActivityTimer();
	}

	// Configure the inactivity timeout (can be called from settings)
	public void SetInactivityTimeout(TimeSpan timeout, TimeSpan warningTime)
	{
		_inactivityThreshold = timeout;
		_warningThreshold = warningTime;
	}
}
