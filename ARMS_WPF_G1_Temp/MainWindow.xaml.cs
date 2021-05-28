using EasyModbus;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ARMS_WPF_G1_Temp
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int MAX_NUM_SLAVES = 20;
        public const int MANUAL_DISCONNECT = -2;
        public const int IDLE_CYCLE = -1;
        public const int CONTINUOUS_READ = 2;
        public const int NO_CHART_MODE = -1;
        public const int SIGNAL_ON = 1;
        public const int CONNECTION_CHECK = 5;
        public const int NUM_LOGFILE_FIELDS = 15;
        public const int DATETIME_OFFSET = 2;
        public const int ID_OFFSET = 3;
        public const int NUM_DISPLAY_CHARS = 3000;
        public const int MAX_CHART_VALUES = 100;
        const int MAX_CONNECTION_ATTEMPTS = 10; //Maximum number of connection attempts while polling BeST SLED
        const int MAX_RETRY_ATTEMPTS = 10;
        const int CHECKING_COMM_PORTS_STATE_ZERO = 0;      //Finite state machine state 0 (initial state)
        const int PULLING_DEFAULTS_STATE_ONE = 1;
        const int GENERAL_POLL_STATE_TWO = 2;
        const int MAX_CONNECTION_ATTEMPTS_REACHED_STATE_THREE = 3;
        const int CONNECTION_SETTINGS_CHANGED_STATE_FOUR = 4;
        const int SOFTWARE_RESET_STATE_FIVE = 5;
        const int TCP_PORT = 502;
        const int INPUT_REGISTER_OPTION = 0;
        const int HOLDING_REGISTER_OPTION = 1;
        const int UNLOCKED = 1;
        const int LOCKED = 0;
        const int NO_MODE_SELECTED = 3;

        const int MANUAL_CONNECT_MODE = 0;
        const int AUTO_SCAN_MODE = 1;
        const int IDLE_SCAN_MODE = 2;
        const int USB_COMMS = 0;
        const int RS232_COMMS = 1;
        const int TCP_COMMS = 2;

        const int HEAT_SINK_MAX_TEMP = 100;
        const int HEAT_SINK_MIN_TEMP = -80;
        const int SLED_TEC_MAX_TEMP = 100;
        const int SLED_TEC_MIN_TEMP = 10;
        const int SLED_TEC_MAX_RAW_TEMP = 61452;
        const int SLED_TEC_MIN_RAW_TEMP = 22119;
        const int PM_TEC_MAX_TEMP = 100;
        const int PM_TEC_MIN_TEMP = 10;
        const int PM_TEC_MAX_RAW_TEMP = 59649;
        const int PM_TEC_MIN_RAW_TEMP = 25040;


        const int MW_UNITS = 0;
        const int DBM_UNITS = 1;
        const int CHART_CURRENT_MODE = 0;
        const int CHART_POWER_MODE = 1;

        //Modulation Form Threading Functions
        const int ENABLE_BUT_CLICK = 1;
        const int MOD_BUT_CLICK = 2;
        const int MOD_FREQ_SAVE_BUT_CLICK = 3;
        const int DUTY_CYCLE_SAVE_BUT_CLICK = 4;

        //Master Thread Functions
        const int SLED_TEC_SAVE_TEMP_CLICK = 1;
        const int ALL_SLEDS_ON_OFF_CLICK = 2;
        const int SLED_TEC_ON_OFF_CLICK = 3;
        const int FAN_SPEED_SAVE_CLICK = 4;

        //About Thread Functions
        const int READ_INPUT_REGISTER = 4;
        const int READ_HOLDING_REGISTER = 3;
        const int WRITE_SINGLE_REGISTER = 6;

        //Defaults Thread Functions
        const int SLED_TEC_TEMP_SAVE_CLICK = 1;
        const int PC_CURR_SAVE_CLICK = 2;
        const int MAN_CURR_SAVE_CLICK = 3;
        const int MODBUS_ID_SAVE_CLICK = 4;
        const int IP_SAVE_CLICK = 5;
        const int FACTORY_RESET_BUT_CLICK = 6;
        const int ERROR_THRESHOLD_BUT_CLICK = 7;
        const int TEC_DELAY_BUT_CLICK = 8;
        const int K_VALUE_BUT_CLICK = 9;
        const int PORT_SAVE_CLICK = 10;

        //Line chart variables
        public List<string> CurrentValues_Labels = new List<string>();
        public List<string> PublicCurrentValues_Labels = new List<string>();
        public List<string> RangedValues_Labels = new List<string>();

        private string latestTime = "";
        public static TabControl tbControl;
        MainWindow mainWindow;
        public static bool charton = false;
        public bool attemptingConnect = true;
        public ModbusClient mbClient;
        SerialPort serial = new SerialPort();
        public static string debugBuffer = "";
        public TextBox diagnosticsWindow;
        public SolidColorBrush mySolidColorBrush;
        public static ModBusSlave[] modbusSlaveList = new ModBusSlave[MAX_NUM_SLAVES + 1];  //0 not used so we add 1
        public Dictionary<int, int> selectedTabSlaveID = new Dictionary<int, int>();
        private bool looping = true;
        private bool graphing = false;
        private int graphing_limit = 2000;
        public bool commslogging = false;
        public int linenumber = 1;

        public bool loggingToFiles = false;
        public bool showRawValues = false;
        private string[] logFileValues = new string[NUM_LOGFILE_FIELDS];

        public Dictionary<string, string> publicLogFilePathStrings = new Dictionary<string, string>();
        public Dictionary<string, string> adminLogFilePathStrings = new Dictionary<string, string>();

        public static bool debugOn = false;
        Thread myThread;
        private double slider1ManualModeSetpoint = 0;

        private double boardTemperature = 0;

        private double sledTECTempGood = 0;

        public int selectedSlaveID = 0;

        private double s1CurrentSetpoint = 0;

        private double Slider1Changed = 0;
        private double Slider1_temp = 0;


        private int mode = 0;
        private string whichMode = "";
        private string modeString = "";

        public Admin adminWindow;
        public Communications commsWindow;
        private Logging loggingWindow;
        public Password passWindow;
        public About aboutWindow;
        public Defaults defaultsWindow;
        public Photosensitivity photosensitivityWindow;

        public int Current1Old = 0;

        public int firstpass = 1;
        public int isadmin = 0; //if you've logged into admin
        public int connectionTypeInt = 0;
        public int ExistingModbusID = 255;
        public bool foundbestsled = false;
        public bool autoscanning = false;
        public bool manual_connection = false;
        public string new_ComType_string = "";
        public string new_ComPort_string = "";

        List<string> Series0_axisX_label = new List<string>();


        // UInt16 modFreqHighBytes = 0;
        //UInt16 modFreqLowBytes = 0;
        //UInt16 dutyCycleHighBytes = 0;
        //UInt16 dutyCycleLowBytes = 0;


        
        private string fanSpeedSetPointEdit_temp;
        private bool temp_too_hot = false;
        public MainWindow()
        {
            InitializeComponent();
            PrintStringToDiagnostics("Starting up...");
            PrintStringToDiagnostics("Auto Scanning COM Ports...");
            autoscanning = true;
            commsWindow = new Communications(this);
            commsWindow.Visibility = Visibility.Hidden;

            aboutWindow = new About(this);
            aboutWindow.Visibility = Visibility.Hidden;

            photosensitivityWindow = new Photosensitivity(this);
            photosensitivityWindow.Visibility = Visibility.Hidden;

            ConnectToBestSLED();

            //Current readings graph
            ValuesChart.AxisY[0].LabelFormatter = value => value.ToString("0.00");


            ValuesChart.AxisX.Add(new Axis
            {
                Labels = (IList<string>)Series0_axisX_label,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 1 Raw",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 1 [V]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 2 Raw",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 2 [V]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });
            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 3 Raw",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 3 [V]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });
            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 4 Raw",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Gas 4 [V]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });


            this.Dispatcher.Invoke(() =>
            {
                AboutBut.IsEnabled = true;
                CommunicationBut.IsEnabled = true;

                AdminBut.IsEnabled = false;
                maxBut.IsEnabled = false;
                minBut.IsEnabled = false;

                FanSpeedSetBut.IsEnabled = false;
                Start_PM.IsEnabled = false;
                Stop_PM.IsEnabled = false;
                Clear_PM.IsEnabled = false;

                sledsOnBut.IsEnabled = false;

                LoggingBut.IsEnabled = false;
                DefaultsBut.IsEnabled = false;
                ExportBut_PM.IsEnabled = false;


                fanSpeedSetPointEdit.IsEnabled = false;
                List_PM.IsEnabled = false;

                slider1TrackBar.IsEnabled = false;

                setCurr1Edit.IsEnabled = false;

            });
        }


        public void ConnectToBestSLED()
        {

            Task.Delay(10).ContinueWith(_ =>
            {
                while (foundbestsled == false)
                {
                    if (autoscanning)
                    {
                        AutoScanPorts();
                    }
                    else
                    {
                        if (manual_connection)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                commsWindow.Manual_ConnectBut.IsEnabled = false;
                            });
                            ManualConnect();
                        }
                    }
                }
            });
        }

        private void AutoScanPorts()
        {
            string Comm_Port_Names = "COM";
            int comtotry = 1;
            int connectStatus = 0;
            foundbestsled = false;
            bool newconnection = false;

            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            this.Dispatcher.Invoke(() =>
            {
                datedisplay.Content = DateTime.Now;
            });

            DateTime dateTimeStart = DateTime.Now;
            while (foundbestsled == false && ((DateTime.Now.Ticks - dateTimeStart.Ticks) < 40000000))
            {
                //Acquire serial ports
                string[] sPorts = SerialPort.GetPortNames();
                var sortedList = sPorts.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));

                foreach (string port in sortedList)
                {
                    this.mbClient = new ModbusClient(port, this);
                    connectStatus = mbClient.Connect();
                    if (connectStatus == 1) //check if it's best led
                    {
                        UInt16[] bytesRead = this.mbClient.ReadInputRegisters(255, 0, 5);
                        //Confirm unit ID
                        if (bytesRead[0] == 0x4152 &&
                            bytesRead[1] == 0x4D53 &&
                            bytesRead[2] == 0x5345 &&
                            bytesRead[3] == 0x4E53 &&
                            bytesRead[4] == 0x4F52) // if  id confirmed
                        {
                            foundbestsled = true;
                            newconnection = true;
                        }
                    }
                    comtotry = Convert.ToInt32(port.Replace("COM", ""));

                }
            }

                if (foundbestsled && newconnection)
                {
                ModBusSlave newSlave = new ModBusSlave();
                newSlave.SlaveID = comtotry;
                modbusSlaveList[newSlave.SlaveID] = newSlave;
                selectedSlaveID = comtotry; // set the active selected slave for all functions going forward
                PrintStringToDiagnostics("ARMS G1 ID verified successfully");
                UpdateFromCommsAsync("COM" + (comtotry).ToString(), "COM");
                
                this.Dispatcher.Invoke(() =>
                {
                    commsWindow.Visibility = Visibility.Hidden; 
                    datedisplay.Content = DateTime.Now;
                });

                newconnection = false;

                }
        }

        private void ManualConnect()
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];
            try
            {
                int connectStatus = 0;
                int manualtry = 1;
                foundbestsled = false;
                bool newconnection = false;

                this.Dispatcher.Invoke(() =>
                {
                    //datedisplay.Content = DateTime.Now;
                    while (foundbestsled == false && manualtry < 10)
                    {
                        if ((new_ComPort_string != ""))
                        {
                            this.mbClient = new ModbusClient(new_ComPort_string, this);
                            connectStatus = mbClient.Connect();
                            if (connectStatus == 1) //check if it's best led
                            {
                                UInt16[] bytesRead = this.mbClient.ReadInputRegisters(255,0, 6);
                                //Confirm unit ID
                                if (bytesRead[0] == 0x5349 &&
                                    bytesRead[1] == 0x4E47 &&
                                    bytesRead[2] == 0x4C45 &&
                                    bytesRead[3] == 0x534C &&
                                    bytesRead[4] == 0x4544 &&
                                    bytesRead[5] == 0x4731) // if "Best sled" id confirmed
                                {
                                    foundbestsled = true;
                                    newconnection = true;
                                }
                            }
                        }
                        else
                        {

                            new_ComType_string = commsWindow.ComType.Text;
                            new_ComPort_string = commsWindow.ComPortBox.Text;

                        }
                        manualtry++;
                    }


                    if (foundbestsled && newconnection)
                    {
                        ModBusSlave newSlave = new ModBusSlave();
                        newSlave.SlaveID = int.Parse(new_ComPort_string.Replace("COM", ""));
                        modbusSlaveList[newSlave.SlaveID] = newSlave;
                        selectedSlaveID = newSlave.SlaveID; // set the active selected slave for all functions going forward
                        PrintStringToDiagnostics("Single-SLED ID verified successfully");
                        UpdateFromCommsAsync(new_ComPort_string, "COM");
                        datedisplay.Content = DateTime.Now;
                        commsWindow.Visibility = Visibility.Hidden;


                    }
                    else
                    {
                        PrintStringToDiagnostics("Manual Connection failed. Please review settings and try again.");
                        manual_connection = false;
                        datedisplay.Content = DateTime.Now;
                        this.Dispatcher.Invoke(() =>
                        {
                            commsWindow.Manual_ConnectBut.IsEnabled = true;
                        });

                    }
                });
                newconnection = false;
            }
            catch
            {
                PrintStringToDiagnostics("Manual Connection failed. Please review settings and try again.");
                manual_connection = false;
                datedisplay.Content = DateTime.Now;
                this.Dispatcher.Invoke(() =>
                {
                    commsWindow.Manual_ConnectBut.IsEnabled = true;
                });
                ConnectToBestSLED();
            }
        }

        public void InitializePublicCSVFile(ModBusSlave mySlave)
        {

            string s = "Date," +
                        "Time," +
                        "Lamp ON/OFF," +

                        "Lamp Drive [mA]," +
                        "Lamp Current Sense [mA]," +
                        "Lamp Operating Mode,";

            s = s + "Board Temp [C],";

                s = s + "Thermopile Temp [C],";

                s = s +
                    "Gas 1 Reading [V],"+
                    "Gas 2 Reading [V]," +
                    "Gas 3 Reading [V]," +
                    "Gas 4 Reading [V],";


            s = s +
                "Fan Speed Set [CFM]," +
                "Fan Speed Read [CFM],";


            s = s +
                "Lamp ON/OFF Raw," +
                "Lamp Drive Raw," +
                "Lamp Current Sense Raw," +
                "Lamp Operating Mode,";


                s = s + "Board Temp Raw,";


            s = s + "Thermopile Temp Raw,";

    

            s = s +
                "Gas 1 Reading Raw," +
                "Gas 2 Reading Raw," +
                "Gas 3 Reading Raw," +
                "Gas 4 Reading Raw,";


            s = s +
                "Fan Speed Set Raw," +
                "Fan Speed Read Raw\n";    //important that last header field end with '\n' not a ','

            mySlave.slavePublicLogFileFsWriter.Write(s);

            mySlave.slavePublicLogFileFsWriter.Flush();
        }
       

        public void AboutBut_Click(object sender, EventArgs e)
        {
                aboutWindow.Visibility = Visibility.Visible;
                aboutWindow.Activate();
                aboutWindow.WindowState = WindowState.Normal;

        }

        public void DefaultsBut_Click(object sender, EventArgs e)
        {
            if (selectedSlaveID != 0)
            {
                defaultsWindow.Visibility = Visibility.Visible;
                defaultsWindow.Activate();
                defaultsWindow.WindowState = WindowState.Normal;
            }
            else
                MessageBox.Show("Not connected device detected.");
        }


        private void LoggingBut_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSlaveID != 0)
            {
                loggingWindow.Visibility = Visibility.Visible;
                loggingWindow.Activate();
                loggingWindow.WindowState = WindowState.Normal;


            }
            else
                MessageBox.Show("Not connected device detected.");
        }


        public void CommsBut_Click(object sender, EventArgs e)
        {

            commsWindow.Visibility = Visibility.Visible;
            commsWindow.Activate();
            commsWindow.WindowState = WindowState.Normal;


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

        public void ToggleGUIElements(bool displayElements, int slaveID)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (displayElements)
                {
                    AboutBut.IsEnabled = true;
                }
                else
                {

                }
            });
        }

        public void UpdateFromCommsAsync(string connectto, string protocol) //create all tabs and update modbus initialization
        {

            foreach (ModBusSlave mySlave in modbusSlaveList)
            {
                if (mySlave != null)
                {

                    //ToggleGUIElements(true, mySlave.SlaveID);


                    PrintStringToDiagnostics("ARMS found on COM Port " + mySlave.SlaveID.ToString());

                    PrintStringToDiagnostics("Loading Configuration...");

                    //Wait for boot done bit
                    while (mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 12, 1)[0] == 0)
                    {
                        //NOP
                        //Should put a timeout here
                    }
                    PrintStringToDiagnostics("Boot done.");
                    SetStateSignal(IDLE_CYCLE, mySlave.SlaveID);

                    UInt16[] readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 0, 1);
                    //mySlave.ModbusID = readData[0];

                    ReadAndSetDefaultsAndCapabilities(mySlave); // do first check stuff for each slave id
                    aboutWindow.UpdateAboutGUI(mySlave);


                    this.Dispatcher.Invoke(() =>
                    {
                        //start admin with selected slave
                        passWindow = new Password(this); //set up pre admin password
                        passWindow.Visibility = Visibility.Hidden;
                        adminWindow = new Admin(this);   //set up an admin window for polling
                        mySlave.AdminLogginBut = false;

                        //start logging window
                        loggingWindow = new Logging(this);
                        loggingWindow.Visibility = Visibility.Hidden;

                        //start defaults window
                        defaultsWindow = new Defaults(this);
                        defaultsWindow.Visibility = Visibility.Hidden;
                    });


                }
            }
            this.Dispatcher.Invoke(() =>
            {


                KickOffAllThreads();


            });

        }

        private void ReadAndSetDefaultsAndCapabilities(ModBusSlave mySlave)
        {

            //Itziar Finish this part
            //read firmware 
            UInt16[] readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,6, 1);
            mySlave.FirmwareVersion = readData[0];

            this.Dispatcher.Invoke(() =>
            {
                AdminBut.IsEnabled = true;
                LoggingBut.IsEnabled = true;
                DefaultsBut.IsEnabled = true;
                //read model
                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,999, 1);
                mySlave.Model = readData[0];
                
            });

            //read mode and capabilities
            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,7, 1);
            mySlave.Mode = readData[0];


            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,100, 34);
            mySlave.Capabilities[0] = readData[0]; //SLED 1: 0 if DNE otherwise wavelength

            mySlave.Capabilities[6] = (int)Math.Round(((readData[6]) * 2.5 / (65535.0) * 1000), 1); //SLED 1: Max current limit & max manufacture default

            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.Capabilities[0] > 0)
                {
                    slider1TrackBar.Maximum = mySlave.Capabilities[6];
                    setCurr1Edit.IsEnabled = true;
                    setCurr1Edit.Text = "0.0 mA";
                }
            });



            this.Dispatcher.Invoke(() =>
            {

                FanSpeedSetBut.IsEnabled = true;
                fanSpeedSetPointEdit.IsEnabled = true;
            });

            //capabilities[13] =
            mySlave.Capabilities[14] = readData[14];    //Heat Sink Temp: 0 if DNE, 1 if enabled
            mySlave.Capabilities[15] = readData[15];    //SLED 6 Submount Temp: 0 if DNE, 1 if enabled
            this.Dispatcher.Invoke(() =>
            {

                //Capabilities 26: PM TEC Default Temp
                mySlave.Capabilities[16] = (readData[16]);

                if (mySlave.Capabilities[16] != 0)
                {
                    Start_PM.IsEnabled = true;
                    Stop_PM.IsEnabled = false;
                    Clear_PM.IsEnabled = true;
                    
                    ExportBut_PM.IsEnabled = false;
                    List_PM.IsEnabled = true;
                    List_PM.Text = Gas1_Raw.Content.ToString();
                    mySlave.PMWavelength = 1550;


                }
            });

            mySlave.Capabilities[18] = (int)(2.5 * ((readData[18]) / 65535.0) * 1000); //SLED 1 PC current manufacture default


            mySlave.Capabilities[32] = readData[32];            //Board Temperature


            this.Dispatcher.Invoke(() =>
            {

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 8, 1);
                if (readData[0] == 0)
                    mySlave.Locked = LOCKED;
                else
                    mySlave.Locked = UNLOCKED;

                if (mySlave.Locked == LOCKED)
                {
                    mySlave.CurrentMode = mySlave.Mode;
                    PrintStringToDiagnostics("Lamp is Locked");
                    mySlave.LampEnable = 0;

                    //disable modulation button

                    //Check if sled Math.Power is on according to the unit
                    UInt16[] readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,16, 1);

                    if (readData2[0] == 1)
                    {
                        sledsOnBut.Content = "         On ";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                    }
                    else
                    {
                        mySlave.SledsAreOn = 0;
                        sledsOnBut.Content = "         On ";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));

                    }



                    maxBut.IsEnabled = false;
                    minBut.IsEnabled = false;
                    sledsOnBut.IsEnabled = false;

                    slider1TrackBar.IsEnabled = false;

                    setCurr1Edit.IsEnabled = false;


                    //capabilities
                    readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 1);
                    if (mySlave.Capabilities[0] > 0)
                    {
                        //Each poll cycle read current from 20-25 and set slider accordingly

                        slider1ManualModeSetpoint = readData2[0] * 2.5 / 65535.0 * 1000;
                        if (slider1ManualModeSetpoint > 550) //maximum slider
                        {
                            slider1ManualModeSetpoint = 550;
                        }
                        Slider1Changed = slider1ManualModeSetpoint;
                        Slider1_temp = slider1ManualModeSetpoint;

                    }
                }
                
                else if (mySlave.Locked == UNLOCKED)
                {
                    mySlave.CurrentMode = mySlave.Mode;
                    PrintStringToDiagnostics("Lamp is unlocked");
                    whichMode = "Unlocked";
                    mySlave.LampEnable = 1;

                        //Check SLEDs on
                    UInt16[] readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,16, 1);
                        if (readData2[0] == 1)
                        {
                            mySlave.SledsAreOn = 1;

                            sledsOnBut.Content = "         On ";
                            sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                            UInt16[] readData3 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 6);
                            //If SLEDs are on, enable track bars and update position
                            if (mySlave.Capabilities[0] != 0)
                            {
                                Slider1Changed = readData3[0] * 2.5 / 65535.0 * 1000;
                                Slider1_temp = readData3[0] * 2.5 / 65535.0 * 1000;
                                slider1TrackBar.IsEnabled = true;
                                setCurr1Edit.IsEnabled = true;
                            }

                            maxBut.IsEnabled = true;
                            minBut.IsEnabled = true;

                            sledsOnBut.IsEnabled = true;

                        }
                        else
                        {
                            mySlave.SledsAreOn = 0;

                            maxBut.IsEnabled = true;
                            minBut.IsEnabled = true;
                            sledsOnBut.IsEnabled = true;


                            //If SLED TEC is off, disable sliders
                            slider1TrackBar.IsEnabled = true;
                            setCurr1Edit.IsEnabled = true;

                            Slider1Changed = 0;
                            Slider1_temp = 0;
                        }
                    
                }
            });


            //defaults
            //SetCommunicationsDefaultValues
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 7, 1);
            mySlave.ExistingModbusID = readData[0];

          

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,124, 1);
            mySlave.ManufacturerDefModbusID = readData[0];

            
            //this is supert important set all the sliderchanged to -1 so that it will do a first time read from intput registers and set sliders
            Slider1Changed = -1;

        }

        
        
        public void SetStateSignal(int whichState, int slaveID)
        {
        }

        private void KickOffAllThreads()
        {
            foreach (ModBusSlave mySlave in modbusSlaveList)
            {
                if (mySlave != null)
                {
                    StartContinousRead(mySlave.SlaveID); //To start automatically
                }
            }

            
            looping = true;
            PrintStringToDiagnostics("Connection to Single-SLED established.");
            Thread t = new Thread(StartLoopCycle);
            t.Start();

            //StartLoopCycle();
        }

        public void StartContinousRead(int SlaveID)
        {
            modbusSlaveList[SlaveID].continuousSet = true;

            modbusSlaveList[SlaveID].ContinuousFirstRead = true; // For looping timer algorithm
            modbusSlaveList[SlaveID].WhichCycle = CONTINUOUS_READ;
        }

        private void ResetClassVariables(int slaveID)
        {
            loggingToFiles = false;
            showRawValues = false;
            modbusSlaveList[slaveID].ChartMode = NO_CHART_MODE;
            modbusSlaveList[slaveID].WhichCycle = IDLE_CYCLE;
            myThread.Suspend();
            selectedTabSlaveID.Remove(slaveID);
        }

        private void InitiateManualDisconnect(int slaveID)
        {
            Console.WriteLine("Inside manual disconnect");
            mbClient.Disconnect();
            //ToggleGUIElements(false, slaveID);
            ResetClassVariables(slaveID);
        }

        public void UpdateAndDisplayTxString(string data, string COMport)
        {
            if (commslogging)
            {
                string newString = (COMport + " | TX: " + data + '\n');

                PrintStringToComms(newString);
            }
        }
        public void UpdateAndDisplayRxString(string data, string COMport)
        {
            if (commslogging)
            {
                string newString = (COMport + " | RX: " + data + '\n');

                PrintStringToComms(newString);
            }

        }
        public void PrintStringToComms(string theString)
        {
            Thread thread = new Thread(() => PrintCommsThread(theString));
            thread.Start();

        }
        private void PrintCommsThread(string theString)
        {
            lock (commsWindow.LoggingTextEdit)
            {
                int number_of_lines = 150;
                string[] lines = new string[number_of_lines];
                this.Dispatcher.Invoke(() =>
                {
                    string[] textsplit = commsWindow.LoggingTextEdit.Text.Split('\n');
                    if (textsplit.Length > number_of_lines)
                    {
                        int i;
                        for (i = 0; i < textsplit.Length - 2; i++)
                        {
                            lines[i] = textsplit[i + 1];
                        }
                        lines[lines.Length - 1] = linenumber + " | " + DateTime.Now.ToString() + " | " + theString;
                        commsWindow.LoggingTextEdit.Text = string.Join("\n", lines);
                        commsWindow.LoggingTextEdit.ScrollToEnd();
                        commsWindow.LoggingTextEdit.Refresh();
                    }
                    else
                    {
                        commsWindow.LoggingTextEdit.Text += linenumber + " | " + DateTime.Now.ToString() + " | " + theString;
                        commsWindow.LoggingTextEdit.ScrollToEnd();
                        commsWindow.LoggingTextEdit.Refresh();
                    }
                    linenumber++;
                });
            }
            System.Threading.Thread.CurrentThread.Abort();
        }


       

        private void WriteLineToPublicLogFile(ModBusSlave mySlave)
        {
            string s = "";

            s += DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year; s += ",";
            s += DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second; s += ",";

            s += mySlave.SledsAreOn.ToString(); s += ",";

            s += mySlave.Sled1CurrentSetpoint.ToString("0.00"); s += ",";


            s += (mySlave.ActualCurr1ReadVal * 1000).ToString("0.00"); s += ",";

            s += (mySlave.MonDiode1ReadVal * 1000).ToString("0.00"); s += ",";

            s += mySlave.STECOn.ToString("0.00"); s += ",";
            s += sledTECTempGood.ToString("0.00"); s += ",";
            s += mySlave.SledTECTempSetpoint.ToString("0.00"); s += ",";
            s += mySlave.SledTECTemp.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyCurr.ToString("0.00"); s += ",";          //OSE Body Current. Version 5
            if (mySlave.OSEHeatOrCool == 0)
            {
                s += "Cooling"; s += ",";
            }
            else
            {
                s += "Heating"; s += ",";
            }
            s += mySlave.OSEBodyCapacity.ToString("0.00"); s += ",";   //OSE Body Capacity. Version 5


            s += mySlave.BoardTemperatureN.ToString("0.00"); s += ",";
            s += mySlave.ThermopileTemp.ToString("0.00"); s += ",";

            s += mySlave.PMWavelength.ToString("0.00"); s += ",";
            s += (mySlave.PMRead * 1000000).ToString("0.00"); s += ",";
            s += mySlave.PMPower.ToString("0.00"); s += ",";
            s += mySlave.PMPower.ToString("0.00"); s += ",";

            s += mySlave.PMTECTemp.ToString("0.00"); s += ",";            //PM Temperature. Version 7
            s += mySlave.PMTECTempSetpoint.ToString("0.00"); s += ",";            //PM Temperature Setpoint. Version 7
            s += mySlave.PMTECCurr.ToString("0.00"); s += ",";            //PM Current. Version 7
            s += mySlave.PMTECCapacity.ToString("0.00"); s += ",";     //PM Capacity. Version 7

            s += mySlave.FanSpeed.ToString("0.00"); s += ",";
            s += mySlave.FanSpeed.ToString("0.00"); s += ",";
            s += modeString; s += ",";


            //Raw Values
            s += mySlave.SledsAreOn.ToString("0.00"); s += ",";

            s += mySlave.Sled1CurrentSetpointRaw.ToString("0.00"); s += ",";

            s += mySlave.Sled1CurrSenseRaw.ToString("0.00"); s += ",";

            s += mySlave.MonDiode1RawLogString.ToString("0.00"); s += ",";

            s += mySlave.STECOn.ToString("0.00"); s += ",";
            s += sledTECTempGood.ToString("0.00"); s += ",";
            s += mySlave.SledTECTempSetpointRaw.ToString("0.00"); s += ",";
            s += mySlave.SledTECTempRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyCurrRaw.ToString("0.00"); s += ",";            //OSE Body Current. Version 5
            if (mySlave.OSEHeatOrCool == 0)
            {
                s += "Cooling"; s += ",";
            }
            else
            {
                s += "Heating"; s += ",";
            }
            s += mySlave.OSEBodyCapacityRaw.ToString("0.00"); s += ",";     //OSE Body Capacity. Version 5


            s += mySlave.BoardTemperatureRaw.ToString("0.00"); s += ",";
            s += mySlave.ThermopileTempRaw.ToString("0.00"); s += ",";        //We are putting board temperature raw into heat sink raw as well


            s += mySlave.PMWavelength.ToString("0.00"); s += ",";
            s += mySlave.PMReadRaw.ToString("0.00"); s += ",";
            s += mySlave.PMPowerRaw.ToString("0.00"); s += ",";
            s += mySlave.PMPowerRaw.ToString("0.00"); s += ",";

            s += mySlave.PMTECTempRaw.ToString("0.00"); s += ",";            //PM Temperature. Version 7
            s += mySlave.PMTECTempSetpointRaw.ToString("0.00"); s += ",";            //PM Temperature Setpoint. Version 7
            s += mySlave.PMTECCurrRaw.ToString("0.00"); s += ",";            //PM Current. Version 7
            s += mySlave.PMTECCapacityRaw.ToString("0.00"); s += ",";     //PM Capacity. Version 7


            s += mySlave.FanSpeedReadRaw.ToString("0.00"); s += ",";
            s += mySlave.FanSpeedReadRaw.ToString("0.00"); s += ",";
            s += modeString; s += ",";


            mySlave.slavePublicLogFileFsWriter.Write(s + "\n");
            mySlave.slavePublicLogFileFsWriter.Flush();

            //Add the string to the slave's known strings
            mySlave.CurrentPublicLogStrings.Add(s);

            //If we're overflowing the chart, remove the first value
            if (mySlave.CurrentPublicLogStrings.Count > MAX_CHART_VALUES)
            {
                mySlave.CurrentPublicLogStrings.RemoveAt(0);
                if (PublicCurrentValues_Labels.Count > MAX_CHART_VALUES)
                {
                    PublicCurrentValues_Labels.RemoveAt(0);
                }
            }

        }

      
        double CalculateBoardTempFromReg(double regValue, int firmwareVersion)
        {
            //Calculate Board temp based on formula from Modbus Map
            //Thermistor: ERTJ1VG103FA  Circuit: MAX6682-7680R

            UInt16 posOrNeg = (UInt16)(((UInt16)regValue) >> 15);
            regValue = (double)((UInt16)(regValue) & 0x7FFF);


            regValue = 5.04165492779607 * Math.Pow(10.0, -14) * Math.Pow(regValue, 6) +
            -8.11027056653307 * Math.Pow(10.0, -11) * Math.Pow(regValue, 5) +
            5.13071041412065 * Math.Pow(10.0, -8) * Math.Pow(regValue, 4) +
            -1.56435095292062 * Math.Pow(10.0, -5) * Math.Pow(regValue, 3) +
            2.32689870622949 * Math.Pow(10.0, -3) * Math.Pow(regValue, 2) +
            -1.10903725558273 * Math.Pow(10.0, -2) * regValue;

            if (posOrNeg == 1)
            {
                regValue *= -1.0;
            }

            return regValue;
        }
        double CalculateThermopileTempFromReg(double regValue, int firmwareVersion)
        {
        

            regValue = +4.847241855 * Math.Pow(10.0, -57) * Math.Pow(regValue, 13) - 2.015206679 * Math.Pow(10.0, -51) * Math.Pow(regValue, 12) + 3.768224333 * Math.Pow(10.0, -46) * Math.Pow(regValue, 11) +
                -4.18587553 * Math.Pow(10.0, -41) * Math.Pow(regValue, 10) + 3.074105132 * Math.Pow(10.0, -36) * Math.Pow(regValue, 9) - 1.572211288 * Math.Pow(10.0, -31) * Math.Pow(regValue, 8) +
                5.748088516 * Math.Pow(10.0, -27) * Math.Pow(regValue, 7) - 1.518847856 * Math.Pow(10.0, -22) * Math.Pow(regValue, 6) + 2.901872506 * Math.Pow(10.0, -18) * Math.Pow(regValue, 5) +
                -3.979030409 * Math.Pow(10.0, -14) * Math.Pow(regValue, 4) + 3.869497954 * Math.Pow(10.0, -10) * Math.Pow(regValue, 3) - 2.651418984 * Math.Pow(10.0, -06) * Math.Pow(regValue, 2) +
                0.0144139993 * Math.Pow(regValue, 1) - 64.41750664;

         

            return regValue;

        }

        private void StartLoopCycle()
        {
            UInt16[] readData;
            try
            {
                while (looping) //Always true unless disconnecting
                {

                    foreach (ModBusSlave mySlave in modbusSlaveList)
                    {

                        if (mySlave != null)
                        {
                            selectedSlaveID = mySlave.SlaveID;

                            //DISCONNECTING STATE
                            if (mySlave.WhichCycle == MANUAL_DISCONNECT)
                            {
                                InitiateManualDisconnect(mySlave.SlaveID);
                                looping = false;
                                foundbestsled = false;
                                break;

                            }

                            if (mySlave.WhichCycle != IDLE_CYCLE && mySlave.WhichCycle != MANUAL_DISCONNECT)
                            {
                                SetStateSignal(SIGNAL_ON, mySlave.SlaveID);
                            }

                            //CONTINUOUS READ ////////////////////////////////////////////////////////
                            if (mySlave.WhichCycle == CONTINUOUS_READ)
                            {

                                if (mySlave.ContinuousFirstRead)
                                {
                                    mySlave.ContinuousFirstRead = false;
                                }

                                CheckSLEDConnection(mySlave);
                                if (foundbestsled == false)
                                {
                                    ConnectToBestSLED();
                                    break;
                                }

                                // Find out if PC, Manual Mode or Modulation Mode
                                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 7, 1);
                                mySlave.Mode = readData[0];

                                ///////////////////////////////// MANUAL MODE SELECTED CONTINUOUS READ /////////////////////////////////

                                if (mySlave.Locked == LOCKED)
                                {
                                    // BeST SLED is in Manual Mode, log change if necessary
                                    // If we detect a change in "mode" (including no mode at all on bootup)

                                    if (mySlave.Mode != mySlave.CurrentMode)
                                    {
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            
                                            PrintStringToDiagnostics("Lamp is Locked.");
                                        });
                                    }
                                    mySlave.CurrentMode = mySlave.Mode;

                                    //For logfile
                                    modeString = "Locked";

                                    //Check if sled  Math.Power is on according to the unit
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 16, 1);
                                    mySlave.SledsAreOn = readData[0];

                                    //Read realtime currents. Sliders are set in the Synchronized UpdatePolledVariables function
                                    //Current in manual mode is dictated by hardware.
                                    ushort[] readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 6);
                                    if (mySlave.Capabilities[0] > 0)
                                    {
                                        //Each poll cycle read current from 20-25 and set slider accordingly

                                        slider1ManualModeSetpoint = readData2[0] * 2.5 / 65535.0 * 1000;
                                        if (slider1ManualModeSetpoint > 550) //maximum slider
                                        {
                                            slider1ManualModeSetpoint = 550;
                                        }
                                        Slider1Changed = slider1ManualModeSetpoint;
                                        Slider1_temp = slider1ManualModeSetpoint;
                                    }
                                }


                                ///////////////////////////////// PC MODE CONTINUOUS READ SELECTED /////////////////////////////////

                                else if (mySlave.Locked == UNLOCKED)
                                {


                                    //Display mode changed if needed
                                    if (mySlave.Mode != mySlave.CurrentMode)
                                    {
                                        //If the previous mode was manual, turn off sleds
                                        if (mySlave.Locked == LOCKED)
                                        {
                                            mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 0);
                                            mySlave.SledsAreOn = 0;
                                            this.Dispatcher.Invoke(() =>
                                            {
                                                sledsOnBut.IsEnabled = false;
                                            });

                                        }
                                        
                                        PrintStringToDiagnostics("Lamp is Unlocked");
                                    }
                                    mySlave.CurrentMode = mySlave.Mode;
                                    //modeString is used for logfile string
                                 
                                    //Check if sled  Power is on according to the unit
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 16, 1);
                                    mySlave.SledsAreOn = readData[0];

                                    //Assign currents based on slider position. We assign sled1Current etc. to the value of the slider and then use that value to calculate what to write to reg
                                    //GetSledCurrentsPCMode(mySlave);

                                    if (mySlave.SledsAreOn == 1)
                                    {
                                        UInt16 currentToSend;
                                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 6);
                                        if (mySlave.Capabilities[0] > 0)
                                        {

                                            if (Current1Old != (int)readData[0])
                                            {
                                                Slider1_temp = readData[0] * 2.5 / 65535.0 * 1000;
                                                Slider1Changed = -1;
                                                Current1Old = (int)readData[0];
                                            }
                                            else
                                            {
                                                if (Slider1Changed > -1)
                                                {
                                                    currentToSend = (UInt16)(0.001 * 65535 * Slider1_temp / 2.5);
                                                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 10, currentToSend);
                                                    if (Current1Old == currentToSend)
                                                    {
                                                        Slider1Changed = -1;
                                                    }
                                                    Current1Old = (int)currentToSend;
                                                }
                                            }
                                        }
                                    }
                                }

                                //////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///////////////////////////////// COMMON POLLING BETWEEN ALL MODES /////////////////////////////////

                                //Read connection type
                                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 17, 1);
                                connectionTypeInt = readData[0];
                                commsWindow.Update_ComSettings();


                                this.Dispatcher.Invoke(() =>
                                {


                                    //Output newly polled SLED variables to GUI objects
                                    string connectionTypeString = "";

                                    if (connectionTypeInt == 1)
                                    {
                                        connectionTypeString = " -- Connection Type: USB";
                                    }
                                    else if (connectionTypeInt == 2)
                                    {
                                        connectionTypeString = " -- Connection Type: RS-232";
                                    }
                                    else if (connectionTypeInt == 3)
                                    {
                                        connectionTypeString = " -- Connection Type: Ethernet";
                                    }


                                    datedisplay.Content = DateTime.Now;


                                    if (mySlave.SledsAreOn == 1)
                                    {
                                        sledsOnBut.Content = "         On ";
                                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                                    }
                                    else
                                    {
                                        sledsOnBut.Content = "         Off";
                                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                    }


                                    if (mySlave.Locked == LOCKED)
                                    {

                                        slider1TrackBar.IsEnabled = false;
                                        maxBut.IsEnabled = false;
                                        minBut.IsEnabled = false;
                                        sledsOnBut.IsEnabled = false;
                                        setCurr1Edit.IsEnabled = false;


                                    }
                                    else if (mySlave.Locked == UNLOCKED)
                                    {
                                        if (mySlave.SledsAreOn == 0)
                                        {
                                            //If SLEDs are off, disable track bar and send 0 current
                                            slider1TrackBar.IsEnabled = false;
                                            Slider1Changed = 0;
                                            Slider1_temp = 0;

                                        }
                                        else
                                        {
                                            //If SLEDs are on, enable track bars
                                            if (!slider1TrackBar.IsEnabled && mySlave.Capabilities[0] != 0)
                                            {
                                                slider1TrackBar.IsEnabled = true;
                                                setCurr1Edit.IsEnabled = true;
                                            }

                                            maxBut.IsEnabled = true;
                                            minBut.IsEnabled = true;
                                        }

                                        sledsOnBut.IsEnabled = true;


                                    }

                                    IInputElement focusedControl = FocusManager.GetFocusedElement(this);

                                    TextBox tBox = null;
                                    tBox = focusedControl as TextBox;
                                    //Update slider and current box 
                                    if (mySlave.Capabilities[0] != 0)
                                    {

                                        if (tBox != null)
                                        {
                                            if (tBox.Name != "setCurr1Edit")
                                            {
                                                mySlave.Sled1CurrentSetpointRaw = Math.Round((0.001 * 65535 * Slider1_temp / 2.5), 0);
                                                mySlave.Sled1CurrentSetpoint = Slider1_temp;
                                                if (showRawValues)
                                                {

                                                    setCurr1Edit.Text = mySlave.Sled1CurrentSetpointRaw.ToString();
                                                    slider1TrackBar.Value = mySlave.Sled1CurrentSetpoint;
                                                }
                                                else
                                                {
                                                    setCurr1Edit.Text = mySlave.Sled1CurrentSetpoint.ToString("0.0 mA");
                                                    slider1TrackBar.Value = mySlave.Sled1CurrentSetpoint;

                                                }
                                            }
                                        }
                                        else
                                        {
                                            mySlave.Sled1CurrentSetpointRaw = Math.Round((0.001 * 65535 * Slider1_temp / 2.5), 0);
                                            mySlave.Sled1CurrentSetpoint = Slider1_temp;
                                            if (showRawValues)
                                            {

                                                setCurr1Edit.Text = mySlave.Sled1CurrentSetpointRaw.ToString();
                                                slider1TrackBar.Value = mySlave.Sled1CurrentSetpoint;
                                            }
                                            else
                                            {
                                                setCurr1Edit.Text = mySlave.Sled1CurrentSetpoint.ToString("0.0 mA");
                                                slider1TrackBar.Value = mySlave.Sled1CurrentSetpoint;

                                            }
                                        }
                                    }


                                    double minimumCurrSenseRegValue = 131.0; //From formula 0.001*65535/5 = 13.107 will show no values less than 10 mA

                                    //SLED 1-6 Current Sense Reading: Input Register 20-25 --> text in GUI: "Actual Current"
                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 20, 35);

                                    mySlave.Sled1CurrSenseRaw = readData[0];
                                    if (mySlave.Sled1CurrSenseRaw < minimumCurrSenseRegValue || slider1TrackBar.Value == 0)
                                    {
                                        mySlave.ActualCurr1ReadVal = 0;
                                    }
                                    else
                                    {
                                        mySlave.ActualCurr1ReadVal = 5.0 * (mySlave.Sled1CurrSenseRaw / (65535.0));
                                    }
                                    //Current Sense Values 
                                    if ((showRawValues))
                                    {

                                        if (mySlave.Capabilities[0] != 0)
                                        {
                                            actualCurr1Edit.Text = mySlave.Sled1CurrSenseRaw.ToString("0");
                                        }
                                        else
                                        {
                                            actualCurr1Edit.Text = "";
                                        }
                                    }
                                    else
                                    {

                                        if (mySlave.Capabilities[0] != 0)
                                        {
                                            actualCurr1Edit.Text = (2.5 * (mySlave.Sled1CurrSenseRaw * 1000 / (65535.0))).ToString("0") + " mA"; 
                                        }
                                        else
                                        {
                                            actualCurr1Edit.Text = "";
                                        }
                                    }

                                    //For all other versions show real Heat Sink Temperature

                                    if (showRawValues)
                                    {
                                        thermopileTempEdit.Text = mySlave.ThermopileTempRaw.ToString("");

                                    }
                                    else
                                    {
                                        thermopileTempEdit.Text = mySlave.ThermopileTemp.ToString("0.0 ⁰C");
                                    }


                                    focusedControl = FocusManager.GetFocusedElement(this);
                                    tBox = focusedControl as TextBox;
                                    if (mySlave.SledsAreOn == 1)
                                    {
                                        sledsOnBut.Content = "         On ";
                                    }
                                    else
                                    {
                                        sledsOnBut.Content = "         Off";
                                    }


                                    tBox = focusedControl as TextBox;

                                    if (tBox != null)
                                    {
                                        if (tBox.Name != "fanSpeedSetPointEdit")
                                        {
                                            if (showRawValues)
                                            {
                                                fanSpeedReadEdit.Text = mySlave.FanSpeedReadRaw.ToString("");
                                                fanSpeedSetPointEdit.Text = mySlave.FanSpeedReadRaw.ToString("");
                                            }
                                            else
                                            {
                                                fanSpeedReadEdit.Text = mySlave.FanSpeed.ToString("0.0 CFM");
                                                fanSpeedSetPointEdit.Text = mySlave.FanSpeed.ToString("0.0 CFM");
                                            }
                                            FanSpeedSetBut.IsDefault = false;
                                        }
                                        else
                                        {
                                            FanSpeedSetBut.IsDefault = true;
                                            fanSpeedSetPointEdit_temp = fanSpeedSetPointEdit.Text;
                                        }


                                    }
                                    else
                                    {
                                        if (showRawValues)
                                        {
                                            fanSpeedReadEdit.Text = mySlave.FanSpeedReadRaw.ToString("");
                                            fanSpeedSetPointEdit.Text = mySlave.FanSpeedReadRaw.ToString("");
                                        }
                                        else
                                        {
                                            fanSpeedReadEdit.Text = mySlave.FanSpeed.ToString("0.0 CFM");
                                            fanSpeedSetPointEdit.Text = mySlave.FanSpeed.ToString("0.0 CFM");
                                        }
                                        FanSpeedSetBut.IsDefault = false;
                                    }

                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 20, 35);
                                    //Board Temperature

                                    //Board Temperature reg 40
                                    mySlave.BoardTemperatureRaw = readData[20];

                                    boardTemperature = CalculateBoardTempFromReg(mySlave.BoardTemperatureRaw, (int)mySlave.FirmwareVersion);
                                    mySlave.BoardTemperatureN = boardTemperature;

                                    //Thermopile Temp
                                    if (mySlave.Capabilities[14] == 1)
                                    {
                                        mySlave.ThermopileTemp = CalculateThermopileTempFromReg(mySlave.ThermopileTempRaw, (int)mySlave.FirmwareVersion);
                                    }

                                    //SLED TEC Temperature Reading: Input Register 32
                                    mySlave.ThermopileTempRaw = readData[12];
                                    mySlave.ThermopileTemp = CalculateThermopileTempFromReg(mySlave.ThermopileTempRaw, (int)mySlave.FirmwareVersion);


                                    // Get fan speed
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 67, 1);
                                    mySlave.FanSpeedReadRaw = readData[0];
                                    mySlave.FanSpeed = Math.Floor((7.0 * (mySlave.FanSpeedReadRaw / 65535.0) * 10.0) + 0.5) / 10.0;


                                    //Gas Reading
                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 35, 4);
                                    mySlave.Gas1Raw = readData[0];
                                    mySlave.Gas1 = Math.Round((2.5 * (double) mySlave.Gas1Raw / 65535.0), 1);
                                    mySlave.Gas2Raw = readData[1];
                                    mySlave.Gas2 = Math.Round((2.5 * (double)mySlave.Gas2Raw / 65535.0), 1);
                                    mySlave.Gas3Raw = readData[2];
                                    mySlave.Gas3 = Math.Round((2.5 * (double)mySlave.Gas3Raw / 65535.0), 1);
                                    mySlave.Gas4Raw = readData[3];
                                    mySlave.Gas4 = Math.Round((2.5 * (double)mySlave.Gas4Raw / 65535.0), 1);

                                    Gas1Edit.Text = mySlave.Gas1 + " V";
                                    Gas2Edit.Text = mySlave.Gas2 + " V";
                                    Gas3Edit.Text = mySlave.Gas3 + " V";
                                    Gas4Edit.Text = mySlave.Gas4 + " V";

                                    // if locked set slider to setcurrent

                                    if (mySlave.Locked == LOCKED)
                                    {
                                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 1);
                                        
                                        slider1TrackBar.Value = readData[0] * 2.5 / 65535.0 * 1000;

                                    }

                                    UpdateAdminVariables(mySlave);


                                }); //invoke thread ending
                                CheckSLEDConnection(mySlave);
                                if (foundbestsled == false)
                                {
                                    ConnectToBestSLED();
                                    break;
                                }
                            }


                            if (loggingToFiles)
                            {
                                WriteLineToPublicLogFile(mySlave);
                                this.Dispatcher.Invoke(() =>
                                {
                                    LoggingIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                                });
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    LoggingIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                });
                            }



                            //If we have a disconnect
                            if (mySlave.WhichCycle != CONTINUOUS_READ)
                                break;




                            //Charting 
                            if (graphing == true)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    if (List_PM.Text == Gas1_Raw.Content.ToString())
                                    {

                                        ValuesChart_YAxis.MinValue = Math.Round(Math.Min(mySlave.SledTECCapacity, ValuesChart_YAxis.MinValue + 0.1) - 0.1, 2);
                                        ValuesChart_YAxis.MaxValue = Math.Round(Math.Max(mySlave.SledTECCapacity, ValuesChart_YAxis.MaxValue - 0.1) + 0.1, 2);
                                        ValuesChart_YAxis_Ticks.Step = Math.Round((ValuesChart_YAxis.MaxValue - ValuesChart_YAxis.MinValue) / 21, 3);
                                        ValuesChart.Series[0].Values.Add(mySlave.SledTECCapacity);
                                        Series0_axisX_label.Add(DateTime.Now.ToString());
                                        if ((int)Series0_axisX_label.Count > graphing_limit)
                                        {
                                            // remove that number of items from the start of the list
                                            Series0_axisX_label.RemoveRange(0, 1);
                                            ValuesChart.Series[0].Values.RemoveAt(0);
                                        }


                                    }

                                });

                                //Thread.Sleep(100);
                            }

                        }


                    }
                }

            }
            catch (Exception e)
            {
                looping = false;
                temp_too_hot = false;
                ClearMainWindow();
                PrintStringToDiagnostics("Connection to SLED lost. Returning to Initialization..." + e);
                selectedSlaveID = 0;
                foundbestsled = false;
                foreach (ModBusSlave mySlave in modbusSlaveList)
                {
                    if (mySlave != null)
                    {
                        if (mySlave.slaveAdminLogFileFsWriter != null)
                            mySlave.slaveAdminLogFileFsWriter.Close();

                        if (mySlave.slavePublicLogFileFsWriter != null)
                            mySlave.slavePublicLogFileFsWriter.Close();

                        aboutWindow.UpdateAboutGUI(mySlave);
                    }

                }
                modbusSlaveList = new ModBusSlave[MAX_NUM_SLAVES + 1];

                ConnectToBestSLED();

            }


        }

        private void CheckSLEDConnection(ModBusSlave mySlave)
        {
            //Make sure Single-SLED is still connected. 
            UInt16[] bytesRead = this.mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 0, 5);
            if (bytesRead[0] == 0 && bytesRead[1] == 0 && bytesRead[2] == 0 && bytesRead[3] == 0 && bytesRead[4] == 0)
            {

                bytesRead = this.mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 0, 5);
                if (bytesRead[0] == 0 && bytesRead[1] == 0 && bytesRead[2] == 0 && bytesRead[3] == 0 && bytesRead[4] == 0)
                {
                    looping = false;
                    temp_too_hot = false;
                    ClearMainWindow();
                    PrintStringToDiagnostics("Connection to SLED lost. Returning to Initialization...");
                    selectedSlaveID = 0;
                    this.mbClient.Disconnect();
                    foundbestsled = false;

                    if (mySlave != null)
                    {
                        if (mySlave.slaveAdminLogFileFsWriter != null)
                            mySlave.slaveAdminLogFileFsWriter.Close();

                        if (mySlave.slavePublicLogFileFsWriter != null)
                            mySlave.slavePublicLogFileFsWriter.Close();
                    }


                    modbusSlaveList = new ModBusSlave[MAX_NUM_SLAVES + 1];

                }
            }
            
        }
        private void ClearMainWindow()
        {
            this.Dispatcher.Invoke(() =>
            {
                AboutBut.IsEnabled = true;
                CommunicationBut.IsEnabled = true;
                AdminBut.IsEnabled = false;

                maxBut.IsEnabled = false;
                minBut.IsEnabled = false;
                FanSpeedSetBut.IsEnabled = false;
                Start_PM.IsEnabled = false;
                Stop_PM.IsEnabled = false;
                Clear_PM.IsEnabled = false;
                sledsOnBut.IsEnabled = false;
                
                LoggingBut.IsEnabled = false;
                DefaultsBut.IsEnabled = false;
                
                ExportBut_PM.IsEnabled = false;

                fanSpeedSetPointEdit.IsEnabled = false;
                List_PM.IsEnabled = false;

                slider1TrackBar.IsEnabled = false;
                slider1TrackBar.Value = 0;

                setCurr1Edit.IsEnabled = false;
                sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));

                var alltextboxes = this.main_window_grid.Children.OfType<TextBox>();
                foreach (var textbox in alltextboxes)
                {
                    if (textbox.Name != "Diagnostics")
                    {
                        textbox.Clear();
                    }
                }

                ValuesChart.Series[0].Values.Clear();
                ValuesChart.Series[1].Values.Clear();
                ValuesChart.Series[2].Values.Clear();
                ValuesChart_YAxis.MinValue = Math.Round((double)1000000000, 2);
                ValuesChart_YAxis.MaxValue = Math.Round((double)-1000000000, 2);
                Series0_axisX_label.Clear();

                displaybottom.Content = "";
            });
        }

      
        void UpdateAdminVariables(ModBusSlave mySlave)
        {

            ///////Itziar need to finish this part
            this.Dispatcher.Invoke(() =>
            {
                //Set the values on About Window

                adminWindow.CurrSenseRaw1Edit.Text = mySlave.Sled1CurrSenseRaw.ToString("0");
                adminWindow.CurrSenseCalc1Edit.Text = Math.Round((mySlave.ActualCurr1ReadVal * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[0]))),1).ToString("0.0") + CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[1];
                      
                adminWindow.BoardTempBox.Text = mySlave.BoardTemperatureN.ToString("0.0") + (" ⁰C");
                adminWindow.BoardTempRaw.Content = mySlave.BoardTemperatureRaw;
            });
        }

        string CheckMagnitude(double number)
        {
            string units = "";
            int magnitude = 0;

            while (number * Math.Pow(10.0, magnitude) < 1)
            {
                if (magnitude == 15)
                {
                    magnitude = 0;
                    break;
                }
                magnitude += 3;
            }
            if (magnitude == 0)
            {
                units = " A";
            }
            else if (magnitude == 3)
            {
                units = " mA";
            }
            else if (magnitude == 6)
            {
                units = " uA";
            }
            else if (magnitude == 9)
            {
                units = " nA";
            }
            else if (magnitude == 12)
            {
                units = " pA";
            }
            return magnitude.ToString() + "," + units;
        }

       
       
        private void sledsOnBut_Click(object sender, RoutedEventArgs e)
        {
            
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.SledsAreOn == 0)
                {
                    mySlave.SledsAreOn = 1;
                    sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                    sledsOnBut.Content = "         On ";
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 1);
                    // Send the change
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 50, 1);

                    PrintStringToDiagnostics("SLED Power On successful.");

                }
                else
                {
                    mySlave.SledsAreOn = 0;
                    sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 0);
                    sledsOnBut.Content = "         Off";
                    //Set all SLED currents to 0
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 10, 0);
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 11, 0);
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 12, 0);
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 13, 0);
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 14, 0);
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 15, 0);
                    PrintStringToDiagnostics("SLED Power Off successful.");

                }
            });

        }

        private void lampEnableBut_Click(object sender, RoutedEventArgs e)
        {

            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.LampEnable == 0)
                {
                    mySlave.Locked = UNLOCKED;

                    mySlave.LampEnable = 1;
                    lampEnableIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                    lampEnableBut.Content = "         UnLocked";
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 8, 1);

                    PrintStringToDiagnostics("Lamp Unlocked successful.");
                   ushort[] readData=mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 8, 1);

                    //enable slider
                    whichMode = "1";
                    slider1TrackBar.IsEnabled =true;
                }
                else
                {
                    mySlave.Locked = LOCKED;

                    mySlave.LampEnable = 0;
                    lampEnableIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 8, 0);
                    lampEnableBut.Content = "         Locked";

                    //disable slider
                    whichMode = "0";
                    slider1TrackBar.IsEnabled = false;
                    mySlave.Locked = LOCKED;
                    PrintStringToDiagnostics("Lamp Locked successful.");

                }
            });

        }
        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider1_temp = e.NewValue;
            Slider1Changed = e.NewValue;
        }

        private void Slider1_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Slider1_temp = ((Slider)sender).Value;
                Slider1Changed = ((Slider)sender).Value;
            });
        }

        private void Sled1_TextChanged(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (setCurr1Edit.Text.Length > 0 && IsTextNumeric(setCurr1Edit.Text.Replace("mA", "").Replace(" ", "")) == true)
                {
                    double newValue = double.Parse(setCurr1Edit.Text.Replace("mA", "").Replace(" ", ""));


                    if (newValue < 0 || newValue > modbusSlaveList[selectedSlaveID].Capabilities[6])
                    {
                        MessageBox.Show("Invalid Set Current");
                    }
                    else
                    {
                        Slider1Changed = newValue;
                        Slider1_temp = newValue;
                    }
                }

            });
        }
        private void maxBut_Click(object sender, RoutedEventArgs e) 
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            this.Dispatcher.Invoke(() =>
            {
                    Slider1Changed = slider1TrackBar.Maximum;
                    Slider1_temp = slider1TrackBar.Maximum;
                    PrintStringToDiagnostics("Operating Current set to maximum.");
            });
        }
       
        private void minBut_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Slider1Changed = slider1TrackBar.Minimum;
                Slider1_temp = slider1TrackBar.Minimum;
                PrintStringToDiagnostics("Operating Current set to minimmum.");
            });
        }

     
        private void FanSpeedSetBut_Click(object sender, RoutedEventArgs e)
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            double fanSpeedMax = 7.0;   //Maximum CFM value from Modbus Register Map equation
            double fanSpeedMin = 0.0;

            int fanSpeedMaxRaw = 65535;   //Maximum Raw value from Modbus Register Map equation
            int fanSpeedMinRaw = 0;

            if (fanSpeedSetPointEdit_temp != null)
            {
                if (fanSpeedSetPointEdit_temp.Length > 0 && IsTextNumeric(fanSpeedSetPointEdit_temp.Replace(" CFM", "")) == true)
                {
                    double givenSpeed = double.Parse(fanSpeedSetPointEdit_temp.ToString().Replace(" CFM", ""));

                    if (showRawValues)
                    {
                        if (givenSpeed > fanSpeedMaxRaw)
                        {
                            givenSpeed = fanSpeedMaxRaw;
                        }
                        else if (givenSpeed < fanSpeedMinRaw)
                        {
                            givenSpeed = fanSpeedMinRaw;
                        }

                        // Setting Fan Speed. Calculate Fan Speed and send the change
                        double SpeedValue = Math.Round(givenSpeed * 65535.0 / 7.0, 0);
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 67, (UInt16)givenSpeed);
                        Keyboard.Focus(FanSpeedSetBut);
                        Keyboard.ClearFocus();
                        PrintStringToDiagnostics("Fan Speed set successful. Fan Speed=" + SpeedValue.ToString("0.0") + " CFM. Raw=" + givenSpeed.ToString(""));
                    }
                    else
                    {
                        //Check bounds
                        if (givenSpeed > fanSpeedMax)
                        {
                            givenSpeed = fanSpeedMax;
                        }
                        else if (givenSpeed < fanSpeedMin)
                        {
                            givenSpeed = fanSpeedMin;
                        }

                        // Setting Fan Speed. Calculate Fan Speed and send the change
                        double regValue = Math.Round(givenSpeed * 65535.0 / 7.0, 0);
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 67, (UInt16)regValue);
                        Keyboard.Focus(FanSpeedSetBut);
                        Keyboard.ClearFocus();
                        PrintStringToDiagnostics("Fan Speed set successful. Fan Speed=" + givenSpeed.ToString("0.0") + " CFM. Raw=" + regValue.ToString(""));
                    }
                }
            }

        }
        
       
        private static bool IsTextNumeric(string str)
        {
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            //  return reg.IsMatch(str);

            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[0-9]([.][0-9]{1,3})?$");
            //return reg.IsMatch(str);
            Decimal dummy;
            return Decimal.TryParse(str, out dummy);

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Keyboard.Focus(sledsOnBut);
                Keyboard.ClearFocus();
            }
        }

        private void AdminBut_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            if (selectedSlaveID != 0)
            {
                if (isadmin == 0)
                {
                    passWindow.Show();
                    passWindow.Visibility = Visibility.Visible;
                    passWindow.Activate();
                    passWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    adminWindow.Show();
                    adminWindow.Visibility = Visibility.Visible;
                    adminWindow.Activate();
                    adminWindow.WindowState = WindowState.Normal;
                }

            }
            else
                MessageBox.Show("Not connected device detected.");
        }

        public Func<double, string> Formatter { get; set; }

        private void Start_PM_Click(object sender, RoutedEventArgs e)
        {
           
            ValuesChart.Series[0].Values.Clear();
            ValuesChart.Series[1].Values.Clear();
            ValuesChart.Series[2].Values.Clear();
            ValuesChart.Series[3].Values.Clear();
            ValuesChart.Series[4].Values.Clear();
            ValuesChart.Series[5].Values.Clear();
            ValuesChart.Series[6].Values.Clear();
            ValuesChart.Series[7].Values.Clear();
 
            Series0_axisX_label.Clear();

            this.Dispatcher.Invoke(() => {
                ValuesChart_YAxis.MinValue = Math.Round((double)1000000000, 2);
                ValuesChart_YAxis.MaxValue = Math.Round((double)-1000000000, 2);
            });
            ValuesChart_YAxis.Title = List_PM.Text;

            graphing = true;
            Start_PM.IsEnabled = false;
            Stop_PM.IsEnabled = true;
            Clear_PM.IsEnabled = true;
            List_PM.IsEnabled = false;

            
        }

        private void Stop_PM_Click(object sender, RoutedEventArgs e)
        {
            graphing = false;
            Start_PM.IsEnabled = true;
            Stop_PM.IsEnabled = false;
            Clear_PM.IsEnabled = true;
            List_PM.IsEnabled = true;
        }

        private void Clear_PM_Click(object sender, RoutedEventArgs e)
        {
            ValuesChart.Series[0].Values.Clear();
            ValuesChart.Series[1].Values.Clear();
            ValuesChart.Series[2].Values.Clear();
            ValuesChart.Series[3].Values.Clear();
            ValuesChart.Series[4].Values.Clear();
            ValuesChart.Series[5].Values.Clear();
            ValuesChart.Series[6].Values.Clear();
            ValuesChart.Series[7].Values.Clear();
            Series0_axisX_label.Clear();

            this.Dispatcher.Invoke(() => {
                 ValuesChart_YAxis.MinValue = Math.Round((double)1000000000, 2);
                 ValuesChart_YAxis.MaxValue = Math.Round((double)-1000000000, 2);
            });
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}

public static class ExtensionMethods
{
    private static Action EmptyDelegate = delegate () { };

    public static void Refresh(this UIElement uiElement)
    {
        uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
    }
}
    