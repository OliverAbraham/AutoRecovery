using Abraham.Threading;
using System;
using System.Diagnostics;
using System.IO;

namespace Abraham.AutoRecovery
{
	/// <summary>
	/// AutoRecovery library. Provides two functions:
	/// 1. A scheduler to save the user data regularly.
	/// 2. A supervisor process to restart an app after a crash.
	/// 
	/// Oliver Abraham
	/// https://www.abraham-beratung.de
	/// Hosted on https://github.com/OliverAbraham
	/// Available as nuget package on https://www.nuget.org
	/// </summary>
	public class AutoRecovery : IAutoRecovery
	{
		#region ------------- Properties ----------------------------------------------------------
		public bool AppHasCrashed => CheckIfAppHasCrashed();
		public int RestartCheckIntervalInSeconds { get; set; } = 5;
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private Scheduler _autoSaveScheduler;
		private int _supervisorProcessID;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		#endregion



		#region ------------- Methods -------------------------------------------------------------
		public void NormalShutdown()
		{
			File.Delete("CrashIndicator.eye");
			StopAutoSaveScheduler();
			StopSupervisorProcess();
		}

		public void EnableAutoSave(uint intervalInSeconds, Action saveDataAction)
		{
			StartAutoSaveScheduler(intervalInSeconds, saveDataAction);
		}

		public void DisableAutoSave()
		{
			StopAutoSaveScheduler();
		}

		public void EnableAutoRestart(bool startVisible = false)
		{
			File.WriteAllText("CrashIndicator.eye", "");
			StartSupervisorProcess(startVisible);
		}
		#endregion



		#region ------------- Implementation ------------------------------------------------------
		private bool CheckIfAppHasCrashed()
		{
			return File.Exists("CrashIndicator.eye");
		}

		#pragma warning disable 1998
		private void StartAutoSaveScheduler(uint intervalInSeconds, Action saveDataAction)
		{
			_autoSaveScheduler = new Scheduler();
			_autoSaveScheduler.SetSimpleSchedule(intervalInSeconds);
			_autoSaveScheduler.Action = async delegate() { saveDataAction(); };
			_autoSaveScheduler.Start();
		}

		private void StopAutoSaveScheduler()
		{
			_autoSaveScheduler?.Stop();
		}

		private void StartSupervisorProcess(bool startVisible = false)
		{
			var ownProcessID = Process.GetCurrentProcess().Id;
			
			var startInfo = new ProcessStartInfo();
			startInfo.FileName = "AutoRecoverySupervisor.exe";
			startInfo.Arguments = $"{ownProcessID} {RestartCheckIntervalInSeconds}";
			if (startVisible)
			{
				startInfo.CreateNoWindow = false;
				startInfo.WindowStyle = ProcessWindowStyle.Normal;
				startInfo.UseShellExecute = true;
			}
			var process = Process.Start(startInfo);

			_supervisorProcessID = process?.Id ?? 0;
		}

		private void StopSupervisorProcess()
		{
			var process = Process.GetProcessById(_supervisorProcessID);
			if (process != null)
			{
				process.Kill();
			}
		}
		#endregion
	}
}