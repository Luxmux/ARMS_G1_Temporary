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
                    ExistingManCurr1.Text = mySlave.ExistingManCurr1.ToString("0.0") + " mA";
                    ManufacturerDefPcCurr1.Text = mySlave.ManufacturerDefPcCurr1.ToString("0.0") + " mA";
                    ManufacturerDefManCurr1.Text = mySlave.ManufacturerDefManCurr1.ToString("0.0") + " mA";
                }
                else
                {
                    Sled1PCCurrSaveBut.IsEnabled = false;
                    TempPcCurr1.IsEnabled = false;
                    Sled1ManCurrSaveBut.IsEnabled = false;
                    TempManCurr1.IsEnabled = false;
                }


                ExistingModbusID.Text = mySlave.ExistingModbusID.ToString();
                ExistingIP.Text = mySlave.ExistingIP;
                ExistingPort.Text = mySlave.ExistingPort.ToString();
                ManufacturerDefModbusID.Text = mySlave.ManufacturerDefModbusID.ToString();
                ManufacturerDefIP.Text = mySlave.ManufacturerDefIP;
                ManufacturerDefPort.Text = mySlave.ManufacturerDefPort.ToString();

                //TEC CONTROL DEFAULTS

                    if (mySlave.Capabilities[12] != 0)
                    {
                        ExistingSTECTempSetpointDefaultBoot.Text = mySlave.ExistingSTECTempSetpointDefaultBoot.ToString("0.0") + " °C";
                        ExistingOSEBodyTECTimeConstant.Text = mySlave.ExistingOSEBodyTECTimeConstant.ToString("0.0") + " s";
                        ExistingOSEBodyTECKpFactorDefault.Text = mySlave.ExistingOSEBodyTECKpFactorDefault.ToString();
                        ExistingOSEBodyTECKdFactorDefault.Text = mySlave.ExistingOSEBodyTECKdFactorDefault.ToString();
                        ExistingOSEBodyTECKiFactorDefault.Text = mySlave.ExistingOSEBodyTECKiFactorDefault.ToString();
                        ManufacturerSTECTempSetpointDefaultBoot.Text = mySlave.ManufacturerSTECTempSetpointDefaultBoot.ToString("0.0") + " °C";
                        ManufacturerOSEBodyTECTimeConstant.Text = mySlave.ManufacturerOSEBodyTECTimeConstant.ToString() + " s";
                        ManufacturerOSEBodyTECKpFactorDefault.Text = mySlave.ManufacturerOSEBodyTECKpFactorDefault.ToString();
                        ManufacturerOSEBodyTECKdFactorDefault.Text = mySlave.ManufacturerOSEBodyTECKdFactorDefault.ToString();
                        ManufacturerOSEBodyTECKiFactorDefault.Text = mySlave.ManufacturerOSEBodyTECKiFactorDefault.ToString();
                    }
                    else
                    {
                        TempSTECTempSetpointDefaultBoot.IsEnabled = false;
                        STECTempSetpointDefaultBootSaveBut.IsEnabled = false;
                        TempOSEBodyTECTimeConstant.IsEnabled = false;
                        OSEBodyTECTimeConstantSaveBut.IsEnabled = false;
                        TempOSEBodyTECKpFactorDefault.IsEnabled = false;
                        OSEBodyTECKpFactorDefaultSaveBut.IsEnabled = false;
                        TempOSEBodyTECKdFactorDefault.IsEnabled = false;
                        OSEBodyTECKdFactorDefaultSaveBut.IsEnabled = false;
                        TempOSEBodyTECKiFactorDefault.IsEnabled = false;
                        OSEBodyTECKiFactorDefaultSaveBut.IsEnabled = false;
                    }
                    if (mySlave.Capabilities[26] != 0)
                    {
                        ExistingPMTECTempSetpointDefaultBoot.Text = mySlave.ExistingPMTECTempSetpointDefault.ToString("0.0") + " °C";
                        ExistingPMTECTimeConstant.Text = mySlave.ExistingPMTECTimeConstant.ToString("0.0") + " s";
                        ExistingPMTECKpFactorDefault.Text = mySlave.ExistingPMTECKpFactorDefault.ToString();
                        ExistingPMTECKdFactorDefault.Text = mySlave.ExistingPMTECKdFactorDefault.ToString();
                        ExistingPMTECKiFactorDefault.Text = mySlave.ExistingPMTECKiFactorDefault.ToString();
                        ManufacturerPMTECTempSetpointDefaultBoot.Text = mySlave.ManufacturerPMTECTempSetpointDefault.ToString("0.0") + " °C";
                        ManufacturerPMTECTimeConstant.Text = mySlave.ManufacturerPMTECTimeConstant.ToString("0.0") + " s";
                        ManufacturerPMTECKpFactorDefault.Text = mySlave.ManufacturerPMTECKpFactorDefault.ToString();
                        ManufacturerPMTECKdFactorDefault.Text = mySlave.ManufacturerPMTECKdFactorDefault.ToString();
                        ManufacturerPMTECKiFactorDefault.Text = mySlave.ManufacturerPMTECKiFactorDefault.ToString();
                    }
                    else
                    {
                        TempPMTECTempSetpointDefaultBoot.IsEnabled = false;
                        PMTECTempSetpointDefaultBootSaveBut.IsEnabled = false;
                        TempPMTECTimeConstant.IsEnabled = false;
                        PMTECTimeConstantSaveBut.IsEnabled = false;
                        TempPMTECKpFactorDefault.IsEnabled = false;
                        PMTECKpFactorDefaultSaveBut.IsEnabled = false;
                        TempPMTECKdFactorDefault.IsEnabled = false;
                        PMTECKdFactorDefaultSaveBut.IsEnabled = false;
                        TempPMTECKiFactorDefault.IsEnabled = false;
                        PMTECKiFactorDefaultSaveBut.IsEnabled = false;
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
                else if (tBox.Name == "TempSTECTempSetpointDefaultBoot")
                    STECTempSetpointDefaultBootSaveBut.IsDefault = true;
                else if (tBox.Name == "TempOSEBodyTECTimeConstant")
                    OSEBodyTECTimeConstantSaveBut.IsDefault = true;
                else if (tBox.Name == "TempOSEBodyTECKpFactorDefault")
                    OSEBodyTECKpFactorDefaultSaveBut.IsDefault = true;
                else if (tBox.Name == "TempOSEBodyTECKdFactorDefault")
                    OSEBodyTECKdFactorDefaultSaveBut.IsDefault = true;
                else if (tBox.Name == "TempOSEBodyTECKiFactorDefault")
                    OSEBodyTECKiFactorDefaultSaveBut.IsDefault = true;
                else if (tBox.Name == "TempModbusID")
                    ModbusIDSaveBut.IsDefault = true;
                else if (tBox.Name == "TempIP")
                    TCPIPSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPort")
                    TCPIPPortSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPMTECTempSetpointDefaultBoot")
                    PMTECTempSetpointDefaultBootSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPMTECTimeConstant")
                    PMTECTimeConstantSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPMTECKpFactorDefault")
                    PMTECKpFactorDefaultSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPMTECKdFactorDefault")
                    PMTECKdFactorDefaultSaveBut.IsDefault = true;
                else if (tBox.Name == "TempPMTECKiFactorDefault")
                    PMTECKiFactorDefaultSaveBut.IsDefault = true;
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

        private void Sled1ManCurrSaveBut_Click(object sender, RoutedEventArgs e)
        {
            Sled1ManCurrSaveBut.IsDefault = false;
            if (TempManCurr1.Text.Length > 0 && IsTextNumeric(TempManCurr1.Text) == true)
            {
                double valueToWrite = 65535.0 * double.Parse(TempManCurr1.Text) / 1000 / 2.5;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,36, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 36);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("SLED 1 Manual Mode Default Current set successful.");
                    TempManCurr1.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,36, 1);
                    mySlave.ExistingManCurr1 = ((double)readData[0] * 2.5 / 65535.0 * 1000.0);
                    ExistingManCurr1.Text = mySlave.ExistingManCurr1.ToString("0.0") + " mA";
                }
                else
                {
                    PrintStringToDiagnostics("Error: Write SLED 1 Manual Mode Default Current failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for  SLED 1 Manual Mode Default Current is not valid");
                TempManCurr1.Text = "";
            }

        }

        private void STECTempSetpointDefaultBootSaveBut_Click(object sender, RoutedEventArgs e)
        {
            STECTempSetpointDefaultBootSaveBut.IsDefault = false;
            if (TempSTECTempSetpointDefaultBoot.Text.Length > 0 && IsTextNumeric(TempSTECTempSetpointDefaultBoot.Text) == true)
            {
                double valueToWrite = Math.Round(mainWindow.CalculateSledTECRegFromTemp(double.Parse(TempSTECTempSetpointDefaultBoot.Text), (int)mySlave.FirmwareVersion), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,66, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 66);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("SLED TEC Temp Setpoint Default set successful.");
                    TempSTECTempSetpointDefaultBoot.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,66, 1);
                    mySlave.ExistingSTECTempSetpointDefaultBoot = Math.Round(mainWindow.CalculateSledTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion), 1);
                    ExistingSTECTempSetpointDefaultBoot.Text = mySlave.ExistingSTECTempSetpointDefaultBoot.ToString("0.0") + " °C";
                }
                else
                {
                    PrintStringToDiagnostics("Error: SLED TEC Temp Setpoint Default set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                TempSTECTempSetpointDefaultBoot.Text = "";
                PrintStringToDiagnostics("Error: New Value for SLED TEC Temp Setpoint Default is not valid");
            }

        }


        private void OSEBodyTECTimeConstantSaveBut_Click(object sender, RoutedEventArgs e)
        {
            OSEBodyTECTimeConstantSaveBut.IsDefault = false;

            if (TempOSEBodyTECTimeConstant.Text.Length > 0 && IsTextNumeric(TempOSEBodyTECTimeConstant.Text) == true)
            {
                double value1 = (double.Parse(TempOSEBodyTECTimeConstant.Text) * (2.25 * Math.Pow(10.0, 6))) / 65536;
                double value2 = 0;


                if (value1 >= 0)
                {
                    value2 = (double.Parse(TempOSEBodyTECTimeConstant.Text) * (2.25 * Math.Pow(10.0, 6))) - value1 * 65536;
                }

                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,55, (UInt16)value1);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 55);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {

                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,56, (UInt16)value2);
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 56);

                    CurrentTime = DateTime.Now;

                    while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                    if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                    {
                        PrintStringToDiagnostics("SLED TEC Time Constant Default set successful.");
                        TempOSEBodyTECTimeConstant.Text = "";

                        UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,55, 2);
                        mySlave.ExistingOSEBodyTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));
                        ExistingOSEBodyTECTimeConstant.Text = mySlave.ExistingOSEBodyTECTimeConstant.ToString("0.0") + " s";
                    }
                    else
                    {
                        PrintStringToDiagnostics("Error: SLED TEC Time Constant Default set failed.");
                        mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                    }


                }
                else
                {
                    PrintStringToDiagnostics("Error: SLED TEC Time Constant Default set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for SLED TEC Time Constant is not valid");
                TempOSEBodyTECTimeConstant.Text = "";
            }

        }

        private void OSEBodyTECKpFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)
        {
            OSEBodyTECKpFactorDefaultSaveBut.IsDefault = false;
            if (TempOSEBodyTECKpFactorDefault.Text.Length > 0 && IsTextNumeric(TempOSEBodyTECKpFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempOSEBodyTECKpFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,59, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 59);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("SLED TEC Kp Factor Default Boot set successful.");
                    TempOSEBodyTECKpFactorDefault.Text = "";

                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,59, 1);
                    mySlave.ExistingOSEBodyTECKpFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKpFactorDefault.Text = mySlave.ExistingOSEBodyTECKpFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("SLED TEC Kp Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for SLED TEC Kp Factor Default Boot is not valid");
                TempOSEBodyTECKpFactorDefault.Text = "";
            }

        }

        private void OSEBodyTECKdFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)
        {
            OSEBodyTECKdFactorDefaultSaveBut.IsDefault = false;

            if (TempOSEBodyTECKdFactorDefault.Text.Length > 0 && IsTextNumeric(TempOSEBodyTECKdFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempOSEBodyTECKdFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,62, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 62);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("SLED TEC Kd Factor Default Boot set successful.");
                    TempOSEBodyTECKdFactorDefault.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,62, 1);
                    mySlave.ExistingOSEBodyTECKdFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKdFactorDefault.Text = mySlave.ExistingOSEBodyTECKdFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("SLED TEC Kd Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                TempOSEBodyTECKdFactorDefault.Text = "";
                PrintStringToDiagnostics("Error: New Value for SLED TEC Kd Factor Default Boot is not valid");
            }

        }

        private void OSEBodyTECKiFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)
        {
            OSEBodyTECKiFactorDefaultSaveBut.IsDefault = false;
            if (TempOSEBodyTECKiFactorDefault.Text.Length > 0 && IsTextNumeric(TempOSEBodyTECKiFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempOSEBodyTECKiFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,64, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 64);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("SLED TEC Ki Factor Default Boot set successful.");
                    TempOSEBodyTECKiFactorDefault.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,64, 1);
                    mySlave.ExistingOSEBodyTECKiFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKiFactorDefault.Text = mySlave.ExistingOSEBodyTECKiFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("SLED TEC Ki Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for SLED TEC Ki Factor Default Boot is not valid");
                TempOSEBodyTECKiFactorDefault.Text = "";
            }

        }

        private void PMTECTempSetpointDefaultBootSaveBut_Click(object sender, RoutedEventArgs e)
        {
            PMTECTempSetpointDefaultBootSaveBut.IsDefault = false;
            if (TempPMTECTempSetpointDefaultBoot.Text.Length > 0 && IsTextNumeric(TempPMTECTempSetpointDefaultBoot.Text) == true)
            {
                double valueToWrite = Math.Round(mainWindow.CalculatePMTECRegFromTemp(double.Parse(TempPMTECTempSetpointDefaultBoot.Text), (int)mySlave.FirmwareVersion), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,48, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 48);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("PM TEC Temp Setpoint Default set successful.");
                    TempPMTECTempSetpointDefaultBoot.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,48, 1);
                    mySlave.ExistingPMTECTempSetpointDefault = Math.Round(mainWindow.CalculatePMTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion), 1);
                    ExistingPMTECTempSetpointDefaultBoot.Text = mySlave.ExistingPMTECTempSetpointDefault.ToString("0.0") + " °C";
                }
                else
                {
                    PrintStringToDiagnostics("Error: PM TEC Temp Setpoint Default set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                TempPMTECTempSetpointDefaultBoot.Text = "";
                PrintStringToDiagnostics("Error: New Value for PM TEC Temp Setpoint Default is not valid");
            }

        }

        private void PMTECTimeConstantSaveBut_Click(object sender, RoutedEventArgs e)

        { 
            PMTECTimeConstantSaveBut.IsDefault = false;

            if (TempPMTECTimeConstant.Text.Length > 0 && IsTextNumeric(TempPMTECTimeConstant.Text) == true)
            {
                double value1 = (double.Parse(TempPMTECTimeConstant.Text) * (2.25 * Math.Pow(10.0, 6))) / 65536;
                double value2 = 0;


                if (value1 >= 0)
                {
                    value2 = (double.Parse(TempPMTECTimeConstant.Text) * (2.25 * Math.Pow(10.0, 6))) - value1 * 65536;
                }

                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,70, (UInt16)value1);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 70);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {

                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,71, (UInt16)value2);
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 71);

                    CurrentTime = DateTime.Now;

                    while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                    if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                    {
                        PrintStringToDiagnostics("PM TEC Time Constant Default set successful.");
                        TempPMTECTimeConstant.Text = "";

                        UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,70, 2);
                        mySlave.ExistingPMTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));
                        ExistingPMTECTimeConstant.Text = mySlave.ExistingPMTECTimeConstant.ToString("0.0") + " s";
                    }
                    else
                    {
                        PrintStringToDiagnostics("Error: PM TEC Time Constant Default set failed.");
                        mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                    }


                }
                else
                {
                    PrintStringToDiagnostics("Error: PM TEC Time Constant Default set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for PM TEC Time Constant is not valid");
                TempPMTECTimeConstant.Text = "";
            }

        }

        private void PMTECKpFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)
        {

            PMTECKpFactorDefaultSaveBut.IsDefault = false;
            if (TempPMTECKpFactorDefault.Text.Length > 0 && IsTextNumeric(TempPMTECKpFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempPMTECKpFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,74, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 74);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("PM TEC Kp Factor Default Boot set successful.");
                    TempPMTECKpFactorDefault.Text = "";

                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,74, 1);
                    mySlave.ExistingPMTECKpFactorDefault = (int)readData[0];
                    ExistingPMTECKpFactorDefault.Text = mySlave.ExistingPMTECKpFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("PM TEC Kp Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for PM TEC Kp Factor Default Boot is not valid");
                TempPMTECKpFactorDefault.Text = "";
            }

        }

        private void PMTECKdFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)
        {

            PMTECKdFactorDefaultSaveBut.IsDefault = false;

            if (TempPMTECKdFactorDefault.Text.Length > 0 && IsTextNumeric(TempPMTECKdFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempPMTECKdFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,76, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 76);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("PM TEC Kd Factor Default Boot set successful.");
                    TempPMTECKdFactorDefault.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,76, 1);
                    mySlave.ExistingPMTECKdFactorDefault = (int)readData[0];
                    ExistingPMTECKdFactorDefault.Text = mySlave.ExistingPMTECKdFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("PM TEC Kd Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                TempPMTECKdFactorDefault.Text = "";
                PrintStringToDiagnostics("Error: New Value for PM TEC Kd Factor Default Boot is not valid");
            }

        }

        private void PMTECKiFactorDefaultSaveBut_Click(object sender, RoutedEventArgs e)

        { 
            PMTECKiFactorDefaultSaveBut.IsDefault = false;
            if (TempPMTECKiFactorDefault.Text.Length > 0 && IsTextNumeric(TempPMTECKiFactorDefault.Text) == true)
            {
                double valueToWrite = Math.Round(double.Parse(TempPMTECKiFactorDefault.Text), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,78, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 78);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("PM TEC Ki Factor Default Boot set successful.");
                    TempPMTECKiFactorDefault.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,78, 1);
                    mySlave.ExistingPMTECKiFactorDefault = (int)readData[0];
                    ExistingPMTECKiFactorDefault.Text = mySlave.ExistingPMTECKiFactorDefault.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("PM TEC Ki Factor Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for PM TEC Ki Factor Default Boot is not valid");
                TempPMTECKiFactorDefault.Text = "";
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

        private void TCPIPSaveBut_Click(object sender, RoutedEventArgs e)
        {
            TCPIPSaveBut.IsDefault = false;
            string[] parsme = TempIP.Text.Split('.');

            if (TempIP.Text.Length > 0 && IsTextNumeric(parsme[0]) == true && IsTextNumeric(parsme[1]) == true && IsTextNumeric(parsme[2]) == true && IsTextNumeric(parsme[3]) == true)
            {
                UInt16 firstregister = (ushort)((UInt16.Parse(parsme[0]) << 8) + UInt16.Parse(parsme[1]));
                UInt16 scndregister = (ushort)((UInt16.Parse(parsme[2]) << 8) + UInt16.Parse(parsme[3]));


                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,1, (UInt16)firstregister);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 1);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {


                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,2, (UInt16)scndregister);
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 2);
                    CurrentTime = DateTime.Now;

                    while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                    if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                    {
                        UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,1, 2);

                        int ExistingIP1 = readData[0] >> 8 & 0x00ff;
                        int ExistingIP2 = readData[0] & 0x00ff;
                        int ExistingIP3 = readData[1] >> 8 & 0x00ff;
                        int ExistingIP4 = readData[1] & 0x00ff;

                        mySlave.ExistingIP = ExistingIP1.ToString() + "." + ExistingIP2.ToString() + "." + ExistingIP3.ToString() + "." + ExistingIP4.ToString();
                        ExistingIP.Text = mySlave.ExistingIP;
                        PrintStringToDiagnostics("Default IP set successful.");
                        TempIP.Text = "";
                    }
                    else
                    {
                        PrintStringToDiagnostics("Error: Default IP set failed.");
                        mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                    }

                }
                else
                {
                    PrintStringToDiagnostics("Error: Default IP set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }
            }
            else
            {
                PrintStringToDiagnostics("Error: New Value for Default IP is not valid");
                TempIP.Text = "";
            }

        }

        private void TCPIPPortSaveBut_Click(object sender, RoutedEventArgs e)
        {
            TCPIPPortSaveBut.IsDefault = false;
            if (TempPort.Text.Length > 0 && IsTextNumeric(TempPort.Text) == true)
            {
                double valueToWrite = double.Parse(TempPort.Text);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,5, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 5);

                DateTime CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    PrintStringToDiagnostics("TCP IP Port address Default Boot set successful.");
                    TempPort.Text = "";
                    UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,5, 1);
                    mySlave.ExistingPort = (int)readData[0];
                    ExistingPort.Text = mySlave.ExistingPort.ToString("");
                }
                else
                {
                    PrintStringToDiagnostics("TCP IP Port address Default Boot set failed.");
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);
                }

            }
            else
            {
                PrintStringToDiagnostics("Error: New Value fo TCP IP Port address Default Boot is not valid");
                TempPort.Text = "";
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

            //SLED 1 Current MANUAL
            valueToWrite = Math.Round(65535.0 * mySlave.ManufacturerDefManCurr1 / 1000 / 2.5, 0);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 36, (UInt16)valueToWrite);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 36);
            CurrentTime = DateTime.Now;

            while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

            if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
            {
                readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 36, 1);
                mySlave.ExistingManCurr1 = ((double)readData[0] * 2.5 / 65535.0 * 1000.0);
                ExistingManCurr1.Text = mySlave.ExistingManCurr1.ToString("0.0") + " mA";

            }
            else
            {
                failure++;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
            }

            //SLED TEC SETPOINT
            valueToWrite = Math.Round(mainWindow.CalculateSledTECRegFromTemp(mySlave.ManufacturerSTECTempSetpointDefaultBoot, (int)mySlave.FirmwareVersion), 0);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 66, (UInt16)valueToWrite);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 66);

            CurrentTime = DateTime.Now;

            while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

            if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
            {
                readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 66, 1);
                mySlave.ExistingSTECTempSetpointDefaultBoot = Math.Round(mainWindow.CalculateSledTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion), 1);
                ExistingSTECTempSetpointDefaultBoot.Text = mySlave.ExistingSTECTempSetpointDefaultBoot.ToString("0.0") + " °C";

            }
            else
            {
                failure++;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
            }
            if (mySlave.Capabilities[12] != 0)
            {
                //OSE TEC Time Constant
                double value1 = (mySlave.ManufacturerOSEBodyTECTimeConstant * (2.25 * Math.Pow(10.0, 6))) / 65536;
                double value2 = 0;


                if (value1 >= 0)
                {
                    value2 = (mySlave.ManufacturerOSEBodyTECTimeConstant * (2.25 * Math.Pow(10.0, 6))) - value1 * 65536;
                }
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 55, (UInt16)value1);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 55);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {


                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 56, (UInt16)value2);
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 56);

                    CurrentTime = DateTime.Now;

                    while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                    if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                    {
                        readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 55, 2);
                        mySlave.ExistingOSEBodyTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));
                        ExistingOSEBodyTECTimeConstant.Text = mySlave.ExistingOSEBodyTECTimeConstant.ToString("0.0") + " s";
                    }
                    else
                    {
                        failure++;
                        mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                    }

                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }





                //OSE Kp Factor
                valueToWrite = Math.Round(mySlave.ManufacturerOSEBodyTECKpFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 59, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 59);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {

                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 59, 1);
                    mySlave.ExistingOSEBodyTECKpFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKpFactorDefault.Text = mySlave.ExistingOSEBodyTECKpFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }

                //OSE Kd Factor
                valueToWrite = Math.Round(mySlave.ManufacturerOSEBodyTECKdFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 62, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 62);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {


                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 62, 1);
                    mySlave.ExistingOSEBodyTECKdFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKdFactorDefault.Text = mySlave.ExistingOSEBodyTECKdFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }

                //OSE Ki Factor
                valueToWrite = Math.Round(mySlave.ManufacturerOSEBodyTECKiFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 64, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 64);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {
                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 64, 1);
                    mySlave.ExistingOSEBodyTECKiFactorDefault = (int)readData[0];
                    ExistingOSEBodyTECKiFactorDefault.Text = mySlave.ExistingOSEBodyTECKiFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);

                }
            }

            if (mySlave.Capabilities[26] != 0)
            {
                //PM TEC SETPOINT
                valueToWrite = Math.Round(mainWindow.CalculatePMTECRegFromTemp(mySlave.ManufacturerPMTECTempSetpointDefault, (int)mySlave.FirmwareVersion), 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 48, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 48);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {
                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 48, 1);
                    mySlave.ExistingPMTECTempSetpointDefault = Math.Round(mainWindow.CalculatePMTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion), 1);
                    ExistingPMTECTempSetpointDefaultBoot.Text = mySlave.ExistingPMTECTempSetpointDefault.ToString("0.0") + " °C";

                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }


                //PM TEC Time Constant
                double value1 = (mySlave.ManufacturerPMTECTimeConstant * (2.25 * Math.Pow(10.0, 6))) / 65536;
                double value2 = 0;


                if (value1 >= 0)
                {
                    value2 = (mySlave.ManufacturerPMTECTimeConstant * (2.25 * Math.Pow(10.0, 6))) - value1 * 65536;
                }
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 70, (UInt16)value1);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 70);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {


                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 71, (UInt16)value2);
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 71);

                    CurrentTime = DateTime.Now;

                    while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                    if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                    {
                        readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 70, 2);
                        mySlave.ExistingPMTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));
                        ExistingPMTECTimeConstant.Text = mySlave.ExistingPMTECTimeConstant.ToString("0.0") + " s";
                    }
                    else
                    {
                        failure++;
                        mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                    }

                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }





                //PM Kp Factor
                valueToWrite = Math.Round(mySlave.ManufacturerPMTECKpFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 74, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 74);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {

                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 74, 1);
                    mySlave.ExistingPMTECKpFactorDefault = (int)readData[0];
                    ExistingPMTECKpFactorDefault.Text = mySlave.ExistingPMTECKpFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }

                //PM Kd Factor
                valueToWrite = Math.Round(mySlave.ManufacturerPMTECKdFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 76, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 76);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {


                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 76, 1);
                    mySlave.ExistingPMTECKdFactorDefault = (int)readData[0];
                    ExistingPMTECKdFactorDefault.Text = mySlave.ExistingPMTECKdFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);
                }

                //PM Ki Factor
                valueToWrite = Math.Round(mySlave.ManufacturerPMTECKiFactorDefault, 0);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 78, (UInt16)valueToWrite);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 78);

                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 22, 1)[0] == 999)
                {
                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 78, 1);
                    mySlave.ExistingPMTECKiFactorDefault = (int)readData[0];
                    ExistingPMTECKiFactorDefault.Text = mySlave.ExistingPMTECKiFactorDefault.ToString("");
                }
                else
                {
                    failure++;
                    mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 22, 999);

                }
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
            


            string[] parsme = mySlave.ManufacturerDefIP.Split('.');
            UInt16 firstregister = (ushort)((UInt16.Parse(parsme[0]) << 8) + UInt16.Parse(parsme[1]));
            UInt16 scndregister = (ushort)((UInt16.Parse(parsme[2]) << 8) + UInt16.Parse(parsme[3]));


            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,1, (UInt16)firstregister);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 1);

            CurrentTime = DateTime.Now;

            while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

            if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
            {


                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,2, (UInt16)scndregister);
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 2);
                CurrentTime = DateTime.Now;

                while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

                if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
                {
                    readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,1, 2);

                    int ExistingIP1 = readData[0] >> 8 & 0x00ff;
                    int ExistingIP2 = readData[0] & 0x00ff;
                    int ExistingIP3 = readData[1] >> 8 & 0x00ff;
                    int ExistingIP4 = readData[1] & 0x00ff;

                    mySlave.ExistingIP = ExistingIP1.ToString() + "." + ExistingIP2.ToString() + "." + ExistingIP3.ToString() + "." + ExistingIP4.ToString();
                    ExistingIP.Text = mySlave.ExistingIP;

                }
                else
                    failure++;
                mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);

            }
            else
                failure++;
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 999);

            // IP Port
            valueToWrite = mySlave.ManufacturerDefPort;
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,5, (UInt16)valueToWrite);
            mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,22, 5);

            CurrentTime = DateTime.Now;

            while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] != (UInt16)999 && DateTime.Now < CurrentTime.AddSeconds(2)) { }

            if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,22, 1)[0] == 999)
            {
                readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,5, 1);
                mySlave.ExistingPort = (int)readData[0];
                ExistingPort.Text = mySlave.ExistingPort.ToString("");
            }
            else
                failure++;
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
        
    

