using Abraham.AutoRecovery;
using System.IO;
using System.Windows;

namespace AutoRecoveryDemoWPF
{
	/// <summary>
	/// AutoRecovery library Demo.
	/// Demonstrates the AutoSave and AutoRestart functionality
	/// 
	/// 1. A scheduler to save the user data regularly.
	/// 2. A supervisor process to restart an app after a crash.
	/// 
	/// Oliver Abraham
	/// https://www.abraham-beratung.de
	/// Hosted on https://github.com/OliverAbraham
	/// Available as nuget package on https://www.nuget.org
	/// </summary>
	public partial class MainWindow : Window
	{
		IAutoRecovery _autoRecovery = new AutoRecovery();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (_autoRecovery.AppHasCrashed)
				LoadDataFromAutoSaveFile();
			else
				LoadData();
			_autoRecovery.EnableAutoSave(10, SaveDataForAutoSave); // save every 10 seconds
			_autoRecovery.EnableAutoRestart(startVisible: true); // restart this app after crash, start visible for Demo only!
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveData();
			DeleteAutoSaveFile();
			_autoRecovery.NormalShutdown(); // indicate that app didn't crash
		}

		private void LoadData()
		{
			if (File.Exists("MyDatafile.txt"))
				MyTextbox.Text = File.ReadAllText("MyDatafile.txt");
		}

		private void SaveData()
		{
			File.WriteAllText("MyDatafile.txt", MyTextbox.Text);
		}

		private void LoadDataFromAutoSaveFile()
		{
			if (File.Exists("MyAutosaveFile.txt"))
				MyTextbox.Text = File.ReadAllText("MyAutosaveFile.txt");
		}

		private void SaveDataForAutoSave()
		{
			Dispatcher.Invoke(() => { SaveDataToAutoSaveFile(); });
		}

		private void SaveDataToAutoSaveFile()
		{
			File.WriteAllText("MyAutosaveFile.txt", MyTextbox.Text);
		}

		private void DeleteAutoSaveFile()
		{
			File.Delete("MyAutosaveFile.txt");
		}
	}
}
