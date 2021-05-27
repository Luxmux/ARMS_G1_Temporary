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

                    SLED1.Content = mySlave.Capabilities[0].ToString() + " nm";

                    
                    Firmware_Version.Content = mySlave.FirmwareVersion.ToString();
                });
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    SoftwareVersionLabel.Content = "Software Version: R01.P01";

                    ModelumberLabel.Content = "Model Number: N/A";
                    SLED1.Content = "N/A";
                    
                    Firmware_Version.Content = "N/A";
                });
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
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
