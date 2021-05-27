using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Defaults.xaml
    /// </summary>
    public partial class Defaults : Window
    {
        public MainWindow mainWindow;
        ModBusSlave mySlave;

        public Defaults(MainWindow _mainWindow)
        {
            InitializeComponent();
            this.mainWindow = _mainWindow;
            mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];

            //update UI
            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.Capabilities[0] != 0)
                {
                    ExistingPcCurr1.Text = mySlave.ExistingPcCurr1.ToString("0.0") + " mA";
                    ManufacturerDefPcCurr1.Text = mySlave.ManufacturerDefPcCurr1.ToString("0.0") + " mA";
                }
                else
                {
                    Sled1PCCurrSaveBut.IsEnabled = false;
                    TempPcCurr1.IsEnabled = false;
                }

                
            });

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(this);
            TextBox tBox = null;
            tBox = focusedControl as TextBox;
            if (tBox != null)
            {
                if (tBox.Name == "TempPcCurr1")
                    Sled1PCCurrSaveBut.IsDefault = true;
                else { }
            }




        }


        public void PrintStringToDiagnostics(string theString)
        {
            Thread thread = new Thread(() => PrintThread(theString));
            thread.Start();

        }
        private void PrintThread(string theString)
        {
            this.Dispatcher.Invoke(() =>
            {
                Diagnostics.Text += DateTime.Now.ToString() + " | " + theString + '\n';
                Diagnostics.ScrollToEnd();

                //for testing commented out
                //ClearDiagnosticsLine();
                Diagnostics.Refresh();
            });

            System.Threading.Thread.CurrentThread.Abort();
        }

        private void Sled1PCCurrSaveBut_Click(object sender, RoutedEventArgs e)
        {
            Sled1PCCurrSaveBut.IsDefault = false;
            if (TempPcCurr1.Text.Length > 0 && IsTextNumeric(TempPcCurr1.Text) == true)
            {
                ushort valueToWrite = (ushort) Math.Round(65535.0 * double.Parse(TempPcCurr1.Text) / 1000 / 2.5,0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,30, valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 30);
                UInt16[] readData1 = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1);
                UInt16[] readData2 = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,30, 1);



                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {

                    PrintStringToDiagnostics("SLED 1 PC Mode Default Current set successful.");
                    TempPcCurr1.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,30, 1);
                    mySlave.ExistingPcCurr1 = ((double)readData[0] * 2.5 / 65535.0 * 1000.0);
                    ExistingPcCurr1.Text = mySlave.ExistingPcCurr1.ToString("0.0") + " mA";
                }
                else
                    PrintStringToDiagnostics("Error: Write SLED 1 PC Mode Default Current failed.");
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);

            }
            else
            {
                TempPcCurr1.Text = "";
                PrintStringToDiagnostics("Error: New Value for SLED 11 PC Mode Default Current is not valid");
            }

        }


        private void ModbusIDSaveBut_Click(object sender, RoutedEventArgs e)
        {
            ModbusIDSaveBut.IsDefault = false;

            if (TempModbusID.Text.Length > 0 && IsTextNumeric(TempModbusID.Text) == true)
            {
                double valueToWrite = double.Parse(TempModbusID.Text);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,7, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 7);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("Default Modbus ID set successful.");
                    TempModbusID.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,7, 1);
                    mySlave.ExistingModbusID = (int)readData[0];
                    ExistingModbusID.Text = mySlave.ExistingModbusID.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("Error: Default Modbus ID set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for Default Modbus ID is not valid");
                TempModbusID.Text = "";
            }
            
        

        }

      
        private void ResetToManufacturerDefaultsBut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reset to Factory Defaults in Process, please wait. Status will be updated when done. ");
            //PrintStringToDiagnostics("Initiating Manufacturer Reset...");
            int failure = 0;
            failure = ResetFactory();

            //CHECK IF RESET TO FACTORY DEFAULTS WAS SUCCESSFUL
            if (failure > 0)
            {
                PrintStringToDiagnostics("Error: Reset to manufacturer defaults exception encountered in " + failure + " settings");
            }
            else
            {
                PrintStringToDiagnostics("Reset to manufacturer defaults was successful.");
            }

        }

        private int ResetFactory()
        {
            //SLED 1 Current PC
            UInt16[] readData = null;
            int failure = 0;
            double valueToWrite = Math.Round(65535.0 * mySlave.ManufacturerDefPcCurr1 / 1000 / 2.5, 0);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 30, (UInt16)valueToWrite);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 30);
            DateTime CurrentTime = DateTime.Now;

            while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

            if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
            {
                readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 30, 1);
                mySlave.ExistingPcCurr1 = ((double)readData[0] * 2.5 / 65535.0 * 1000.0);
                ExistingPcCurr1.Text = mySlave.ExistingPcCurr1.ToString("0.0") + " mA";

            }
            else
            {
                failure++;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
            }

           
            //MODBUS ID
                valueToWrite = mySlave.ManufacturerDefModbusID;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,7, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 7);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,7, 1);
                    mySlave.ExistingModbusID = (int)readData[0];
                    ExistingModbusID.Text = mySlave.ExistingModbusID.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);

            return failure;
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
        private static bool IsTextNumeric(string str)
        {
            Decimal dummy;
            return Decimal.TryParse(str, out dummy);

        }
    }
}
        
    

