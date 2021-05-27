using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.IO;
using System.ComponentModel;
using System.Windows.Threading;

namespace ARMS_WPF_G1_Temp
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    /// 
    public partial class About : Window
    {
        public MainWindow mainWindow;
        ModBusSlave mySlave;
        ModBusSlave selectedSlave;

        public About(MainWindow _mainWindow)
        {
            InitializeComponent();
            this.mainWindow = _mainWindow;
            mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
            selectedSlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
            UpdateAboutGUI(mySlave);



        }
        public void UpdateAboutGUI(ModBusSlave mySlave)
        {
            if (mainWindow.selectedSlaveID != 0)
            {
                //update UI
                this.Dispatcher.Invoke(() =>
                {
                    if (mySlave.Model - 120 <= 10)
                        ModelumberLabel.Content = "Model Number: ASM00000" + (mySlave.Model - 120);
                    else if (mySlave.Model - 120 >= 10 && mySlave.Model - 120 <= 100)
                        ModelumberLabel.Content = "Model Number: ASM0000" + (mySlave.Model - 120);
                    else if (mySlave.Model - 120 >= 100 && mySlave.Model - 120 <= 1000)
                        ModelumberLabel.Content = "Model Number: ASM000" + (mySlave.Model - 120);
                    else if (mySlave.Model - 120 >= 1000 && mySlave.Model - 120 <= 10000)
                        ModelumberLabel.Content = "Model Number: ASM00" + (mySlave.Model - 120);
                    else if (mySlave.Model - 120 >= 10000 && mySlave.Model - 120 <= 100000)
                        ModelumberLabel.Content = "Model Number: ASM0" + (mySlave.Model - 120);
                    else
                        ModelumberLabel.Content = "Model Number: ASM" + (mySlave.Model - 120);



                    SLED1.Content = mySlave.Capabilities[0].ToString() + " nm";

                    if (mySlave.Capabilities[12] == 0)
                        SLED_TEC.Content = "Not Present";
                    else
                        SLED_TEC.Content = "Present";
                    if (mySlave.Capabilities[14] == 0)
                        Heat_Sink_Temp.Content = "Not Present";
                    else
                        Heat_Sink_Temp.Content = "Present";
                    if (mySlave.Capabilities[16] == 0)
                        Power_Meter.Content = "Not Present";
                    else
                        Power_Meter.Content = "Present";
                    Firmware_Version.Content = mySlave.FirmwareVersion.ToString();
                    Mac_Address.Content = ("0.0.0.0");

                    NewDiagnosticFile.IsEnabled = true;
                });
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    SoftwareVersionLabel.Content = "Software Version: R01.P07";

                    ModelumberLabel.Content = "Model Number: N/A";
                    SLED1.Content = "N/A";
                    SLED_TEC.Content = "N/A";
                    Heat_Sink_Temp.Content = "N/A";
                    Power_Meter.Content = "N/A";
                    Firmware_Version.Content = "N/A";
                    Mac_Address.Content = "N/A";

                    NewDiagnosticFile.IsEnabled = false;
                });
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }

        }
        private void NewDiagnosticFile_Click(object sender, RoutedEventArgs e)
        {
            
            mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = ".csv"; // Required file extension 
            fileDialog.Filter = "csv files (.csv)|*.csv"; // Optional file extensions
            bool? res = fileDialog.ShowDialog();
            //Initialize line counts
            mySlave.AdminLogfileLineCount = 0;
            if (res.HasValue && res.Value)
            {
                filepath.Text = "Diagnostic File saved under: " + fileDialog.FileName;
                mainWindow.adminLogFilePathStrings.Remove(mySlave.SlaveID.ToString());

                mainWindow.adminLogFilePathStrings.Add(mySlave.SlaveID.ToString(), fileDialog.FileName);

                //Create empty files
                File.WriteAllText(mainWindow.adminLogFilePathStrings[mySlave.SlaveID.ToString()], String.Empty);

                //Assign slave object properties
                mySlave.SlaveAdminLogFilePath = mainWindow.adminLogFilePathStrings[mySlave.SlaveID.ToString()];

                //Establish output streams
                mySlave.slaveAdminLogFileOutputStream = new FileStream(mainWindow.adminLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Append, FileAccess.Write, FileShare.Read);
                mySlave.slaveAdminLogFileFsWriter = new StreamWriter(mySlave.slaveAdminLogFileOutputStream);

                //Establish input streams
                mySlave.adminIStreamLeft = new FileStream(mainWindow.adminLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                mySlave.adminIStreamRight = new FileStream(mainWindow.adminLogFilePathStrings[mySlave.SlaveID.ToString()], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                mySlave.adminFSReaderLeft = new StreamReader(mySlave.adminIStreamLeft);
                mySlave.adminFSReaderRight = new StreamReader(mySlave.adminIStreamRight);

                //Initialize CSV file
                mainWindow.InitializeAdminCSVFile(mySlave);

                //Read past header line for left and right streams for both files
                mySlave.adminFSReaderLeft.ReadLine();
                mySlave.adminFSReaderRight.ReadLine();

                mySlave.CurrentLeftIndex = -1;
                mySlave.CurrentRightIndex = -1;

                mySlave.NewDiagnosticFileBut = true;

                

            }


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
