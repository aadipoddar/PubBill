using System.Windows.Threading;

namespace PubBill.Common;

internal class SmartRefreshManager
{
	private readonly DispatcherTimer _timer;
	private readonly Func<Task> _refreshAction;
	private readonly TimeSpan _interval;
	private bool _isRefreshing = false;
	private DateTime _lastRefreshTime = DateTime.MinValue;

	internal SmartRefreshManager(Func<Task> refreshAction, TimeSpan interval)
	{
		_refreshAction = refreshAction;
		_interval = interval;
		_timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
		_timer.Tick += CheckAndRefresh;
	}

	internal void Start() => _timer.Start();
	internal void Stop() => _timer.Stop();

	private async void CheckAndRefresh(object sender, EventArgs e)
	{
		if (_isRefreshing || DateTime.Now - _lastRefreshTime < _interval)
			return;

		_isRefreshing = true;
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();

			await _refreshAction();
			_lastRefreshTime = DateTime.Now;
		}
		catch (Exception)
		{
			// Log error
		}
		finally
		{
			_isRefreshing = false;
			// Always end the refresh operation
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	internal async Task ForceRefresh()
	{
		if (_isRefreshing) return;

		_isRefreshing = true;
		try
		{
			// Tell the inactivity monitor that we're starting a programmatic refresh
			InactivityMonitor.Instance.BeginRefreshOperation();

			await _refreshAction();
			_lastRefreshTime = DateTime.Now;
		}
		finally
		{
			_isRefreshing = false;
			// Always end the refresh operation
			InactivityMonitor.Instance.EndRefreshOperation();
		}
	}

	internal void Dispose()
	{
		if (_timer is not null)
		{
			_timer.Tick -= CheckAndRefresh;
			_timer.Stop();
		}
	}
}