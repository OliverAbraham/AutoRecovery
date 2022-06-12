# Abraham.AutoRecovery


## Abstract

This library provides a scheduler for automatically saving user data 
and a supervisor process to automatically restart your app after a crash.


## License

Licensed under Apache licence.
https://www.apache.org/licenses/LICENSE-2.0


## Compatibility

The nuget package was build with DotNET 6.


## Example

For an example refer to project "AutoRecoveryDemoWPF". It demonstrates both features:
1. The scheduler that triggers a given function periodically to save the user's data.
2. When you app starts, a separate (hidden) process is started to monitor your app.
   In case you app crashes and its process ends, the hidden process will restart
   your app. If your process ends normally, the hidden process will also end.


## Getting started

Add the Nuget package "Abraham.AutoRecovery" to your project.

Add a field to your project:

```C#
		IAutoRecovery _autoRecovery = new AutoRecovery();
```



### Adding AutoSave to your project

At startup of your app, add the following code:

```C#
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (_autoRecovery.AppHasCrashed)
				LoadDataFromAutoSaveFile();
			else
				LoadData();
			_autoRecovery.EnableAutoSave(10, SaveDataForAutoSave); // save every 10 seconds
		}
```

This method will be called periodically to save the user's data:

```C#
		private void SaveDataForAutoSave()
		{
			Dispatcher.Invoke(() => { SaveDataToAutoSaveFile(); });
		}
```

At shutdown, add the following code:

```C#
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveData();
			DeleteAutoSaveFile(); // optional
		}
```

After saving user's data as usual, you can add a method that deletes the file 
containing the autosave data.


### Adding AutoRestart to your project

At startup of your app, add the following code. 
(You'll want to change startVisible to false after verifying it's working):

```C#
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_autoRecovery.EnableAutoRestart(startVisible: true); // restart this app after crash, start visible for Demo only!
		}
```


At shutdown, add the following code:

```C#
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_autoRecovery.NormalShutdown();
		}
```

This call will end the hidden supervisor process.

### Testing AutoRestart

- Start the demo app 'AutoRecoveryDemoWPF.exe' by doubleclicking.
- The go to task manager and end this process.
- Don't kill process tree! Only kill the single process.
- After a few seconds the supervisor process will recognize that and restart your process.




## HOW TO INSTALL A NUGET PACKAGE
This is very simple:
- Start Visual Studio (with NuGet installed) 
- Right-click on your project's References and choose "Manage NuGet Packages..."
- Choose Online category from the left
- Enter the name of the nuget package to the top right search and hit enter
- Choose your package from search results and hit install
- Done!


or from NuGet Command-Line:

    Install-Package Abraham.ProgramSettingsManager





## AUTHOR

Oliver Abraham, mail@oliver-abraham.de, https://www.oliver-abraham.de

Please feel free to comment and suggest improvements!



