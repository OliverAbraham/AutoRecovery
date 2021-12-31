using System;

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
	public interface IAutoRecovery
	{
		bool AppHasCrashed { get; }
		int RestartCheckIntervalInSeconds { get; set; }
		void NormalShutdown();
		void EnableAutoSave(uint intervalInSeconds, Action saveDataAction);
		void EnableAutoRestart(bool startVisible = false);
	}
}
