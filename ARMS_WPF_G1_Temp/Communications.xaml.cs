using EasyModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ARMS_WPF_G1_Temp
{
    /// <summary>
    /// Interaction logic for Communications.xaml
    /// </summary>
    public partial class Communications : Window
    {
        SerialPort mySerialPort = new SerialPort();
        ModBusSlave mySlave;

        const int NONE_SELECTED = 0;
        const int USB_SELECTED = 1;
        const int RS232_SELECTED = 2;
        const int ETHERNET_SELECTED = 3;

        const int MAX_LINE_COUNT = 5;

        public MainWindow mainWindow;
        public ModbusClient mbClient;
        private List<string> ipAddresses = new List<string>();
        private List<string> ports = new List<string>();

        public Communications(MainWindow _mainWindow)
        {

            this.mainWindow = _mainWindow;
            InitializeComponent();


            //Acquire serial ports
            string[] sPorts = SerialPort.GetPortNames();
            var sortedList = sPorts.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            ComPortBox.Items.Clear();

            foreach (string port in sortedList)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = port;

                ComPortBox.Items.Add(item);
            }

            if (mainWindow.autoscanning)
            {
                AutoscanBut.Content = "Autoscanning...";
                Manual_ConnectBut.IsEnabled = false;
                SetGUIElements(mainWindow.connectionTypeInt);
            }
            else
            {
                AutoscanBut.Content = "Start Autoscanning";
                Manual_ConnectBut.IsEnabled = true;
                if (mainWindow.foundbestsled)
                {
                    SetGUIElements(mainWindow.connectionTypeInt);
                }
                else
                {
                    if (ComType.Text == USBType.Content.ToString())
                    {
                        SetGUIElements(USB_SELECTED);
                    }
                    else if (ComType.Text == RS232Type.Content.ToString())
                    {
                        SetGUIElements(RS232_SELECTED);
                    }
                    else if (ComType.Text == EthernetType.Content.ToString())
                    {
                        SetGUIElements(ETHERNET_SELECTED);
                    }
                    else
                    {
                        SetGUIElements(NONE_SELECTED);
                    }
                }

            }
            Update_ComSettings();
        }

        public void ScanCOMPorts(object sender, EventArrivedEventArgs e)
        {
            //Acquire serial ports
            string[] sPorts = SerialPort.GetPortNames();
            var sortedList = sPorts.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            this.Dispatcher.Invoke(() =>
            {
                ComPortBox.Items.Clear();

                foreach (string port in sortedList)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = port;

                    ComPortBox.Items.Add(item);
                }
            });
        }

        //private void Disconnect_Comms(object sender, RoutedEventArgs e)
        //{
        //    if (mainWindow.mbClient.Connected)
        //    {
        //        mainWindow.Diagnostics.Text += "Disconnecting...";
        //        MainWindow.modbusSlaveList[mainWindow.selectedTabSlaveID[mainWindow.TopTab.SelectedIndex]].WhichCycle = MainWindow.MANUAL_DISCONNECT;
        //    }
        //}

        public void Update_ComSettings()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (mainWindow.selectedSlaveID != 0)
                {

                    if (mainWindow.connectionTypeInt == 1)
                    {

                        ComPortBox.Text = "COM" + mainWindow.selectedSlaveID.ToString();
                        ComType.Text = USBType.Content.ToString();
                        ComSpeedEdit.Text = mainWindow.mbClient.Baudrate.ToString();
                        EthernetIPAddressEdit.Text = "255.255.255.255";
                        EthernetPortEdit.Text = "502";

                    }
                    else if (mainWindow.connectionTypeInt == 2)
                    {
                        ComPortBox.Text = "COM" + mainWindow.selectedSlaveID.ToString();
                        ComType.Text = RS232Type.Content.ToString();
                        ComSpeedEdit.Text = mainWindow.mbClient.Baudrate.ToString();
                        EthernetIPAddressEdit.Text = "255.255.255.255";
                        EthernetPortEdit.Text = "502";

                    }
                    else if (mainWindow.connectionTypeInt == 3)
                    {
                        ComType.Text = EthernetType.Content.ToString();
                        ComPortBox.Text = "";
                        ComSpeedEdit.Text = "";
                        EthernetIPAddressEdit.Text = "255.255.255.255";
                        EthernetPortEdit.Text = "502";

                    }
                    else
                    {
                        ComType.Text = mainWindow.new_ComType_string;
                    }

                }
                ModbusIDEdit.Text = (mainWindow.ExistingModbusID).ToString();
            });
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

        }

        private void ComTypeChanged(object sender, RoutedEventArgs e)
        {

            ComboBoxItem new_ComType = ComType.SelectedItem as ComboBoxItem;

            if (new_ComType != null)
            {
                mainWindow.new_ComType_string = new_ComType.Content.ToString();
                ComType.Text = mainWindow.new_ComType_string;
            }


            if (mainWindow.new_ComType_string == USBType.Content.ToString())
            {
                SetGUIElements(USB_SELECTED);
            }
            else if (mainWindow.new_ComType_string == RS232Type.Content.ToString())
            {
                SetGUIElements(RS232_SELECTED);
            }
            else if (mainWindow.new_ComType_string == EthernetType.Content.ToString())
            {
                SetGUIElements(ETHERNET_SELECTED);
            }
            else
            {
                SetGUIElements(NONE_SELECTED);
            }

        }
        private void ComPortChanged(object sender, RoutedEventArgs e)
        {

            ComboBoxItem new_ComPort = ComPortBox.SelectedItem as ComboBoxItem;

            if (new_ComPort != null)
            {
                mainWindow.new_ComPort_string = new_ComPort.Content.ToString();
                ComPortBox.Text = mainWindow.new_ComPort_string;
            }

        }
        private void ClipboardBut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LoggingTextEdit.Text);
        }
        private void ClearBut_Click(object sender, RoutedEventArgs e)
        {
            LoggingTextEdit.Text = "";
        }
        private void PauseBut_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.commslogging == true)
            {
                mainWindow.commslogging = false;
                PauseBut.Content = "Resume";
            }
            else if (mainWindow.commslogging == false)
            {
                mainWindow.commslogging = true;
                PauseBut.Content = "Pause";
            }
        }
        private void AutoscanBut_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.autoscanning)
            {
                mainWindow.autoscanning = false;
                if (ComType.Text == USBType.Content.ToString())
                {
                    SetGUIElements(USB_SELECTED);
                }
                else if (ComType.Text == RS232Type.Content.ToString())
                {
                    SetGUIElements(RS232_SELECTED);
                }
                else if (ComType.Text == EthernetType.Content.ToString())
                {
                    SetGUIElements(ETHERNET_SELECTED);
                }
                else
                {
                    SetGUIElements(NONE_SELECTED);
                }
                AutoscanBut.Content = "Start Autoscanning";
                Manual_ConnectBut.IsEnabled = true;
                //new_ComType_string = ComType.Text;
                //new_ComPort_string = ComPortBox.Text;

                ManagementEventWatcher watcher = new ManagementEventWatcher();
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
                watcher.EventArrived += new EventArrivedEventHandler(ScanCOMPorts);
                watcher.Query = query;
                watcher.Start();

            }
            else
            {
                mainWindow.autoscanning = true;
                SetGUIElements(mainWindow.connectionTypeInt);
                AutoscanBut.Content = "Autoscanning...";
                Manual_ConnectBut.IsEnabled = false;
            }
        }

        private void Manual_ConnectBut_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.manual_connection = true;
            mainWindow.new_ComType_string = ComType.Text;
            mainWindow.new_ComPort_string = ComPortBox.Text;
        }


        private void SetGUIElements(int whichSelection)
        {
            if (!mainWindow.autoscanning)
            {
                ComType.IsEnabled = true;

                if (whichSelection == NONE_SELECTED)
                {
                    ModbusIDEdit.IsEnabled = true;

                    EthernetIPAddressEdit.IsEnabled = false;
                    EthernetPortEdit.IsEnabled = false;

                    ComPortBox.IsEnabled = false;
                }

                else if (whichSelection == USB_SELECTED)
                {
                    ModbusIDEdit.IsEnabled = true;

                    //Disable Ethernet GUI Elements
                    EthernetIPAddressEdit.IsEnabled = false;
                    EthernetPortEdit.IsEnabled = false;

                    //Enable USB GUI Elements
                    ComPortBox.IsEnabled = true;

                }
                else if (whichSelection == RS232_SELECTED)
                {
                    ModbusIDEdit.IsEnabled = true;

                    //Disable Ethernet GUI Elements
                    EthernetIPAddressEdit.IsEnabled = false;
                    EthernetPortEdit.IsEnabled = false;

                    //Enable USB GUI Elements
                    ComPortBox.IsEnabled = true;

                }
                else if (whichSelection == ETHERNET_SELECTED)
                {
                    ModbusIDEdit.IsEnabled = false;

                    //Enable Ethernet GUI Elements
                    EthernetIPAddressEdit.IsEnabled = true;
                    EthernetPortEdit.IsEnabled = true;

                    //Disable USB GUI Elements
                    ComPortBox.IsEnabled = false;


                }
            }
            else
            {
                ModbusIDEdit.IsEnabled = false;
                ComType.IsEnabled = false;

                EthernetIPAddressEdit.IsEnabled = false;
                EthernetPortEdit.IsEnabled = false;

                ComPortBox.IsEnabled = false;

            }
        }
        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            //Do whatever you want here..
            this.Visibility = Visibility.Hidden;
        }

    }

}

