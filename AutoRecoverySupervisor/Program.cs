using System.Diagnostics;

namespace AutoRecoverySupervisor
{
	/// <summary>
	/// AutoRecovery library. Provides two functions:
	/// 1. A scheduler to save the user data regularly.
	/// 2. A supervisor process to restart an app after a crash.
	/// 
	/// This component monitors a windows process with a given ID and restarts it if crashed.
	///
	/// Usage:
	/// Copy the binaries of this program into the application folder (bin folder) of your app.
	/// Use the demo app to see how to include it into your program.
	///
	/// Oliver Abraham
	/// https://www.abraham-beratung.de
	/// Hosted on https://github.com/OliverAbraham
	/// Available as nuget package on https://www.nuget.org
	/// </summary>
	public class Program
	{
		#region ------------- Fields --------------------------------------------------------------
		private static int _processID;
		private static int _checkIntervalInSeconds;
		private static Process _processToObserve;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		public static void Main(string[] args)
		{
			LogStart();
			ParseCommandlineParameters(args);
			ReadProcessToObserve();
			LogParameters();
			ObserverLoop();
			RestartCrashedProcess();
			LogEnding();
		}
		#endregion



		#region ------------- Implementation ------------------------------------------------------
		private static void LogStart()
		{
			Console.WriteLine("AutoRecoverySupervisor has started");
		}

		private static void ParseCommandlineParameters(string[] args)
		{
			_processID = Convert.ToInt32(args[0]);
			_checkIntervalInSeconds = Convert.ToInt32(args[1]);
		}

		private static void ReadProcessToObserve()
		{
			_processToObserve = Process.GetProcessById(_processID);
			if (_processToObserve != null && _processToObserve.HasExited)
			{
				Console.WriteLine($"Process with name '{_processToObserve?.ProcessName}' has crashed. It will be restarted now");
				Process.Start(_processToObserve.ProcessName);
			}
		}

		private static void LogParameters()
		{
			Console.WriteLine($"ProcessID      = {_processToObserve.Id}");
			Console.WriteLine($"ProcessName    = {_processToObserve.ProcessName}");
			Console.WriteLine($"Check interval = {_checkIntervalInSeconds} seconds");
		}

		private static void ObserverLoop()
		{
			while (Check())
				Thread.Sleep(_checkIntervalInSeconds * 1000);
		}

		private static void LogEnding()
		{
			Console.WriteLine("AutoRecoverySupervisor has ended");
			Thread.Sleep(10 * 1000);
		}

		private static bool Check()
		{
			try
			{
				var process = Process.GetProcessById(_processID);
				if (process == null || process.HasExited)
					return false;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private static void RestartCrashedProcess()
		{
			try
			{
				Console.WriteLine($"Process with name '{_processToObserve?.ProcessName}' has crashed. It will be restarted now");
				Process.Start(_processToObserve.ProcessName);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unable to restart the process. More info:\n{ex.ToString()}");
			}
		}
		#endregion
	}
}
