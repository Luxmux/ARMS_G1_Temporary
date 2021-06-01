using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ARMS_WPF_G1_Temp
{
    /// <summary>
    /// Interaction logic for Logging.xaml
    /// </summary>
    public partial class Logging : Window
    {
		public MainWindow mainWindow;
		ModBusSlave mySlave;

		public Logging(MainWindow _mainWindow)
		{
            InitializeComponent();
			this.mainWindow = _mainWindow;
			mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
			if (mainWindow.loggingToFiles)
			{
				this.Dispatcher.Invoke(() =>
				{
					LoggingExistingBut.IsEnabled = false;
					LoggingNewBut.IsEnabled = false;
					LoggingOnOffBut.IsEnabled = true;
				});
			}
			else
			{
				this.Dispatcher.Invoke(() =>
				{
					LoggingExistingBut.IsEnabled = true;
					LoggingNewBut.IsEnabled = true;
					LoggingOnOffBut.IsEnabled = false;
				});
			}
			
		}

		private void LoggingNewBut_Click(object sender, RoutedEventArgs e)
		{
			if (mainWindow.loggingToFiles == false)
			{
				SaveFileDialog fileDialog = new SaveFileDialog();
				fileDialog.DefaultExt = ".csv"; // Required file extension 
				fileDialog.Filter = "csv files (.csv)|*.csv"; // Optional file extensions
				bool? res = fileDialog.ShowDialog();
				if (res.HasValue && res.Value)
				{
					filepath.Text = fileDialog.FileName;
					mainWindow.publicLogFilePathStrings.Remove(mySlave.SlaveID.ToString());
					mainWindow.publicLogFilePathStrings.Add(mySlave.SlaveID.ToString(), fileDialog.FileName);

					//Create empty files
					File.WriteAllText(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], String.Empty);

					//Assign slave object properties
					mySlave.SlavePublicLogFilePath = mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()];

					//Initialize line counts
					mySlave.PublicLogfileLineCount = 0;

					//Establish output streams
					mySlave.slavePublicLogFileOutputStream = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Append, FileAccess.Write, FileShare.Read);
					mySlave.slavePublicLogFileFsWriter = new StreamWriter(mySlave.slavePublicLogFileOutputStream);

					//Establish input streams
					mySlave.publicIStreamLeft = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					mySlave.publicIStreamRight = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					mySlave.publicFSReaderLeft = new StreamReader(mySlave.publicIStreamLeft);
					mySlave.publicFSReaderRight = new StreamReader(mySlave.publicIStreamRight);

					//Initialize both CSV files
					mainWindow.InitializePublicCSVFile(mySlave);

					//Read past header line for left and right streams for both files
					mySlave.publicFSReaderLeft.ReadLine();
					mySlave.publicFSReaderRight.ReadLine();

					mySlave.CurrentLeftIndex = -1;
					mySlave.CurrentRightIndex = -1;

					mainWindow.loggingToFiles = true;

					LoggingOnOffBut.Content = "Stop Logging";
					LoggingExistingBut.IsEnabled = false;
					LoggingNewBut.IsEnabled = false;
					LoggingOnOffBut.IsEnabled = true;
					PrintStringToLogging_actions("Logging to File: " + filepath.Text);

					if (logInterval.SelectedIndex == 0) mySlave.LoggingInterval = 1;
					if (logInterval.SelectedIndex == 1) mySlave.LoggingInterval = 5;
					if (logInterval.SelectedIndex == 2) mySlave.LoggingInterval = 10;
					if (logInterval.SelectedIndex == 3) mySlave.LoggingInterval = 15;
					if (logInterval.SelectedIndex == 4) mySlave.LoggingInterval = 30;
					if (logInterval.SelectedIndex == 5) mySlave.LoggingInterval = 60;
				}
			}
			else
			{
				MessageBox.Show("Please stop logging before changing path");
			}
		}

		private void LoggingExistingBut_Click(object sender, RoutedEventArgs e)
		{
			if (mainWindow.loggingToFiles == false)
			{
				OpenFileDialog fileDialog = new OpenFileDialog();
				fileDialog.DefaultExt = ".csv"; // Required file extension 
				fileDialog.Filter = "csv files (.csv)|*.csv"; // Optional file extensions
				bool? res = fileDialog.ShowDialog();
				if (res.HasValue && res.Value)
				{
					filepath.Text = fileDialog.FileName;
					mainWindow.publicLogFilePathStrings.Remove(mySlave.SlaveID.ToString());
					mainWindow.publicLogFilePathStrings.Add(mySlave.SlaveID.ToString(), fileDialog.FileName);

					//Assign slave object properties
					mySlave.SlavePublicLogFilePath = mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()];

					//Establish output streams
					mySlave.slavePublicLogFileOutputStream = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Append, FileAccess.Write, FileShare.Read);
					mySlave.slavePublicLogFileFsWriter = new StreamWriter(mySlave.slavePublicLogFileOutputStream);

					//Establish input streams
					mySlave.publicIStreamLeft = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					mySlave.publicIStreamRight = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					mySlave.publicFSReaderLeft = new StreamReader(mySlave.publicIStreamLeft);
					mySlave.publicFSReaderRight = new StreamReader(mySlave.publicIStreamRight);

					//Read past header line for left and right streams for both files
					mySlave.publicFSReaderLeft.ReadLine();
					mySlave.publicFSReaderRight.ReadLine();

					mySlave.CurrentLeftIndex = -1;
					mySlave.CurrentRightIndex = -1;

					mainWindow.loggingToFiles = true;

					LoggingOnOffBut.Content = "Stop Logging";
					LoggingExistingBut.IsEnabled = false;
					LoggingNewBut.IsEnabled = false;
					LoggingOnOffBut.IsEnabled = true;
					PrintStringToLogging_actions("Logging to File: " + filepath.Text);

					if (logInterval.SelectedIndex == 0) mySlave.LoggingInterval = 1;
					if (logInterval.SelectedIndex == 1) mySlave.LoggingInterval = 5;
					if (logInterval.SelectedIndex == 2) mySlave.LoggingInterval = 10;
					if (logInterval.SelectedIndex == 3) mySlave.LoggingInterval = 15;
					if (logInterval.SelectedIndex == 4) mySlave.LoggingInterval = 30;
					if (logInterval.SelectedIndex == 5) mySlave.LoggingInterval = 60;
				}
			}
			else
			{
				MessageBox.Show("Please stop logging before changing path");
			}
		}

		private void LoggingOnOffBut_Click(object sender, RoutedEventArgs e)
		{
			if (filepath.Text.Trim() != "[Please choose log path]")
			{
				if (mainWindow.loggingToFiles == true)
				{
					mainWindow.loggingToFiles = false;
					LoggingOnOffBut.Content = "Start Logging";
					mySlave.slavePublicLogFileFsWriter.Close();
					PrintStringToLogging_actions("Stopped logging to file.");
					LoggingExistingBut.IsEnabled = true;
					LoggingNewBut.IsEnabled = true;
					LoggingOnOffBut.IsEnabled = true;
					if (logInterval.SelectedIndex == 0) mySlave.LoggingInterval = 1;
					if (logInterval.SelectedIndex == 1) mySlave.LoggingInterval = 5;
					if (logInterval.SelectedIndex == 2) mySlave.LoggingInterval = 10;
					if (logInterval.SelectedIndex == 3) mySlave.LoggingInterval = 15;
					if (logInterval.SelectedIndex == 4) mySlave.LoggingInterval = 30;
					if (logInterval.SelectedIndex == 5) mySlave.LoggingInterval = 60;

				}
				else
				{
					mainWindow.loggingToFiles = true;
					LoggingOnOffBut.Content = "Stop Logging";
					PrintStringToLogging_actions("Logging to File: " + filepath.Text);
					LoggingExistingBut.IsEnabled = false;
					LoggingNewBut.IsEnabled = false;
					LoggingOnOffBut.IsEnabled = true;

					mainWindow.publicLogFilePathStrings.Remove(mySlave.SlaveID.ToString());
					mainWindow.publicLogFilePathStrings.Add(mySlave.SlaveID.ToString(), filepath.Text);

					//Assign slave object properties
					mySlave.SlavePublicLogFilePath = mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()];

					//Establish output streams
					mySlave.slavePublicLogFileOutputStream = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Append, FileAccess.Write, FileShare.Read);
					mySlave.slavePublicLogFileFsWriter = new StreamWriter(mySlave.slavePublicLogFileOutputStream);

					//Establish input streams
					mySlave.publicIStreamLeft = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					mySlave.publicIStreamRight = new FileStream(mainWindow.publicLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					mySlave.publicFSReaderLeft = new StreamReader(mySlave.publicIStreamLeft);
					mySlave.publicFSReaderRight = new StreamReader(mySlave.publicIStreamRight);

					//Read past header line for left and right streams for both files
					mySlave.publicFSReaderLeft.ReadLine();
					mySlave.publicFSReaderRight.ReadLine();

					mySlave.CurrentLeftIndex = -1;
					mySlave.CurrentRightIndex = -1;

					mainWindow.loggingToFiles = true;
				}
			}
			else
			{
				MessageBox.Show("Please create or a choose file first before running logging.");
			}
		}
		public void PrintStringToLogging_actions(string theString)
		{
		
			this.Dispatcher.Invoke(() =>
			{
				Logging_actions.Text += DateTime.Now.ToString() + " | " + theString + '\n';
				Logging_actions.ScrollToEnd();

				//for testing commented out
				//ClearDiagnosticsLine();
				Logging_actions.Refresh();
			});
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			//Do whatever you want here..
			this.Visibility = Visibility.Hidden;
		}
		private void CloseBut_Click(object sender, RoutedEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
		}

	}
}
