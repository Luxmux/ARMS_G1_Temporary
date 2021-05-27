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
        const int MODULATION_MODE = 2;
        const int PC_MODE = 1;
        const int MANUAL_MODE = 0;
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
        private Modulation modulationWindow;
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


        private string sledTECSetPointEdit_temp;
        private string PMTECSetPointEdit_temp;
        private string fanSpeedSetPointEdit_temp;
        private string pmWaveLengthEdit_temp;
        public string DutyCycleEdit_temp;
        public string ModFreqEdit_temp;
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
                Title = "Current [uA]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Power [uW]",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 0.1,
            });

            ValuesChart.Series.Add(new LineSeries
            {
                Title = "Power [dBm]",
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
                sledTECSaveBut.IsEnabled = false;
                FanSpeedSetBut.IsEnabled = false;
                Start_PM.IsEnabled = false;
                Stop_PM.IsEnabled = false;
                Clear_PM.IsEnabled = false;
                waveChangeBut_PM.IsEnabled = false;
                PMTECSaveBut.IsEnabled = false;
                sledsOnBut.IsEnabled = false;
                sledTECOnOffBut.IsEnabled = false;
                modulationBut.IsEnabled = false;
                LoggingBut.IsEnabled = false;
                DefaultsBut.IsEnabled = false;
                CurveBut_PM.IsEnabled = false;
                ExportBut_PM.IsEnabled = false;
                pmWaveLengthEdit.IsEnabled = false;
                PMTECSetPointEdit.IsEnabled = false;

                sledTECSetPointEdit.IsEnabled = false;
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
                        UInt16[] bytesRead = this.mbClient.ReadInputRegisters(255, 0, 6);
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
                    comtotry = Convert.ToInt32(port.Replace("COM", ""));

                }
            }

                if (foundbestsled && newconnection)
                {
                ModBusSlave newSlave = new ModBusSlave();
                newSlave.SlaveID = comtotry;
                modbusSlaveList[newSlave.SlaveID] = newSlave;
                selectedSlaveID = comtotry; // set the active selected slave for all functions going forward
                PrintStringToDiagnostics("Single-SLED ID verified successfully");
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
                        "SLED ON/OFF," +

                        "SLED Drive [mA]," +
                        "SLED Current Sense [mA]," +
                        "SLED  Mon Diode Current [μA],"+

                        "SLED TEC ON/OFF," +
                        "SLED TEC Stable," +
                        "SLED TEC Temp Setpoint [C]," +
                        "SLED TEC Temp [C]," +
                        "SLED TEC Current [A],"+
                        "SLED TEC Status,";

                s = s + "SLED TEC Capacity [%],";



                s = s + "Board Temp [C],";


                s = s + "Heat Sink Temp [C],";


                s = s +
                    "PM Wavelength [nm],"+
                "PM Current [uA]," +
                "PM Power [mW]," +
                "PM Power [dBm]," ;

                s = s +
                "PM Temp Read [C]," +
                "PM Temp Setpoint [C]," +
                "PM TEC Current," +
                "PM TEC Capacity [%],";


            s = s +
                "Fan Speed Set [CFM]," +
                "Fan Speed Read [CFM]," +
                "Operating Mode,";

            s = s +
                "SLED ON/OFF Raw," +
                "SLED Drive Raw," +
                "SLED Current Sense Raw," +
                "SLED Mon Diode Current Raw," +

                "SLED TEC ON/OFF Raw," +
                "SLED TEC Stable Raw," +
                "SLED TEC Temp Setpoint Raw," +
                "SLED TEC Temp Raw," +
                "SLED TEC Current Raw,"+
                "SLED TEC Status Raw,";


            s = s +
                "SLED TEC Capacity Raw,";


                s = s + "Board Temp Raw,";


            s = s + "Heat Sink Temp Raw,";


                s = s +
                "PM Wavelength Raw," +
                "PM Current Raw," +
               "PM Power - mW - Raw," +
               "PM Power - dBm - Raw,";


                s = s +
                "PM Temp Read Raw," +
                "PM Temp Setpoint Raw," +
                "PM TEC Current Raw," +
                "PM TEC Capacity Raw,";

            s = s +
                "Fan Speed Set Raw," +
                "Fan Speed Read Raw," +
                "Operating Mode\n";    //important that last header field end with '\n' not a ','

            mySlave.slavePublicLogFileFsWriter.Write(s);

            mySlave.slavePublicLogFileFsWriter.Flush();
        }

        public void InitializeAdminCSVFile(ModBusSlave mySlave)
        {

            string s = "Date," +
                        "Time," +
                        "SLED ON/OFF," +

                        "SLED Drive [mA]," +

                        "SLED Current Sense [mA]," +
                        "SLED Mon Diode Current [μA]," +

                        "SLED TEC ON/OFF," +
                        "SLED TEC Stable," +
                        "SLED TEC Temp Setpoint [C]," +
                        "SLED TEC Temp [C],";

                s = s +
                "SLED TEC Current [A]," +
                "SLED TEC Capacity [%],";


                s = s + "Board Temp [C],";


            s = s + "Heat Sink Temp [C],";

                s = s +
                "OSE Body Temp Read [C]," +
                        "OSE Body TEC Current [A]," +
                        "OSE Body TEC Status,"+
                        "OSE Body TEC Capacity [%]," +
                        "OSE TEC Time Constant [s]," +
                        "OSE Body TEC Kp Realtime," +
                        "OSE Body TEC Kd Realtime," +
                        "OSE Body TEC Ki Realtime," +
                        "OSE Body TEC Cooling PID [%]," +
                        "OSE Body TEC Heating PID [%]," +
                        "OSE Body TEC Error Size Box [C]," +
                        "OSE Body Speed Change Value [C]," +
                        "OSE Direction Correct,";

                s = s +
                        "PM Wavelength [nm]," + 
                        "PM Current [uA]," +
                        "PM Power [mW]," +
                        "PM Power [dBm],";

                s = s +
                "PM Temp Read [C]," +
                "PM Temp Setpoint [C]," +
                        "PM TEC Current [A]," +
                        "PM TEC Capacity [%]," +
                        "PM TEC Time Constant [s]," +
                        "PM TEC Kp Realtime," +
                        "PM TEC Kd Realtime," +
                        "PM TEC Ki Realtime," +
                        "PM TEC Cooling PID [%]," +
                        "PM TEC Heating PID [%]," +
                        "PM TEC Error Size Box [C]," +
                        "PM Speed Change Value [C]," +
                        "PM Direction Correct,";


            s = s +
                "Fan Speed Set [CFM]," +
                "Fan Speed Read [CFM]," +
                "Operating Mode,";

            s = s +
                "SLED ON/OFF Raw," +
                "SLED Drive Raw," +
                "SLED Current Sense Raw," +
                "SLED Mon Diode Current Raw," +

                "SLED TEC ON/OFF Raw," +
                "SLED TEC Stable Raw," +
                "SLED TEC Temp Setpoint Raw," +
                "SLED TEC Temp Raw,";

                s = s + "SLED TEC Current Raw,";
                s = s + "SLED TEC Capacity Raw,";



                s = s + "Board Temp Raw,";


            s = s + "Heat Sink Temp Raw,";

                s = s +
                "OSE Body Temp Read Raw," +
                "OSE Body TEC Current Raw," +
                "OSE Body TEC Status Raw," +
                "OSE Body TEC Capacity Raw," +
                "OSE TEC Time Constant Raw," +
                "OSE Body TEC Kp Realtime Raw," +
                "OSE Body TEC Kd Realtime Raw," +
                "OSE Body TEC Ki Realtime Raw," +
                "OSE Body TEC Cooling PID Raw," +
                "OSE Body TEC Heating PID Raw," +
                "OSE Body TEC Error Size Raw," +
                "OSE Body Speed Change Value Raw," +
                "OSE Direction Correct Raw,";


                s = s +
               "PM Wavelength Raw," +
               "PM Current [uA] Raw," +
                "PM Power [mW] Raw," +
               "PM Power [dBm] Raw,";


                s = s +
                "PM Temp Read Raw," +
                "PM Temp Setpoint Raw," +
                "PM TEC Current Raw," +
                "PM TEC Capacity Raw," +
                "PM TEC Time Constant Raw," +
                "PM TEC Kp Realtime Raw," +
                "PM TEC Kd Realtime Raw," +
                "PM TEC Ki Realtime Raw," +
                "PM TEC Cooling PID Raw," +
                "PM TEC Heating PID Raw," +
                "PM TEC Error Size Raw," +
                "PM Speed Change Value Raw," +
                "PM Direction Correct Raw,";



            s = s +
                "Fan Speed Set Raw," +
                "Fan Speed Read Raw," +
                "Operating Mode\n";

            mySlave.slaveAdminLogFileFsWriter.Write(s);

            mySlave.slaveAdminLogFileFsWriter.Flush();
        }

        public void CurveBut_PM_Click(object sender, EventArgs e)
        {
            photosensitivityWindow.Visibility = Visibility.Visible;
            photosensitivityWindow.Activate();
            photosensitivityWindow.WindowState = WindowState.Normal;

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


                    PrintStringToDiagnostics("Single-SLED found on COM Port " + mySlave.SlaveID.ToString());

                    PrintStringToDiagnostics("Loading Configuration...");

                    //Wait for boot done bit
                    while (mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 5, 1)[0] == 0)
                    {
                        //NOP
                        //Should put a timeout here
                    }
                    PrintStringToDiagnostics("Boot done.");
                    SetStateSignal(IDLE_CYCLE, mySlave.SlaveID);

                    UInt16[] readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 0, 1);
                    mySlave.ModbusID = readData[0];

                    ReadAndSetDefaultsAndCapabilities(mySlave); // do first check stuff for each slave id
                    aboutWindow.UpdateAboutGUI(mySlave);
                    PrintStringToDiagnostics("Model number found.");

                    this.Dispatcher.Invoke(() =>
                    {
                        //start admin with selected slave
                        passWindow = new Password(this); //set up pre admin password
                        passWindow.Visibility = Visibility.Hidden;
                        adminWindow = new Admin(this);   //set up an admin window for polling
                        mySlave.AdminLogginBut = false;

                        //start modulation window
                        modulationWindow = new Modulation(this);
                        modulationWindow.Visibility = Visibility.Hidden;

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
            //read firmware 
            UInt16[] readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,0, 7);
            mySlave.FirmwareVersion = readData[6];

            this.Dispatcher.Invoke(() =>
            {
                AdminBut.IsEnabled = true;
                LoggingBut.IsEnabled = true;
                DefaultsBut.IsEnabled = true;
                //read model
                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,999, 1);
                mySlave.Model = readData[0];
                if ((mySlave.Model - 120) == 20001)
                {
                    Current_A.Content = "Voltage [uA]";
                }
                else
                {
                    Current_A.Content = "Current [uA]";
                }
            });

            //read mode and capabilities
            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,7, 1);
            mySlave.Mode = readData[0];


            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,100, 34);
            mySlave.Capabilities[0] = readData[0]; //SLED 1: 0 if DNE otherwise wavelength
            mySlave.Capabilities[1] = readData[1]; //SLED 2: 0 if DNE otherwise wavelength
            mySlave.Capabilities[2] = readData[2]; //SLED 3: 0 if DNE otherwise wavelength
            mySlave.Capabilities[3] = readData[3]; //SLED 4: 0 if DNE otherwise wavelength
            mySlave.Capabilities[4] = readData[4]; //SLED 5: 0 if DNE otherwise wavelength
            mySlave.Capabilities[5] = readData[5]; //SLED 6: 0 if DNE otherwise wavelength



            mySlave.Capabilities[6] = (int)Math.Round(((readData[6]) * 2.5 / (65535.0) * 1000), 1); //SLED 1: Max current limit & max manufacture default
            mySlave.Capabilities[7] = (int)Math.Round(((readData[7]) * 2.5 / (65535.0) * 1000), 1); //SLED 2: Max current limit & max manufacture default
            mySlave.Capabilities[8] = (int)Math.Round(((readData[8]) * 2.5 / (65535.0) * 1000), 1); //SLED 3: Max current limit & max manufacture default
            mySlave.Capabilities[9] = (int)Math.Round(((readData[9]) * 2.5 / (65535.0) * 1000), 1); //SLED 4: Max current limit & max manufacture default
            mySlave.Capabilities[10] = (int)Math.Round(((readData[10]) * 2.5 / (65535.0) * 1000), 1); //SLED 5: Max current limit & max manufacture default
            mySlave.Capabilities[11] = (int)Math.Round(((readData[11]) * 2.5 / (65535.0) * 1000), 1); //SLED 6: Max current limit & max manufacture default

            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.Capabilities[0] > 0)
                {

                    Wavelength1.Content = (mySlave.Capabilities[0]).ToString("0 nm"); //SLED 1 wavelength
                    slider1TrackBar.Maximum = mySlave.Capabilities[6];
                    setCurr1Edit.IsEnabled = true;
                    setCurr1Edit.Text = "0.0 mA";
                }
            });



            //Capabilities 12: SLED TEC Default Temp
            int sledTECFactoryDefaultTempReg = (readData[12]); //0 if SLED TEC DNE, otherwise factory default temp in reg value
            mySlave.Capabilities[12] = (int)CalculateSledTECTempFromReg(sledTECFactoryDefaultTempReg, (int)mySlave.FirmwareVersion);
            mySlave.SledTECFactoryDefaultTemp = mySlave.Capabilities[12];
            this.Dispatcher.Invoke(() =>
            {
                sledTECSaveBut.IsEnabled = true;
                sledTECOnOffBut.IsEnabled = true;
                sledTECSetPointEdit.IsEnabled = true;
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
                    waveChangeBut_PM.IsEnabled = true;
                    PMTECSaveBut.IsEnabled = true;
                    CurveBut_PM.IsEnabled = true;
                    ExportBut_PM.IsEnabled = false;
                    List_PM.IsEnabled = true;
                    List_PM.Text = Current_A.Content.ToString();
                    mySlave.PMWavelength = 1550;
                    pmWaveLengthEdit.IsEnabled = true;
                    PMTECSetPointEdit.IsEnabled = true;

                }
            });
            //capabilities[17] =
            mySlave.Capabilities[18] = (int)(2.5 * ((readData[18]) / 65535.0) * 1000); //SLED 1 PC current manufacture default
            mySlave.Capabilities[19] = (int)(2.5 * ((readData[19]) / 65535.0) * 1000); //SLED 2 PC current manufacture default
            mySlave.Capabilities[20] = (int)(2.5 * ((readData[20]) / 65535.0) * 1000); //SLED 3 PC current manufacture default
            mySlave.Capabilities[21] = (int)(2.5 * ((readData[21]) / 65535.0) * 1000); //SLED 4 PC current manufacture default
            mySlave.Capabilities[22] = (int)(2.5 * ((readData[22]) / 65535.0) * 1000); //SLED 5 PC current manufacture default
            mySlave.Capabilities[23] = (int)(2.5 * ((readData[23]) / 65535.0) * 1000); //SLED 6 PC current manufacture default

            mySlave.Capabilities[24] = readData[24];            //Modbus ID manufacture default setting
            mySlave.Capabilities[25] = readData[25];            //OSE Body TEC Temp: 0 if DNE, else default setting

            int PMTECFactoryDefaultTempReg = (readData[26]); //0 if SLED TEC DNE, otherwise factory default temp in reg value
            mySlave.Capabilities[26] = PMTECFactoryDefaultTempReg;
            mySlave.PMTECFactoryDefaultTemp = (int)CalculatePMTECTempFromReg(PMTECFactoryDefaultTempReg, (int)mySlave.FirmwareVersion);  //Power Meter Capabilities: 0 if DNE, 1 if enabled

            //capabilities[27] =
            //capabilities[28] =
            //capabilities[29] =

            mySlave.Capabilities[30] = readData[30];            //TCP IP V4 Manufacture Default Setting address [16-31]
            mySlave.Capabilities[31] = readData[31];            //TCP IP V4 Manufacture Default Setting address [0-15]

            mySlave.Capabilities[32] = readData[32];            //Board Temperature


            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.Mode == MANUAL_MODE)
                {
                    mySlave.CurrentMode = mySlave.Mode;
                    PrintStringToDiagnostics("Manual Mode Selected");
                    whichMode = "Manual Mode";

                    //Turn SLED TEC on
                    mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 50, 1);
                    mySlave.STECOn = 1;

                    //disable modulation button

                    //Check if sled Math.Power is on according to the unit
                    UInt16[] readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,16, 1);

                    if (readData2[0] == 1)
                    {
                        sledTECSaveBut.IsEnabled = true;
                        sledsOnBut.Content = "         On ";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                    }
                    else
                    {
                        mySlave.SledsAreOn = 0;
                        sledsOnBut.Content = "         On ";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                        sledTECSaveBut.IsEnabled = false;
                    }

                    sledTECOnOffBut.IsEnabled = false;

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
                else if (mySlave.Mode == MODULATION_MODE)
                {
                    mySlave.CurrentMode = mySlave.Mode;
                    PrintStringToDiagnostics("Modulation Mode Selected");
                    whichMode = "Modulation Mode";
                    modulationBut.IsEnabled = true;

                    UInt16[] readData2 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,16, 1);
                    if (readData2[0] == 1)
                    {
                        mySlave.SledsAreOn = 1;
                        sledsOnBut.Content = "         On ";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                        sledTECSaveBut.IsEnabled = true;
                        UInt16[] readData3 = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 10, 6);
                        if (mySlave.Capabilities[0] != 0)
                        {

                            Slider1Changed = readData3[0] * 2.5 / 65535.0 * 1000;
                            Slider1_temp = readData3[0] * 2.5 / 65535.0 * 1000;

                        }

                    }
                    else
                    {
                        mySlave.SledsAreOn = 0;
                        sledsOnBut.Content = "         Off";
                        sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                        sledTECSaveBut.IsEnabled = false;

                        Slider1Changed = 0;
                        Slider1_temp = 0;
                    }

                    sledTECOnOffBut.IsEnabled = false;

                    maxBut.IsEnabled = false;
                    minBut.IsEnabled = false;
                    sledsOnBut.IsEnabled = false;

                    slider1TrackBar.IsEnabled = false;

                    setCurr1Edit.IsEnabled = false;
                }
                else if (mySlave.Mode == PC_MODE)
                {
                    mySlave.CurrentMode = mySlave.Mode;
                    PrintStringToDiagnostics("PC Mode Selected");
                    whichMode = "PC Mode";

                    //Check if TEC on
                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,50, 1);
                    if (readData[0] == 1)
                    {
                        mySlave.STECOn = 1;

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
                            //If sleds Power enabled we can click modulation button
                            modulationBut.IsEnabled = true;

                            sledTECOnOffBut.IsEnabled = true;
                            maxBut.IsEnabled = true;
                            minBut.IsEnabled = true;

                            sledsOnBut.IsEnabled = true;

                            sledTECSaveBut.IsEnabled = true;
                        }
                        else
                        {
                            mySlave.SledsAreOn = 0;

                            sledTECOnOffBut.Content = "           Off";
                            sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                            maxBut.IsEnabled = false;
                            minBut.IsEnabled = false;
                            sledsOnBut.IsEnabled = false;

                            sledTECSaveBut.IsEnabled = false;

                            //If SLED TEC is off, disable sliders
                            slider1TrackBar.IsEnabled = false;
                            setCurr1Edit.IsEnabled = false;

                            Slider1Changed = 0;
                            Slider1_temp = 0;
                        }
                    }
                }
            });


            //defaults
            //SetCommunicationsDefaultValues
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 7, 1);
            mySlave.ExistingModbusID = readData[0];

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,1, 2);

            int ExistingIP1 = readData[0] >> 8 & 0x00ff;
            int ExistingIP2 = readData[0] & 0x00ff;
            int ExistingIP3 = readData[1] >> 8 & 0x00ff;
            int ExistingIP4 = readData[1] & 0x00ff;

            mySlave.ExistingIP = ExistingIP1.ToString() + "." + ExistingIP2.ToString() + "." + ExistingIP3.ToString() + "." + ExistingIP4.ToString();
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,5, 1);
            mySlave.ExistingPort = readData[0];

            //set pc mode and man
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,30, 6);
            mySlave.ExistingPcCurr1 = GetDefaultCurrFromRegVal(readData[0]);
            mySlave.ExistingPcCurr2 = GetDefaultCurrFromRegVal(readData[1]);
            mySlave.ExistingPcCurr3 = GetDefaultCurrFromRegVal(readData[2]);
            mySlave.ExistingPcCurr4 = GetDefaultCurrFromRegVal(readData[3]);
            mySlave.ExistingPcCurr5 = GetDefaultCurrFromRegVal(readData[4]);
            mySlave.ExistingPcCurr6 = GetDefaultCurrFromRegVal(readData[5]);
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,36, 6);
            mySlave.ExistingManCurr1 = GetDefaultCurrFromRegVal(readData[0]);
            mySlave.ExistingManCurr2 = GetDefaultCurrFromRegVal(readData[1]);
            mySlave.ExistingManCurr3 = GetDefaultCurrFromRegVal(readData[2]);
            mySlave.ExistingManCurr4 = GetDefaultCurrFromRegVal(readData[3]);
            mySlave.ExistingManCurr5 = GetDefaultCurrFromRegVal(readData[4]);
            mySlave.ExistingManCurr6 = GetDefaultCurrFromRegVal(readData[5]);

            //manufacturer defaults
            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,118, 6);

            mySlave.ManufacturerDefPcCurr1 = GetDefaultCurrFromRegVal(readData[0]);
            mySlave.ManufacturerDefPcCurr2 = GetDefaultCurrFromRegVal(readData[1]);
            mySlave.ManufacturerDefPcCurr3 = GetDefaultCurrFromRegVal(readData[2]);
            mySlave.ManufacturerDefPcCurr4 = GetDefaultCurrFromRegVal(readData[3]);
            mySlave.ManufacturerDefPcCurr5 = GetDefaultCurrFromRegVal(readData[4]);
            mySlave.ManufacturerDefPcCurr6 = GetDefaultCurrFromRegVal(readData[5]);


            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,106, 6);

            mySlave.ManufacturerDefManCurr1 = GetDefaultCurrFromRegVal(readData[0]);
            mySlave.ManufacturerDefManCurr2 = GetDefaultCurrFromRegVal(readData[1]);
            mySlave.ManufacturerDefManCurr3 = GetDefaultCurrFromRegVal(readData[2]);
            mySlave.ManufacturerDefManCurr4 = GetDefaultCurrFromRegVal(readData[3]);
            mySlave.ManufacturerDefManCurr5 = GetDefaultCurrFromRegVal(readData[4]);
            mySlave.ManufacturerDefManCurr6 = GetDefaultCurrFromRegVal(readData[5]);

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,124, 1);
            mySlave.ManufacturerDefModbusID = readData[0];

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,130, 2);
            ExistingIP1 = readData[0] >> 8 & 0x00ff;
            ExistingIP2 = readData[0] & 0x00ff;
            ExistingIP3 = readData[1] >> 8 & 0x00ff;
            ExistingIP4 = readData[1] & 0x00ff;

            mySlave.ManufacturerDefIP = ExistingIP1.ToString() + "." + ExistingIP2.ToString() + "." + ExistingIP3.ToString() + "." + ExistingIP4.ToString();

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,133, 1);
            mySlave.ManufacturerDefPort = readData[0];


            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,112, 1);
            mySlave.ManufacturerSTECTempSetpointDefaultBoot = CalculateSledTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion);

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,135, 2);
            mySlave.ManufacturerOSEBodyTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,137, 1);
            mySlave.ManufacturerOSEBodyTECKpFactorDefault = readData[0];

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,138, 1);
            mySlave.ManufacturerOSEBodyTECKdFactorDefault = readData[0];

            readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,139, 1);
            mySlave.ManufacturerOSEBodyTECKiFactorDefault = readData[0];

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,66, 1);
            mySlave.ExistingSTECTempSetpointDefaultBoot = CalculateSledTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion);

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,55, 2);
            mySlave.ExistingOSEBodyTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,59, 1);
            mySlave.ExistingOSEBodyTECKpFactorDefault = readData[0];

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,62, 1);
            mySlave.ExistingOSEBodyTECKdFactorDefault = readData[0];

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,64, 1);
            mySlave.ExistingOSEBodyTECKiFactorDefault = readData[0];
            if (mySlave.Capabilities[0] != 0)
            {
                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,126, 1);
                mySlave.ManufacturerPMTECTempSetpointDefault = CalculatePMTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion);

                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,140, 2);
                mySlave.ManufacturerPMTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));

                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,142, 1);
                mySlave.ManufacturerPMTECKpFactorDefault = readData[0];

                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,143, 1);
                mySlave.ManufacturerPMTECKdFactorDefault = readData[0];

                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,144, 1);
                mySlave.ManufacturerPMTECKiFactorDefault = readData[0];

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,48, 1);
                mySlave.ExistingPMTECTempSetpointDefault = CalculatePMTECTempFromReg(readData[0], (int)mySlave.FirmwareVersion);

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,70, 2);
                mySlave.ExistingPMTECTimeConstant = ((readData[0] << 16) + (readData[1])) / (2.25 * Math.Pow(10.0, 6));

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,74, 1);
                mySlave.ExistingPMTECKpFactorDefault = readData[0];

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,76, 1);
                mySlave.ExistingPMTECKdFactorDefault = readData[0];

                readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,78, 1);
                mySlave.ExistingPMTECKiFactorDefault = readData[0];
            }

            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,50, 1);
            mySlave.STECOn = readData[0];



            //this is supert important set all the sliderchanged to -1 so that it will do a first time read from intput registers and set sliders
            Slider1Changed = -1;

        }

        private double GetDefaultCurrFromRegVal(UInt16 regVal)
        {
            double returnVal;

            returnVal = Math.Round(((double)regVal * 2.5 / 65535.0 * 1000.0) ,1);

            return returnVal;
        }
        public double CalculateSledTECTempFromReg(double regValue, int firmwareVersion)
        {
            // Temperature [C] = +6.642608455e-57*x^13-2.815855091e-51*x^12+5.359109082e-46*x^11-6.046695868e-41*x^10
            //                    +4.499661883e-36*x^9-2.325069565e-31*x^8+8.55743018e-27*x^7-2.265870514e-22*x^6+4.312143019e-18*x^5   
            //                    -5.841936274e-14*x^4+5.547369278e-10*x^3-3.642822896e-06*x^2+0.01789931806*x^1-66.44775692
            regValue = (6.642608455 * Math.Pow(10.0, -57)) * Math.Pow(regValue, 13) +
               (-2.815855091 * Math.Pow(10.0, -51)) * Math.Pow(regValue, 12) +
               (5.359109082 * Math.Pow(10.0, -46)) * Math.Pow(regValue, 11) +
               (-6.046695868 * Math.Pow(10.0, -41)) * Math.Pow(regValue, 10) +
               (+4.499661883 * Math.Pow(10.0, -36)) * Math.Pow(regValue, 9) +
               (-2.325069565 * Math.Pow(10.0, -31)) * Math.Pow(regValue, 8) +
               (+8.55743018 * Math.Pow(10.0, -27)) * Math.Pow(regValue, 7) +
               (-2.265870514 * Math.Pow(10.0, -22)) * Math.Pow(regValue, 6) +
               (+4.312143019 * Math.Pow(10.0, -18)) * Math.Pow(regValue, 5) +
               (-5.841936274 * Math.Pow(10.0, -14)) * Math.Pow(regValue, 4) +
               (5.547369278 * Math.Pow(10.0, -10)) * Math.Pow(regValue, 3) +
               (-3.642822896 * Math.Pow(10.0, -6)) * Math.Pow(regValue, 2) +
               (+0.01789931806 * regValue) +
               -66.44775692;

            return regValue;
        }
        public double CalculatePMTECTempFromReg(double regValue, int firmwareVersion)
        {
                // Temperature [C] = +1.8461e-39*x^9-5.1866e-34*x^8+6.2921e-29*x^7-4.3106e-24*x^6+1.8359e-19*x^5-5.0545e-15*x^4+9.1282e-11*x^3
                //                   -1.0864e-06*x^2+0.0098962*x^1-62.7528  

                // Temperature [C] = e-39*x^9e-34*x^8+e-29*x^7e-24*x^6+e-19*x^5-5.0545e-15*x^4+e-11*x^3
                //                   e-06*x^2+0.0098962*x^1-62.7528  
                regValue =
                    (+1.8461 * Math.Pow(10.0, -39)) * Math.Pow(regValue, 9) +
                   (-5.1866 * Math.Pow(10.0, -34)) * Math.Pow(regValue, 8) +
                   (6.2921 * Math.Pow(10.0, -29)) * Math.Pow(regValue, 7) +
                   (-4.3106 * Math.Pow(10.0, -24)) * Math.Pow(regValue, 6) +
                   (1.8359 * Math.Pow(10.0, -19)) * Math.Pow(regValue, 5) +
                   (-5.0545 * Math.Pow(10.0, -15)) * Math.Pow(regValue, 4) +
                   (9.1282 * Math.Pow(10.0, -11)) * Math.Pow(regValue, 3) +
                   (-1.0864 * Math.Pow(10.0, -6)) * Math.Pow(regValue, 2) +
                   (0.0098962 * regValue) +
                  -62.7528;

            return regValue;
        }

        public double CalculatePMPowerfromCurr(int PMType, double PMCurrentRead, double PMWavelength)
        {
            double regValue = 0;

            if (PMType == 1)
            {
                regValue = PMCurrentRead /
                (2.32532329792853 * Math.Pow(10.0, -39) * Math.Pow(PMWavelength, 15)
                - 4.92980138623402 * Math.Pow(10.0, -35) * Math.Pow(PMWavelength, 14)
                + 4.8555614650717 * Math.Pow(10.0, -31) * Math.Pow(PMWavelength, 13)
                - 2.94713664748845 * Math.Pow(10.0, -27) * Math.Pow(PMWavelength, 12)
                + 1.23269933832968 * Math.Pow(10.0, -23) * Math.Pow(PMWavelength, 11)
                - 3.76342618717899 * Math.Pow(10.0, -20) * Math.Pow(PMWavelength, 10)
                + 8.66314632263242 * Math.Pow(10.0, -17) * Math.Pow(PMWavelength, 9)
                - 1.53100559626784 * Math.Pow(10.0, -13) * Math.Pow(PMWavelength, 8)
                + 2.09421485663562 * Math.Pow(10.0, -10) * Math.Pow(PMWavelength, 7)
                - 2.21710004541209 * Math.Pow(10.0, -07) * Math.Pow(PMWavelength, 6)
                + 0.000180171977337481 * Math.Pow(PMWavelength, 5)
                - 0.110366589981702 * Math.Pow(PMWavelength, 4)
                + 49.3280831349843 * Math.Pow(PMWavelength, 3)
                - 15185.8725369337 * Math.Pow(PMWavelength, 2)
                + 2879298.2009897 * Math.Pow(PMWavelength, 1)
                - 253459196.322034);
            }

            return regValue;
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

            //PrintStringToDiagnostics("Bootup complete please wait...");

            //foreach (ModBusSlave mySlave in modbusSlaveList)
            //{
            //    if (mySlave != null)
            //    {
            //        PrintStringToDiagnostics("Reading from ID: " + mySlave.SlaveID.ToString());
            //    }
            //}

            //sw.Start();
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


        public void UpdateAndDisplayRxString(string data, string COMport)
        {
            if (commslogging)
            {
                string newString = (COMport + " | RX: " + data + '\n');

                PrintStringToComms(newString);
            }

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
            s += mySlave.HeatSinkTemp.ToString("0.00"); s += ",";

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
            s += mySlave.HeatSinkTempRaw.ToString("0.00"); s += ",";        //We are putting board temperature raw into heat sink raw as well


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

        private void WriteLineToAdminLogFile(ModBusSlave mySlave)
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
            s += mySlave.SledTECCurrRead.ToString("0.00"); s += ",";
            s += mySlave.SledTECCapacity.ToString("0.00"); s += ",";

            s += mySlave.BoardTemperatureN.ToString("0.00"); s += ",";
            s += mySlave.HeatSinkTemp.ToString("0.00"); s += ",";

            s += mySlave.OSEBodyTemp.ToString("0.00"); s += ",";           //OSE Body Temperature. Version 5
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
            s += mySlave.OSEBodyTECTimeConstant.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKpRealtime.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKdRealtime.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKiRealtime.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECCoolingPID.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECHeatingPID.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECErrorSize.ToString("0.00"); s += ",";
            s += mySlave.OSEBodySpeedChangeValue.ToString("0.00"); s += ",";
            s += mySlave.OSEDirectionCorrect.ToString("0.00"); s += ",";


            s += mySlave.PMWavelength.ToString("0.00"); s += ",";
            s += (mySlave.PMRead * 1000000).ToString("0.00"); s += ",";
            s += mySlave.PMPower.ToString("0.00"); s += ",";
            s += mySlave.PMPower.ToString("0.00"); s += ",";

            s += mySlave.PMTECTemp.ToString("0.00"); s += ",";            //PM Temperature. Version 7
            s += mySlave.PMTECTempSetpoint.ToString("0.00"); s += ",";            //PM Temperature Setpoint. Version 7
            s += mySlave.PMTECCurr.ToString("0.00"); s += ",";            //PM Current. Version 7
            s += mySlave.PMTECCapacity.ToString("0.00"); s += ",";     //PM Capacity. Version 7
            s += mySlave.PMTECTimeConstantRealtime.ToString("0.00"); s += ",";
            s += mySlave.PMTECKpRealtime.ToString("0.00"); s += ",";
            s += mySlave.PMTECKdRealtime.ToString("0.00"); s += ",";
            s += mySlave.PMTECKiRealtime.ToString("0.00"); s += ",";
            s += mySlave.PMTECCoolingPID.ToString("0.00"); s += ",";
            s += mySlave.PMTECHeatingPID.ToString("0.00"); s += ",";
            s += mySlave.PMTECErrorSize.ToString("0.00"); s += ",";
            s += mySlave.PMSpeedChangeValue.ToString("0.00"); s += ",";
            s += mySlave.PMDirectionCorrect.ToString("0.00"); s += ",";

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
            s += mySlave.SledTECCurrentRawLogString.ToString("0.00"); s += ",";
            s += mySlave.SledTECCapacityRaw.ToString("0.00"); s += ",";

            s += mySlave.BoardTemperatureRaw.ToString("0.00"); s += ",";
            s += mySlave.HeatSinkTempRaw.ToString("0.00"); s += ",";        //We are putting board temperature raw into heat sink raw as well

            s += mySlave.OSEBodyTempRaw.ToString("0.00"); s += ",";            //OSE Body Temperature. Version 5
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
            s += mySlave.OSEBodyTECTimeConstantRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKpRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKdRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECKiRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECCoolingPIDRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECHeatingPIDRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodyTECErrorSizeRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEBodySpeedChangeValueRaw.ToString("0.00"); s += ",";
            s += mySlave.OSEDirectionCorrectRaw.ToString("0.00"); s += ",";

            s += mySlave.PMWavelength.ToString("0.00"); s += ",";
            s += mySlave.PMReadRaw.ToString("0.00"); s += ",";
            s += mySlave.PMReadRaw.ToString("0.00"); s += ",";
            s += mySlave.PMReadRaw.ToString("0.00"); s += ",";

            s += mySlave.PMTECTempRaw.ToString("0.00"); s += ",";            //PM Temperature. Version 7
            s += mySlave.PMTECTempSetpointRaw.ToString("0.00"); s += ",";            //PM Temperature Setpoint. Version 7
            s += mySlave.PMTECCurrRaw.ToString("0.00"); s += ",";            //OSE Body Current. Version 7
            s += mySlave.PMTECCapacityRaw.ToString("0.00"); s += ",";     //OSE Body Capacity. Version 7
            s += mySlave.PMTECTimeConstantRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECKpRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECKdRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECKiRealtimeRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECCoolingPIDRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECHeatingPIDRaw.ToString("0.00"); s += ",";
            s += mySlave.PMTECErrorSizeRaw.ToString("0.00"); s += ",";
            s += mySlave.PMSpeedChangeValueRaw.ToString("0.00"); s += ",";
            s += mySlave.PMDirectionCorrectRaw.ToString("0.00"); s += ",";


            s += mySlave.FanSpeedReadRaw.ToString("0.00"); s += ",";
            s += mySlave.FanSpeedReadRaw.ToString("0.00"); s += ",";
            s += modeString; s += ",";

            mySlave.slaveAdminLogFileFsWriter.Write(s + "\n");
            mySlave.slaveAdminLogFileFsWriter.Flush();

            //Add the string to the slave's known strings
            mySlave.CurrentLogStrings.Add(s);

            //If we're overflowing the chart, remove the first value
            if (mySlave.CurrentLogStrings.Count > MAX_CHART_VALUES)
            {
                mySlave.CurrentLogStrings.RemoveAt(0);
                if (CurrentValues_Labels.Count > MAX_CHART_VALUES)
                {
                    CurrentValues_Labels.RemoveAt(0);
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

        double CalculateSLED6TempFromReg(double regValue, int firmwareVersion)
        {
            //Calculate SLED6 temp from register based on formula from Modbus Map
            //Thermistor: FH10-6E103GC Mitsubishi  Circuit: MAX6682-7680R


            UInt16 posOrNeg = (UInt16)(((UInt16)regValue) >> 15);
            regValue = (double)((UInt16)(regValue) & 0x7FFF);

            regValue = 3.74216945175521 * Math.Pow(10.0, -14) * Math.Pow(regValue, 6) +
            -5.95587458432679 * Math.Pow(10.0, -11) * Math.Pow(regValue, 5) +
            3.70724277747557 * Math.Pow(10.0, -8) * Math.Pow(regValue, 4) +
            -1.09742089151599 * Math.Pow(10.0, -5) * Math.Pow(regValue, 3) +
            1.52245017366681 * Math.Pow(10.0, -3) * Math.Pow(regValue, 2) +
            4.57891114001541 * Math.Pow(10.0, -2) * regValue;

            if (posOrNeg == 1)
            {
                regValue *= -1.0;
            }
            return regValue;
        }

        double CalculateHeatSinkTempFromReg(double regValue, int firmwareVersion)
        {
            //Calculate Heat Sink temp based on formula from Modbus Map
            //Thermistor: NTCALUG03A103GC  Circuit: MAX6682-7680R
            //	regValue = 4.64990563836727 * pow(10.0, -7) * pow(regValue, 3) +
            //		-2.84776405913262 * pow(10.0, -4) * pow(regValue, 2) +
            //		1.7575701247749 * pow(10.0, -1) * regValue +
            //		-2.62943006151795;

            UInt16 posOrNeg = (UInt16)(((UInt16)regValue) >> 15);
            regValue = (double)((UInt16)(regValue) & 0x7FFF);

            regValue = 3.17355002652106 * Math.Pow(10.0, -14) * Math.Pow(regValue, 6) +
            -4.99508512682520 * Math.Pow(10.0, -11) * Math.Pow(regValue, 5) +
            3.07830909443418 * Math.Pow(10.0, -8) * Math.Pow(regValue, 4) +
            -8.97084451090269 * Math.Pow(10.0, -6) * Math.Pow(regValue, 3) +
            1.20841132404337 * Math.Pow(10.0, -3) * Math.Pow(regValue, 2) +
            6.52086371915175 * Math.Pow(10.0, -2) * regValue;

            if (posOrNeg == 1)
            {
                regValue *= -1.0;
            }

            return regValue;

        }

        double CalculateOSEBodyTempFromReg(double regValue)
        {
            //Calculate OSE Body temp based on formula from Modbus Map
            //Thermistor: NTCALUG03A103GC  Circuit: 10K Divider
            //Temperature = 2.5865e-39*x^9-7.433e-34*x^8+9.1571e-29*x^7-6.3143e-24*x^6
            //+2.6747e-19*x^5-7.2042e-15*x^4+1.2427e-10*x^3-1.3672e-06*x^2+0.010824*x^1-55.3573

            regValue = 2.5865 * Math.Pow(10.0, -39) * Math.Pow(regValue, 9) +
                        -7.433 * Math.Pow(10.0, -34) * Math.Pow(regValue, 8) +
                        9.1571 * Math.Pow(10.0, -29) * Math.Pow(regValue, 7) +
                        -6.3143 * Math.Pow(10.0, -24) * Math.Pow(regValue, 6) +
                        2.6747 * Math.Pow(10.0, -19) * Math.Pow(regValue, 5) +
                        -7.2042 * Math.Pow(10.0, -15) * Math.Pow(regValue, 4) +
                        1.2427 * Math.Pow(10.0, -10) * Math.Pow(regValue, 3) +
                        -1.3672 * Math.Pow(10.0, -6) * Math.Pow(regValue, 2) +
                        0.010824 * regValue +
                        -55.3573;
            return regValue;

        }

        public double CalculateSledTECRegFromTemp(double regValue, int firmwareVersion)
        {
            // Calculate SLED TEC register value from temp based on formula from Modbus Map

                //Register Value = +8.218650025e-22*x^13-1.3018334e-19*x^12-7.615235132e-18*x^11-1.178533451e-15*x^10+9.04899891e-13*x^9
                //                  -7.97775054e-11*x^8-2.656698961e-09*x^7+5.016350378e-07*x^6+7.696368506e-06*x^5-0.001936712264*x^4
                //                  -0.06377762061*x^3+6.594073945*x^2+594.7966412*x^1+15379.32058
                regValue = +8.218650025 * Math.Pow(10.0, -22) * Math.Pow(regValue, 13) +
                           -1.3018334 * Math.Pow(10.0, -19) * Math.Pow(regValue, 12) +
                           -7.615235132 * Math.Pow(10.0, -18) * Math.Pow(regValue, 11) +
                            -1.178533451 * Math.Pow(10.0, -15) * Math.Pow(regValue, 10) +
                            +9.04899891 * Math.Pow(10.0, -13) * Math.Pow(regValue, 9) +
                           -7.97775054 * Math.Pow(10.0, -11) * Math.Pow(regValue, 8) +
                           -2.656698961 * Math.Pow(10.0, -9) * Math.Pow(regValue, 7) +
                            +5.016350378 * Math.Pow(10.0, -7) * Math.Pow(regValue, 6) +
                            +7.696368506 * Math.Pow(10.0, -6) * Math.Pow(regValue, 5) +
                           -0.001936712264 * Math.Pow(regValue, 4) +
                           -0.06377762061 * Math.Pow(regValue, 3) +
                            6.594073945 * Math.Pow(regValue, 2) +
                            +594.7966412 * regValue +
                            +15379.32058;
            return regValue;
        }


        public double CalculatePMTECRegFromTemp(double regValue, int firmwareVersion)
        {

                //Register Value =-1.8291e-14*x^9+1.0478e-11*x^8-1.6737e-09*x^7+2.9367e-08*x^6+1.332e-05*x^5-0.00055747*x^4 
                //                *x^3*x^2*x^1+18828.0435   

                regValue = -1.8291 * Math.Pow(10.0, -14) * Math.Pow(regValue, 9) +
                           +1.0478 * Math.Pow(10.0, -11) * Math.Pow(regValue, 8) +
                           -1.6737 * Math.Pow(10.0, -9) * Math.Pow(regValue, 7) +
                            2.9367 * Math.Pow(10.0, -8) * Math.Pow(regValue, 6) +
                            1.332 * Math.Pow(10.0, -5) * Math.Pow(regValue, 5) +
                           -0.00055747 * Math.Pow(regValue, 4) +
                           -0.078295 * Math.Pow(regValue, 3) +
                            +3.3846 * Math.Pow(regValue, 2) +
                            +594.3495 * regValue +
                            +18828.0435;
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
                                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,7, 1);
                                mySlave.Mode = readData[0];

                                ///////////////////////////////// MANUAL MODE SELECTED CONTINUOUS READ /////////////////////////////////

                                if (mySlave.Mode == MANUAL_MODE)
                                {
                                    whichMode = "Manual Mode";
                                    // BeST SLED is in Manual Mode, log change if necessary
                                    // If we detect a change in "mode" (including no mode at all on bootup)

                                    if (mySlave.Mode != mySlave.CurrentMode)
                                    {

                                        PrintStringToDiagnostics("Manual Mode Selected.");

                                        whichMode = "Manual Mode";


                                        //Turn SLED TEC on
                                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 50, 1);

                                        mySlave.STECOn = 1;



                                        this.Dispatcher.Invoke(() =>
                                        {
                                            //Disable modulation button in Manual Mode and close modulation window
                                            if (modulationWindow != null)
                                                modulationWindow.Visibility = Visibility.Hidden;

                                            mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 200, 0);
                                            mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 201, 0);
                                            modulationBut.IsEnabled = false;
                                        });
                                    }
                                    mySlave.CurrentMode = mySlave.Mode;

                                    //For logfile
                                    modeString = "Manual";

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

                                ///////////////////////////////// MODULATION MODE CONTINUOUS READ SELECTED /////////////////////////////////

                                else if (mySlave.Mode == MODULATION_MODE)
                                {
                                    whichMode = "Modulation";

                                    //Display mode changed if needed
                                    if (mySlave.Mode != mySlave.CurrentMode)
                                    {
                                        PrintStringToDiagnostics("Modulation Mode Selected.");
                                        whichMode = "Modulation";
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            modulationBut.IsEnabled = true;
                                        });
                                    }

                                    mySlave.CurrentMode = mySlave.Mode;
                                    //For logfile
                                    modeString = "Modulation";


                                    //Check if sled  Power is on according to the unit
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 16, 1);
                                    mySlave.SledsAreOn = (readData[0]);

                                    UpdateModulation(mySlave, modulationWindow);
                                }


                                ///////////////////////////////// PC MODE CONTINUOUS READ SELECTED /////////////////////////////////

                                else if (mySlave.Mode == PC_MODE)
                                {
                                    whichMode = "PC Mode";

                                    //Display mode changed if needed
                                    if (mySlave.Mode != mySlave.CurrentMode)
                                    {
                                        //If the previous mode was manual, turn off sleds
                                        if (mySlave.CurrentMode == MANUAL_MODE)
                                        {
                                            mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 0);
                                            mySlave.SledsAreOn = 0;
                                            this.Dispatcher.Invoke(() =>
                                            {
                                                sledsOnBut.IsEnabled = false;
                                            });

                                        }

                                        PrintStringToDiagnostics("PC Mode Selected.");
                                        whichMode = "PC Mode";

                                        this.Dispatcher.Invoke(() =>
                                        {
                                            modulationBut.IsEnabled = true;
                                        });
                                    }
                                    mySlave.CurrentMode = mySlave.Mode;
                                    //modeString is used for logfile string
                                    modeString = "PC Mode";

                                    //enable buttons requiPink for PC mode
                                    // ToggleButtonsEnabled(PC_MODE);

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
                                readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,17, 1);
                                connectionTypeInt = readData[0];
                                commsWindow.Update_ComSettings();


                                this.Dispatcher.Invoke(() =>
                                {
                                    ////If it's our first pass since connecting, read the setpoints and set the trackbars and currents accordingly
                                    //readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,10, 6);
                                    //// Set sliders to SLED current set point
                                    //if (readData[5] > 0)
                                    //{
                                    //    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,10, 6);
                                    //}
                                    //if (mySlave.Capabilities[0] > 0)
                                    //{
                                    //    s1CurrentSetpoint = Math.Round(((readData[0] * 2.5 / 65535.0) * 1000.0), 1);

                                    //    if (s1CurrentSetpoint >= mySlave.Capabilities[6]) //max slider value change later to a variable
                                    //        slider1TrackBar.Value = mySlave.Capabilities[6];
                                    //    else
                                    //        slider1TrackBar.Value = s1CurrentSetpoint;

                                    //}

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

                                    if (sledTECTempGood == 1)
                                        displaybottom.Content = "  Unit Ready " + connectionTypeString;
                                    else
                                        displaybottom.Content = "  Unit Not Ready " + connectionTypeString;

                                    datedisplay.Content = DateTime.Now;

                                    modeEdit.Content = whichMode;

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


                                    if (whichMode == "Manual Mode")
                                    {
                                        if (mySlave.Capabilities[0] > 0)
                                        {
                                            slider1TrackBar.IsEnabled = false;
                                        }
                                       
                                        //If the SLED TEC is on
                                        if (mySlave.STECOn == 1)
                                        {
                                            sledTECOnOffBut.Content = "           On ";
                                            sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                                            sledTECSaveBut.IsEnabled = true;
                                        }
                                        else
                                        {
                                            //If SLED TEC is off, disable sliders

                                            sledTECOnOffBut.Content = "           Off";
                                            sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                            sledTECSaveBut.IsEnabled = false;
                                        }
                                        sledTECOnOffBut.IsEnabled = false;

                                        maxBut.IsEnabled = false;
                                        minBut.IsEnabled = false;
                                        sledsOnBut.IsEnabled = false;

                                        slider1TrackBar.IsEnabled = false;
                                       

                                        setCurr1Edit.IsEnabled = false;
                                        

                                    }
                                    else if (whichMode == "PC Mode")
                                    {
                                        sledTECOnOffBut.IsEnabled = true;

                                        //If the SLED TEC is on
                                        if (mySlave.STECOn == 1)
                                        {
                                            if (mySlave.SledsAreOn == 0)
                                            {
                                                //If SLEDs are off, disable track bar and send 0 current
                                                slider1TrackBar.IsEnabled = false;
                                                Slider1Changed = 0;
                                                Slider1_temp = 0;

                                                setCurr1Edit.IsEnabled = false;
                                               
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

                                                //If sleds Power enabled we can click modulation button
                                                modulationBut.IsEnabled = true;
                                            }

                                            sledTECOnOffBut.Content = "           On ";
                                            sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));

                                            sledsOnBut.IsEnabled = true;
                                            sledTECSaveBut.IsEnabled = true;
                                        }
                                        else
                                        {
                                            //If SLED TEC is off, disable sliders

                                            sledTECOnOffBut.Content = "           Off";
                                            sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                            maxBut.IsEnabled = false;
                                            minBut.IsEnabled = false;
                                            sledsOnBut.IsEnabled = false;

                                            sledTECSaveBut.IsEnabled = false;

                                            slider1TrackBar.IsEnabled = false;
                                            
                                            setCurr1Edit.IsEnabled = false;
                                           
                                        }
                                    }
                                    else if (whichMode == "Modulation")
                                    {


                                        sledTECOnOffBut.IsEnabled = false;
                                        sledTECSaveBut.IsEnabled = true;
                                        maxBut.IsEnabled = false;
                                        minBut.IsEnabled = false;
                                        sledsOnBut.IsEnabled = false;

                                        slider1TrackBar.IsEnabled = false;
                                        setCurr1Edit.IsEnabled = false;
                                        
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

                                    //Monitor Diodes 1
                                    mySlave.MonDiode1ReadRaw = readData[6];      //Monitor diode sled 1 raw value
                                    mySlave.MonDiode1RawLogString = (int)mySlave.MonDiode1ReadRaw;

                                    UInt16[] currentSenseReadBytes = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 20, 1);

                                    //SLED 1-6 Monitor Diode Reading: Input Register 26-31
                                    //if current sense raw less than 90 or slider at zero show zero
                                    if ((currentSenseReadBytes[0] <= 90) || (slider1TrackBar.Value == 0))
                                    {
                                        mySlave.MonDiode1ReadVal = 0.0;
                                    }
                                    //otherwise either current sense is not reading less than 90 or our slider is above zero
                                    else
                                    {

                                        //If firmware 1, use new tweaked curr sense value as Mon Diode Read raw
                                        if (mySlave.FirmwareVersion > 1)
                                        {
                                            mySlave.MonDiode1ReadVal = 5.0 * (mySlave.MonDiode1ReadRaw / (65535.0 * 5000.0 * 2.0));
                                        }
                                        else
                                        {
                                            mySlave.MonDiode1ReadVal = 5.0 * (mySlave.MonDiode1ReadRaw / (65535.0 * 5000.0 * 2.0)) + (mySlave.MonDiode1ReadRaw - 6520) * Math.Pow(10, -6) / 1545;
                                        }
                                    }

                                


                                    //If all sliders at zero show 0 mon diode read

                                if (slider1TrackBar.Value == 0) 
                                    {
                                        mySlave.MonDiode1ReadVal = 0;
                                    }


                                    //Show Monitor and Current Sense Values 
                                    if ((showRawValues))
                                    {

                                        if (mySlave.Capabilities[0] != 0)
                                        {
                                            actualCurr1Edit.Text = mySlave.Sled1CurrSenseRaw.ToString("0");
                                            monDiode1Edit.Text = mySlave.MonDiode1RawLogString.ToString("0");
                                        }
                                        else
                                        {
                                            actualCurr1Edit.Text = "";
                                            monDiode1Edit.Text = "";
                                        }
                                    }
                                    else
                                    {

                                        if (mySlave.Capabilities[0] != 0)
                                        {
                                            actualCurr1Edit.Text = Math.Round((mySlave.ActualCurr1ReadVal * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[1];
                                            monDiode1Edit.Text = Math.Round((mySlave.MonDiode1ReadVal * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.MonDiode1ReadVal).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.MonDiode1ReadVal).Split(',')[1];
                                        }
                                        else
                                        {
                                            actualCurr1Edit.Text = "";
                                            monDiode1Edit.Text = "";
                                        }
                                    }

                                    //For all other versions show real Heat Sink Temperature
                                   
                                        if (showRawValues)
                                        {
                                            heatSinkTempEdit.Text = mySlave.HeatSinkTempRaw.ToString("");

                                        }
                                        else
                                        {
                                            heatSinkTempEdit.Text = mySlave.HeatSinkTemp.ToString("0.0 ⁰C");
                                        }

                                    

                                    string disp;


                                    if (showRawValues)
                                    {

                                            sledTECCurrReadEdit.Text = (mySlave.SledTECCurrRaw).ToString("");
                                            sledTECCapacityEdit.Text = (mySlave.SledTECCapacityRaw).ToString("");

                                    }
                                    else
                                    {
                                        sledTECTempReadEdit.Text = (mySlave.SledTECTemp).ToString("0.0 ⁰C");

                                            sledTECCurrReadEdit.Text = Math.Round((mySlave.SledTECCurr * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.SledTECCurr).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.SledTECCurr).Split(',')[1];
                                            sledTECCapacityEdit.Text = (mySlave.SledTECCapacity).ToString("0.0") + "%";

                                    }

                                    focusedControl = FocusManager.GetFocusedElement(this);
                                    tBox = focusedControl as TextBox;

                                    if (mySlave.Capabilities[16] != 0)
                                    {

                                        tBox = focusedControl as TextBox;
                                        if (tBox != null)
                                        {
                                            if (tBox.Name != "pmWaveLengthEdit")
                                            {
                                                pmWaveLengthEdit.Text = (mySlave.PMWavelength).ToString("0.0 nm");
                                                waveChangeBut_PM.IsDefault = false;
                                            }
                                            else
                                            {
                                                waveChangeBut_PM.IsDefault = true;
                                                pmWaveLengthEdit_temp = pmWaveLengthEdit.Text;
                                            }
                                        }
                                        else
                                        {
                                            pmWaveLengthEdit.Text = (mySlave.PMWavelength).ToString("0.0 nm");
                                            waveChangeBut_PM.IsDefault = false;
                                        }

                                        if (showRawValues)
                                        {
                                            pmCurrEdit.Text = (mySlave.PMReadRaw).ToString("");
                                            pmPowerMWEdit.Text = (mySlave.PMPowerRaw).ToString("");
                                        }
                                        else
                                        {
                                            if ((mySlave.Model - 120) == 20001)
                                            {
                                                pmCurrLabel.Content = "Voltage";
                                                pmCurrEdit.Text = Math.Round(mySlave.PMRead, 5).ToString("0.00000 V");
                                                pmPowerMWEdit.Text = Math.Round((mySlave.PMPower * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.PMPower).Split(',')[0]))), 1).ToString("0.0") + (CheckMagnitude(mySlave.PMPower).Split(',')[1]).Replace("A", "W");
                                            }
                                            else
                                            {
                                                pmCurrLabel.Content = "Current";
                                                pmCurrEdit.Text = Math.Round((mySlave.PMRead * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.PMRead).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.PMRead).Split(',')[1];
                                                pmPowerMWEdit.Text = Math.Round((mySlave.PMPower * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.PMPower).Split(',')[0]))), 1).ToString("0.0") + (CheckMagnitude(mySlave.PMPower).Split(',')[1]).Replace("A", "W");
                                            }


                                        }

                                    }


                                    if (mySlave.Capabilities[26] != 0)
                                    {

                                        tBox = focusedControl as TextBox;
                                        if (tBox != null)
                                        {
                                            if (tBox.Name != "PMTECSetPointEdit")
                                            {
                                                if (showRawValues)
                                                {
                                                    PMTECSetPointEdit.Text = (mySlave.PMTECTempSetpointRaw).ToString("");
                                                }
                                                else
                                                {
                                                    PMTECSetPointEdit.Text = (mySlave.PMTECTempSetpoint).ToString("0.0 ⁰C");
                                                }
                                                PMTECSaveBut.IsDefault = false;
                                            }
                                            else
                                            {
                                                PMTECSaveBut.IsDefault = true;
                                                PMTECSetPointEdit_temp = PMTECSetPointEdit.Text;
                                            }
                                        }
                                        else
                                        {
                                            if (showRawValues)
                                            {
                                                PMTECSetPointEdit.Text = (mySlave.PMTECTempSetpointRaw).ToString("");
                                            }
                                            else
                                            {
                                                PMTECSetPointEdit.Text = (mySlave.PMTECTempSetpoint).ToString("0.0 ⁰C");
                                            }
                                            PMTECSaveBut.IsDefault = false;
                                        }

                                        if (showRawValues)
                                        {

                                            PMTECTempReadEdit.Text = (mySlave.PMTECTempRaw).ToString("");
                                            PMTECCurrReadEdit.Text = (mySlave.PMTECCurrRaw).ToString("");
                                            PMTECCapacityEdit.Text = (mySlave.PMTECCapacityRaw).ToString("");
                                        }
                                        else
                                        {
                                            PMTECTempReadEdit.Text = (mySlave.PMTECTemp).ToString("0.0 ⁰C");
                                            PMTECCurrReadEdit.Text = Math.Round((mySlave.PMTECCurr * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.PMTECCurr).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.PMTECCurr).Split(',')[1];
                                            PMTECCapacityEdit.Text = (mySlave.PMTECCapacity).ToString("0.0") + "%";
                                        }
                                        if (mySlave.PMTECHeatOrCool == 0)
                                        {
                                            pmheatOrCoolEdit.Background = new SolidColorBrush(Color.FromRgb(24, 89, 209));
                                            pmheatOrCoolEdit.Text = "Cooling";
                                            pmheatOrCoolEdit.Opacity = 0.8;

                                        }
                                        else
                                        {
                                            pmheatOrCoolEdit.Background = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                            pmheatOrCoolEdit.Text = "Heating";
                                            pmheatOrCoolEdit.Opacity = 1;
                                        }

                                    }

                                   
                                        if (mySlave.OSEHeatOrCool == 0)
                                        {
                                            heatOrCoolEdit.Background = new SolidColorBrush(Color.FromRgb(24, 89, 209));
                                            heatOrCoolEdit.Text = "Cooling";
                                            heatOrCoolEdit.Opacity = 0.8;

                                        }
                                        else
                                        {
                                            heatOrCoolEdit.Background = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                                            heatOrCoolEdit.Text = "Heating";
                                            heatOrCoolEdit.Opacity = 1;
                                        }
                                   
                                   


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

                                    // Check if TEC is on
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,50, 1);
                                    mySlave.STECOn = readData[0];

                                    //if the sled tec is off, turn the sleds off
                                    if (mySlave.STECOn == 0)
                                    {
                                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 0);
                                    }

                                    //Get SLED TEC Setpoint Realtime
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,60, 1);
                                    mySlave.SledTECTempSetpointRaw = readData[0];
                                    mySlave.SledTECTempSetpoint = Math.Round(CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointRaw, (int)mySlave.FirmwareVersion), 1);

                                    //Get SLED TEC Temp Setpoint Default
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,66, 1);
                                    mySlave.SledTECTempSetpointDefaultRaw = readData[0];
                                    mySlave.SledTECTempSetpointDefault = Math.Round(CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointDefaultRaw, (int)mySlave.FirmwareVersion), 1);
                                    
                                    //Get SLED TEC Curr Setpoint Realtime
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,68, 2);
                                    mySlave.SLEDTECCurrSetRaw = readData[0];
                                    mySlave.SLEDTECCurrSet = (double)mySlave.SLEDTECCurrSetRaw * 3.3 / 65535;

                                    //Get SLED TEC Curr Setpoint Default
                                    mySlave.SLEDTECCurrSetDefaultRaw = readData[1];
                                    mySlave.SLEDTECCurrSetDefault = (double)mySlave.SLEDTECCurrSetDefaultRaw * 3.3 / 65535;
                                    

                                    focusedControl = FocusManager.GetFocusedElement(this);
                              
                                    tBox = null;
                                    tBox = focusedControl as TextBox;
                                    if (tBox != null)
                                    {
                                        if (tBox.Name != "sledTECSetPointEdit")
                                        {
                                            
                                            if (showRawValues)
                                            {
                                                sledTECSetPointEdit.Text = mySlave.SledTECTempSetpointRaw.ToString("0");
                                            }
                                            else
                                            {
                                                sledTECSetPointEdit.Text = mySlave.SledTECTempSetpoint.ToString("0.0 ⁰C");
                                            }
                                            sledTECSaveBut.IsDefault = false;
                                        }
                                        else
                                        {
                                            if (sledTECSaveBut.IsDefault == false)
                                            {
                                                sledTECSetPointEdit.Text = "";
                                            }
                                            sledTECSaveBut.IsDefault = true;
                                            sledTECSetPointEdit_temp = sledTECSetPointEdit.Text;

                                        }
                                    }
                                    else
                                    {
                                        if (showRawValues)
                                        {
                                            sledTECSetPointEdit.Text = mySlave.SledTECTempSetpointRaw.ToString("0");
                                        }
                                        else
                                        {
                                            sledTECSetPointEdit.Text = mySlave.SledTECTempSetpoint.ToString("0.0 ⁰C");
                                        }
                                        sledTECSaveBut.IsDefault = false;
                                    }


                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 20, 35);
                                    //Board Temperature

                                    //Board Temperature reg 40
                                    mySlave.BoardTemperatureRaw = readData[20];

                                    boardTemperature = CalculateBoardTempFromReg(mySlave.BoardTemperatureRaw, (int)mySlave.FirmwareVersion);
                                    mySlave.BoardTemperatureN = boardTemperature;

                                       //Heat Sink - Isolator Temperature - OSE Body TEC Temperature
                                    if (mySlave.Capabilities[14] == 1)
                                    {
                                        mySlave.HeatSinkTemp = CalculateHeatSinkTempFromReg(mySlave.HeatSinkTempRaw, (int)mySlave.FirmwareVersion);
                                    }
                                    //OSE Body Capacity Input Reg 38
                                    mySlave.OSEBodyCapacityRaw = readData[18];
                                    mySlave.OSEBodyCapacity = Math.Round(mySlave.OSEBodyCapacityRaw / 65535.0 * 100.0, 1);

                                    //OSE Body Accuracy Input Reg 39
                                    mySlave.SledTECSTempSetpointDefaultBootRaw = readData[19];
                                    mySlave.SledTECSTempSetpointDefaultBoot = mySlave.SledTECSTempSetpointDefaultBootRaw;

                                    mySlave.SledTECSTempSetpointDefaultRealtimeRaw = readData[19];
                                    mySlave.SledTECSTempSetpointDefaultRealtime = mySlave.SledTECSTempSetpointDefaultRealtimeRaw;
                                    //Heat Sink TEC (HeatSink TEC) Temp is Input Reg 45
                                    mySlave.HeatSinkTempRaw = readData[25];

                                    //OSE Body TEC Temp is Input Reg 42
                                    mySlave.OSEBodyTempRaw = readData[22];
                                    mySlave.OSEBodyTemp = CalculateOSEBodyTempFromReg(mySlave.OSEBodyTempRaw);

                                    //OSE Body TEC Current Reading is Input Reg 54
                                    mySlave.OSEBodyCurrRaw = readData[34];
                                    mySlave.OSEBodyCurr = (5 * mySlave.OSEBodyCurrRaw) / (0.02 * 50 * 65535);

                                    //OSE Body TEC Status is Input Reg 46
                                    mySlave.OSEHeatOrCool = readData[26] >> 15;

                                    //SLED TEC Temperature Reading: Input Register 32
                                    mySlave.SledTECTempRaw = readData[12];
                                    mySlave.SledTECTemp = CalculateSledTECTempFromReg(mySlave.SledTECTempRaw, (int)mySlave.FirmwareVersion);
                                     
                                    if (mySlave.SledTECTemp >= 32 && temp_too_hot == false)
                                    {
                                        temp_too_hot = true;
                                        PrintStringToDiagnostics("SLED Temperature too high. SLED Controls will be disabled until temperature goes down.");
                                    }
                                    else if (mySlave.SledTECTemp < 32 && temp_too_hot == true)
                                    {
                                        temp_too_hot = false;
                                        PrintStringToDiagnostics("SLED Temperature under limit. SLED Controls are enabled.");
                                    }

                                    //SLED TEC Temperature Good: Input Register 34
                                    sledTECTempGood = readData[14] & 0x1;
                                    // no need to combine bytes if masking with only lowest bit


                                    //Sled TEC Current: Input Register 33
                                    mySlave.SledTECCurrRaw = readData[13];
                                    mySlave.SledTECCurrentRawLogString = readData[13];
                                    mySlave.SledTECCurrRead = mySlave.SledTECCurrRaw * 5.0 /(0.02*50*65535.0);


                                    //TEC status contains the heating/cooling bit in MSB Register 34Register 33
                                    mySlave.SledTECHeatOrCool = readData[14] >> 15;

                                    //SLED TEC capacity input Reg 35
                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID, 35, 1);
                                    mySlave.SledTECCapacityDefaultRaw = readData[0];
                                    mySlave.SledTECCapacityDefault = Math.Round(((readData[0]) / 65535.0 * 100.0), 1);
                                    mySlave.SledTECCapacityRaw = readData[0];
                                    mySlave.SledTECCapacity = Math.Round(((readData[0]) / 65535.0 * 100.0), 1);

                                    //OSETECTimeConstant
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,55, 4);
                                    mySlave.OSEBodyTECTimeConstantDefaultBootRaw = (readData[0] << 16) + readData[1];
                                    mySlave.OSEBodyTECTimeConstantDefaultBoot = mySlave.OSEBodyTECTimeConstantDefaultBootRaw / (2.25 * Math.Pow(10.0, 6));
                                    mySlave.OSEBodyTECTimeConstantRaw = (readData[2] << 16) + readData[3];
                                    mySlave.OSEBodyTECTimeConstant = mySlave.OSEBodyTECTimeConstantRaw / (2.25 * Math.Pow(10.0, 6));


                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,61, 6);

                                    //OSEBodyTECKp
                                    mySlave.OSEBodyTECKpRealtimeRaw = readData[0];
                                    mySlave.OSEBodyTECKpRealtime = mySlave.OSEBodyTECKpRealtimeRaw;
                                    mySlave.OSEBodyTECKpDefaultBootRaw = readData[1];
                                    mySlave.OSEBodyTECKpDefaultBoot = mySlave.OSEBodyTECKpDefaultBootRaw;

                                    //OSEBodyTECKd

                                    mySlave.OSEBodyTECKdRealtimeRaw = readData[2];
                                    mySlave.OSEBodyTECKdRealtime = mySlave.OSEBodyTECKdRealtimeRaw;
                                    mySlave.OSEBodyTECKdDefaultBootRaw = readData[3];
                                    mySlave.OSEBodyTECKdDefaultBoot = mySlave.OSEBodyTECKdDefaultBootRaw;

                                    //OSEBodyTECKi

                                    mySlave.OSEBodyTECKiRealtimeRaw = readData[4];
                                    mySlave.OSEBodyTECKiRealtime = mySlave.OSEBodyTECKiRealtimeRaw;
                                    mySlave.OSEBodyTECKiDefaultBootRaw = readData[5];
                                    mySlave.OSEBodyTECKiDefaultBoot = mySlave.OSEBodyTECKiDefaultBootRaw;

                                    //OSEBodyTECCoolingPID
                                    readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,500, 5);

                                    mySlave.OSEBodyTECCoolingPIDRaw = readData[0];
                                    mySlave.OSEBodyTECCoolingPID = mySlave.OSEBodyTECCoolingPIDRaw * 100 / 65535;
                                    mySlave.OSEBodyTECHeatingPIDRaw = readData[1];
                                    mySlave.OSEBodyTECHeatingPID = mySlave.OSEBodyTECHeatingPIDRaw * 100 / 65535;

                                    //OSEBodyTECErrorSize
                                    mySlave.OSEBodyTECErrorSizeRaw = readData[2];
                                    if (mySlave.SledTECTempSetpointRaw > mySlave.SledTECTempRaw)
                                    {
                                        mySlave.OSEBodyTECErrorSize = CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointRaw - mySlave.OSEBodyTECErrorSizeRaw, (int)mySlave.FirmwareVersion) - mySlave.SledTECTempSetpoint;
                                    }
                                    else if (mySlave.SledTECTempSetpointRaw < mySlave.SledTECTempRaw)
                                    {
                                        mySlave.OSEBodyTECErrorSize = CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointRaw + mySlave.OSEBodyTECErrorSizeRaw, (int)mySlave.FirmwareVersion) - mySlave.SledTECTempSetpoint;
                                    }
                                    //OSEBodySpeedChangeValue
                                    mySlave.OSEBodySpeedChangeValueRaw = readData[3];
                                    if (mySlave.OSEDirectionCorrect == 0)
                                    {
                                        mySlave.OSEBodySpeedChangeValue = mySlave.SledTECTemp - CalculateSledTECTempFromReg(mySlave.SledTECTempRaw + mySlave.OSEBodySpeedChangeValueRaw, (int)mySlave.FirmwareVersion);
                                    }
                                    else if (mySlave.OSEDirectionCorrect == 1)
                                    {
                                        mySlave.OSEBodySpeedChangeValue = mySlave.SledTECTemp - CalculateSledTECTempFromReg(mySlave.SledTECTempRaw - mySlave.OSEBodySpeedChangeValueRaw, (int)mySlave.FirmwareVersion);
                                    }
                                    //OSEDirectionCorrect
                                    mySlave.OSEDirectionCorrectRaw = readData[4];
                                    mySlave.OSEDirectionCorrect = readData[4];

                                    // Get fan speed
                                    readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,67, 1);
                                    mySlave.FanSpeedReadRaw = readData[0];
                                    mySlave.FanSpeed = Math.Floor((7.0 * (mySlave.FanSpeedReadRaw / 65535.0) * 10.0) + 0.5) / 10.0;

                                    //Power Meter
                                    if (mySlave.Capabilities[16] != 0)
                                    {
                                        //PM Current: Input Register 43 and 44
                                        readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,43, 2);
                                        mySlave.PMReadRaw = (readData[0] << 16) + readData[1];
                                        if ((mySlave.Model - 120) == 20001)
                                        {
                                            mySlave.PMRead = (double)(5 * (double)mySlave.PMReadRaw / 65535);
                                            mySlave.PMPower = 0;
                                        }
                                        else
                                        {
                                            mySlave.PMRead = (double)(5 * (double)mySlave.PMReadRaw / (100 * (Math.Pow(2.0, 28) - 1)));
                                            mySlave.PMPower = CalculatePMPowerfromCurr(mySlave.Capabilities[16], mySlave.PMRead, mySlave.PMWavelength);
                                        }
                                        
                                    }

                                    //Power Meter TEC
                                    if (mySlave.Capabilities[26] != 0)
                                    {
                                        //Get PM TEC Setpoint Realtime
                                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,47, 2);
                                        mySlave.PMTECTempSetpointRaw = readData[0];
                                        mySlave.PMTECTempSetpoint = Math.Round(CalculatePMTECTempFromReg(mySlave.PMTECTempSetpointRaw, (int)mySlave.FirmwareVersion), 1);

                                        mySlave.PMTECTempSetpointDefaultRaw = readData[1];
                                        mySlave.PMTECTempSetpointDefault = Math.Round(CalculatePMTECTempFromReg(mySlave.PMTECTempSetpointDefaultRaw, (int)mySlave.FirmwareVersion), 1);

                                        readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,43, 11);
                                        mySlave.PMTECTempRaw = readData[6];
                                        mySlave.PMTECTemp = Math.Round(CalculatePMTECTempFromReg(mySlave.PMTECTempRaw, (int)mySlave.FirmwareVersion), 1);
                                       
                                        //PM TEC Current: Input Register 53
                                        mySlave.PMTECCurrRaw = readData[10];
                                        mySlave.PMTECCurrentRawLogString = readData[9];
                                        mySlave.PMTECCurr = mySlave.PMTECCurrRaw * 5 / (0.02 * 50 * 65535);

                                        //TEC status contains the heating/cooling bit in MSB Register 50
                                        mySlave.PMTECHeatOrCool = readData[7] >> 15;

                                        //SLED TEC capacity input Reg 51
                                        mySlave.PMTECCapacityRaw = readData[8];
                                        mySlave.PMTECCapacity = Math.Round((mySlave.PMTECCapacityRaw / 65535.0 * 100.0), 1);

                                        //PMTECTimeConstant
                                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,70, 10);
                                        mySlave.PMTECTimeConstantDefaultBootRaw = (readData[0] << 16) + readData[1];
                                        mySlave.PMTECTimeConstantDefaultBoot = Math.Round(mySlave.PMTECTimeConstantDefaultBootRaw / (2.25 * Math.Pow(10.0, 6)), 1);
                                        mySlave.PMTECTimeConstantRealtimeRaw = (readData[2] << 16) + readData[3];
                                        mySlave.PMTECTimeConstantRealtime = Math.Round(mySlave.PMTECTimeConstantRealtimeRaw / (2.25 * Math.Pow(10.0, 6)), 1);

                                        //PMTECKp
                                        mySlave.PMTECKpDefaultBootRaw = readData[4];
                                        mySlave.PMTECKpDefaultBoot = mySlave.PMTECKpDefaultBootRaw;
                                        mySlave.PMTECKpRealtimeRaw = readData[5];
                                        mySlave.PMTECKpRealtime = mySlave.PMTECKpRealtimeRaw;

                                        //PMTECKd
                                        mySlave.PMTECKdDefaultBootRaw = readData[6];
                                        mySlave.PMTECKdDefaultBoot = mySlave.PMTECKdDefaultBootRaw;
                                        mySlave.PMTECKdRealtimeRaw = readData[7];
                                        mySlave.PMTECKdRealtime = mySlave.PMTECKdRealtimeRaw;

                                        //PMTECKi
                                        mySlave.PMTECKiDefaultBootRaw = readData[8];
                                        mySlave.PMTECKiDefaultBoot = mySlave.PMTECKiDefaultBootRaw;
                                        mySlave.PMTECKiRealtimeRaw = readData[9];
                                        mySlave.PMTECKiRealtime = mySlave.PMTECKiRealtimeRaw;

                                        //PMTECCoolingPID
                                        readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,505, 5);

                                        mySlave.PMTECCoolingPIDRaw = readData[0];
                                        mySlave.PMTECCoolingPID = mySlave.PMTECCoolingPIDRaw * 100 / 65535;
                                        mySlave.PMTECHeatingPIDRaw = readData[1];
                                        mySlave.PMTECHeatingPID = mySlave.PMTECHeatingPIDRaw * 100 / 65535;

                                        //PMTECErrorSize
                                        mySlave.PMTECErrorSizeRaw = readData[2];
                                        if (mySlave.SledTECTempSetpointRaw > mySlave.SledTECTempRaw)
                                        {
                                            mySlave.PMTECErrorSize = CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointRaw - mySlave.PMTECErrorSizeRaw, (int)mySlave.FirmwareVersion) - mySlave.SledTECTempSetpoint;
                                        }
                                        else if (mySlave.SledTECTempSetpointRaw < mySlave.SledTECTempRaw)
                                        {
                                            mySlave.PMTECErrorSize = CalculateSledTECTempFromReg(mySlave.SledTECTempSetpointRaw + mySlave.PMTECErrorSizeRaw, (int)mySlave.FirmwareVersion) - mySlave.SledTECTempSetpoint;
                                        }
                                        //PMSpeedChangeValue
                                        mySlave.PMSpeedChangeValueRaw = readData[3];
                                        if (mySlave.PMDirectionCorrect == 0)
                                        {
                                            mySlave.PMSpeedChangeValue = mySlave.SledTECTemp - CalculateSledTECTempFromReg(mySlave.SledTECTempRaw + mySlave.PMSpeedChangeValueRaw, (int)mySlave.FirmwareVersion);
                                        }
                                        else if (mySlave.PMDirectionCorrect == 1)
                                        {
                                            mySlave.PMSpeedChangeValue = mySlave.SledTECTemp - CalculateSledTECTempFromReg(mySlave.SledTECTempRaw - mySlave.PMSpeedChangeValueRaw, (int)mySlave.FirmwareVersion);
                                        }
                                        //PMDirectionCorrect
                                        mySlave.PMDirectionCorrectRaw = readData[4];
                                        mySlave.PMDirectionCorrect = readData[4];
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
                            if (mySlave.AdminLogginBut)
                            {
                                WriteLineToAdminLogFile(mySlave);
                            }
                            if (mySlave.NewDiagnosticFileBut)
                            {
                                WriteLineToAdminLogFile(mySlave);
                                mySlave.NewDiagnosticFileBut = false;
                                mySlave.slaveAdminLogFileFsWriter.Close();
                            }


                            //If we have a disconnect
                            if (mySlave.WhichCycle != CONTINUOUS_READ)
                                break;

                            


                            //Charting 
                            if (graphing == true)
                            {
                                this.Dispatcher.Invoke(() => {
                                     if (List_PM.Text == Current_A.Content.ToString())
                                    {
                                        if ((mySlave.Model - 120) == 20001)
                                        {
                                            ValuesChart_YAxis.MinValue = Math.Round(Math.Min(mySlave.PMRead, ValuesChart_YAxis.MinValue + 0.008) - 0.008, 2);
                                            ValuesChart_YAxis.MaxValue = Math.Round(Math.Max(mySlave.PMRead, ValuesChart_YAxis.MaxValue - 0.008) + 0.008, 2);
                                            ValuesChart_YAxis_Ticks.Step = Math.Round((ValuesChart_YAxis.MaxValue - ValuesChart_YAxis.MinValue) / 21, 3);
                                            ValuesChart.Series[0].Values.Add(mySlave.PMRead);
                                            if ((int)Series0_axisX_label.Count > graphing_limit)
                                            {
                                                // remove that number of items from the start of the list
                                                Series0_axisX_label.RemoveRange(0, 1);
                                                ValuesChart.Series[0].Values.RemoveAt(0);
                                            }
                                        }
                                        else
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

                                    }
                                    else if (List_PM.Text == Power_mW.Content.ToString())
                                    {
                                        if ((mySlave.Model - 120) == 20001)
                                        {
                                        }
                                        else
                                        {
                                            ValuesChart_YAxis.MinValue = Math.Round(Math.Min(mySlave.PMPower * 1000000, ValuesChart_YAxis.MinValue + 0.1) - 0.1, 2);
                                            ValuesChart_YAxis.MaxValue = Math.Round(Math.Max(mySlave.PMPower * 1000000, ValuesChart_YAxis.MaxValue - 0.1) + 0.1, 2);
                                            ValuesChart_YAxis_Ticks.Step = Math.Round((ValuesChart_YAxis.MaxValue - ValuesChart_YAxis.MinValue) / 21, 3);
                                            ValuesChart.Series[1].Values.Add(mySlave.PMPower * 1000000);
                                            Series0_axisX_label.Add(DateTime.Now.ToString());
                                            if ((int)Series0_axisX_label.Count > graphing_limit)
                                            {
                                                // remove that number of items from the start of the list
                                                Series0_axisX_label.RemoveRange(0, 1);
                                                ValuesChart.Series[1].Values.RemoveAt(0);
                                            }
                                        }
                                    }
                                    else if (List_PM.Text == Power_dBm.Content.ToString())
                                    {
                                        if ((mySlave.Model - 120) == 20001)
                                        {
                                        }
                                        else
                                        {
                                            ValuesChart_YAxis.MinValue = Math.Round(Math.Min(mySlave.PMTECTemp, ValuesChart_YAxis.MinValue + 0.1) - 0.1, 2);
                                            ValuesChart_YAxis.MaxValue = Math.Round(Math.Max(mySlave.PMTECTemp, ValuesChart_YAxis.MaxValue - 0.1) + 0.1, 2);
                                            ValuesChart_YAxis_Ticks.Step = Math.Round((ValuesChart_YAxis.MaxValue - ValuesChart_YAxis.MinValue) / 21, 3);
                                            ValuesChart.Series[2].Values.Add(mySlave.PMTECTemp);
                                            Series0_axisX_label.Add(DateTime.Now.ToString());
                                            if ((int)Series0_axisX_label.Count > graphing_limit)
                                            {
                                                // remove that number of items from the start of the list
                                                Series0_axisX_label.RemoveRange(0, 1);
                                                ValuesChart.Series[2].Values.RemoveAt(0);
                                            }
                                        }

                                    }
                                });

                                //Thread.Sleep(100);
                            }

                        }


                    }
                }

            }
            catch  (Exception e)
            {
                looping = false;
                temp_too_hot = false;
                ClearMainWindow();
                PrintStringToDiagnostics("Connection to SLED lost. Returning to Initialization..."+e);
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
                sledTECSaveBut.IsEnabled = false;
                FanSpeedSetBut.IsEnabled = false;
                Start_PM.IsEnabled = false;
                Stop_PM.IsEnabled = false;
                Clear_PM.IsEnabled = false;
                waveChangeBut_PM.IsEnabled = false;
                PMTECSaveBut.IsEnabled = false;
                sledsOnBut.IsEnabled = false;
                sledTECOnOffBut.IsEnabled = false;
                modulationBut.IsEnabled = false;
                LoggingBut.IsEnabled = false;
                DefaultsBut.IsEnabled = false;
                CurveBut_PM.IsEnabled = false;
                ExportBut_PM.IsEnabled = false;
                pmWaveLengthEdit.IsEnabled = false;
                PMTECSetPointEdit.IsEnabled = false;

                sledTECSetPointEdit.IsEnabled = false;
                fanSpeedSetPointEdit.IsEnabled = false;
                List_PM.IsEnabled = false;

                slider1TrackBar.IsEnabled = false;
                slider1TrackBar.Value = 0;

                setCurr1Edit.IsEnabled = false;
                sledsOnIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                pmheatOrCoolEdit.Background = new SolidColorBrush(Colors.White);
                heatOrCoolEdit.Background = new SolidColorBrush(Colors.White);

                var alltextboxes = this.main_window_grid.Children.OfType<TextBox>();
                foreach (var textbox in alltextboxes)
                {
                    if (textbox.Name != "Diagnostics")
                    {
                        textbox.Clear();
                    }
                }

                Wavelength1.Content = "";

                ValuesChart.Series[0].Values.Clear();
                ValuesChart.Series[1].Values.Clear();
                ValuesChart.Series[2].Values.Clear();
                ValuesChart_YAxis.MinValue = Math.Round((double)1000000000, 2);
                ValuesChart_YAxis.MaxValue = Math.Round((double)-1000000000, 2);
                Series0_axisX_label.Clear();

                displaybottom.Content = "";
            });
        }

        public void UpdateModulation(ModBusSlave mySlave, Modulation modulationWindow)
        {
            //Get duty cycle and modulation frequency
            ushort[] readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 4);
            mySlave.ModulationFrequencyRegVal = (UInt32)(((UInt32)readData[0] << 16) | readData[1]);
            mySlave.DutyCycleRegVal = ((UInt32)(((UInt32)readData[2] << 16) | readData[3]) + 2);

            this.Dispatcher.Invoke(() =>
            {
                IInputElement focusedControl = FocusManager.GetFocusedElement(modulationWindow);
                TextBox tBox = null;
                tBox = focusedControl as TextBox;

                if (tBox != null)
                {
                    if (tBox.Name == "ModFreqEdit")
                    {
                        modulationWindow.ChangeModFreqBut.IsDefault = true;
                        ModFreqEdit_temp = modulationWindow.ModFreqEdit.Text;
                        DutyCycleEdit_temp = modulationWindow.DutyCycleEdit.Text;
                    }
                    else
                    {
                        //Check for divide by zero
                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 4);
                        mySlave.ModulationFrequencyRegVal = (UInt32)(((UInt32)readData[0] << 16) | readData[1]);
                        if (mySlave.ModulationFrequencyRegVal != 0)
                        {
                            //Get frequency from reg value
                            Double ModulationFrequencyDbl = (13.5 * Math.Pow(10, 6)) / (double)mySlave.ModulationFrequencyRegVal;
                            modulationWindow.ModFreqEdit.Text = (Math.Floor((ModulationFrequencyDbl * 10.0) + 0.5) / 10.0).ToString("0");
                        }
                        else
                        {
                            modulationWindow.ModFreqEdit.Text = "NaN";
                        }
                        modulationWindow.ChangeModFreqBut.IsDefault = false;

                    }

                    if (tBox.Name == "DutyCycleEdit")
                    {
                        modulationWindow.ChangeDutyCycleBut.IsDefault = true;
                        DutyCycleEdit_temp = modulationWindow.DutyCycleEdit.Text;

                        //Check for divide by zero
                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 4);
                        mySlave.ModulationFrequencyRegVal = (UInt32)(((UInt32)readData[0] << 16) | readData[1]);
                        if (mySlave.ModulationFrequencyRegVal != 0)
                        {
                            //Get frequency from reg value
                            Double ModulationFrequencyDbl = (13.5 * Math.Pow(10, 6)) / (double)mySlave.ModulationFrequencyRegVal;
                            modulationWindow.ModFreqEdit.Text = (Math.Floor((ModulationFrequencyDbl * 10.0) + 0.5) / 10.0).ToString("0");
                        }
                    }
                    else
                    {
                        //Check for divide by zero
                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 4);
                        mySlave.ModulationFrequencyRegVal = (UInt32)(((UInt32)readData[0] << 16) | readData[1]);
                        mySlave.DutyCycleRegVal = ((UInt32)(((UInt32)readData[2] << 16) | readData[3]) + 2);
                        if (mySlave.ModulationFrequencyRegVal != 0)
                        {
                            //Get dutycycle from reg value
                            Double DutyCycleDbl = 100 * mySlave.DutyCycleRegVal / mySlave.ModulationFrequencyRegVal;
                            modulationWindow.DutyCycleEdit.Text = Math.Floor(DutyCycleDbl).ToString("0");
                        }
                        else
                        {
                            modulationWindow.DutyCycleEdit.Text = "NaN";
                        }
                        modulationWindow.ChangeDutyCycleBut.IsDefault = false;

                    }

                }
                else
                {
                    //Check for divide by zero
                    if (mySlave.ModulationFrequencyRegVal != 0)
                    {
                        readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 4);
                        mySlave.ModulationFrequencyRegVal = (UInt32)(((UInt32)readData[0] << 16) | readData[1]);
                        mySlave.DutyCycleRegVal = ((UInt32)(((UInt32)readData[2] << 16) | readData[3]) + 2);
                        //Get frequency from reg value
                        Double ModulationFrequencyDbl = (13.5 * Math.Pow(10, 6)) / (double)mySlave.ModulationFrequencyRegVal;
                        modulationWindow.ModFreqEdit.Text = (Math.Floor((ModulationFrequencyDbl * 10.0) + 0.5) / 10.0).ToString("0");
                        //Get dutycycle from reg value
                        Double DutyCycleDbl = 100 * mySlave.DutyCycleRegVal / mySlave.ModulationFrequencyRegVal;
                        modulationWindow.DutyCycleEdit.Text = Math.Floor(DutyCycleDbl).ToString("0");
                    }
                    else
                    {
                        modulationWindow.ModFreqEdit.Text = "NaN";
                        modulationWindow.DutyCycleEdit.Text = "NaN";
                    }

                    modulationWindow.ChangeModFreqBut.IsDefault = false;
                    modulationWindow.ChangeDutyCycleBut.IsDefault = false;
                }
            });


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Get status of modulation registers and display
            readData = mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 2);

            UInt16 whichSledsRegVal;
            UInt16 modulationEnabled;

            modulationEnabled = readData[0];
            whichSledsRegVal = readData[1];

            //Use bitwise mask to find which sleds are modulating
            UInt16 sled1ModOn = (UInt16)(whichSledsRegVal & 1);

            this.Dispatcher.Invoke(() =>
            {
                if (modulationEnabled > 0)
                {
                    modulationWindow.ModEnabled = 1;
                    modulationWindow.ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                    modulationWindow.EnableBut.Content = "      On ";

                    //Display SLEDmodulationWindow.Modulation status based onmodulationWindow.Modulation reg and capabilities
                    if (mySlave.Capabilities[0] != 0)
                    {
                        if (s1CurrentSetpoint > 0)
                        {
                            modulationWindow.Mod1.IsEnabled = true;
                        }
                        else
                        {
                            modulationWindow.Mod1.IsEnabled = false;
                        }
                        if (sled1ModOn > 0)
                        {
                            modulationWindow.SLED1Indicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                            modulationWindow.Mod1.Content = "      On ";
                        }
                        else
                        {
                            modulationWindow.SLED1Indicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                            modulationWindow.Mod1.Content = "     Off";
                        }
                    }
                    else
                    {
                        modulationWindow.DcOff1.Text = "N/A";
                        modulationWindow.Mod1.IsEnabled = false;
                    }

                }
                else
                {
                    modulationWindow.ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                    modulationWindow.EnableBut.Content = "     Off";
                    modulationWindow.Mod1.IsEnabled = false;
                    modulationWindow.SLED1Indicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                    modulationWindow.Mod1.Content = "     Off";
                }
                modulationWindow.DcOff1.Text = mySlave.Sled1CurrentSetpoint.ToString("0.0") + " mA";
            });
        }

        void UpdateAdminVariables(ModBusSlave mySlave)
        {
            this.Dispatcher.Invoke(() =>
            {
                

                //Set the values on About Window

                adminWindow.CurrSenseRaw1Edit.Text = mySlave.Sled1CurrSenseRaw.ToString("0");
                adminWindow.CurrSenseCalc1Edit.Text = Math.Round((mySlave.ActualCurr1ReadVal * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[0]))),1).ToString("0.0") + CheckMagnitude(mySlave.ActualCurr1ReadVal).Split(',')[1];
                
                adminWindow.MonDiodeRaw1Edit.Text = mySlave.MonDiode1RawLogString.ToString("0");     //Display the raw value for the monitor diode in the debug window
                adminWindow.MonDiodeCalc1Edit.Text = Math.Round((mySlave.MonDiode1ReadVal * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.MonDiode1ReadVal).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.MonDiode1ReadVal).Split(',')[1];


                adminWindow.SLEDTECTempSetpointDefaultBox.Text = mySlave.SledTECTempSetpointDefault.ToString("0.0") + (" ⁰C");
                adminWindow.SLEDTECTempSetpointDefaultRaw.Content = mySlave.SledTECTempSetpointDefaultRaw;

                adminWindow.SLEDTECTempSetpointRealtimeBox.Text = mySlave.SledTECTempSetpoint.ToString("0.0") + (" ⁰C");
                adminWindow.SLEDTECTempSetpointRealtimeRaw.Content = mySlave.SledTECTempSetpointRaw;

                adminWindow.SLEDTECTempBox.Text = mySlave.SledTECTemp.ToString("0.0") + (" ⁰C");
                adminWindow.SledTECSTempReadingeditRaw.Content = mySlave.SledTECTempRaw;


                adminWindow.SLEDTECCurrentRaw.Content = mySlave.SledTECCurrentRawLogString;
                adminWindow.SLEDTECCurrentBox.Text = Math.Round((mySlave.SledTECCurrRead * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.SledTECCurrRead).Split(',')[0]))),1).ToString("0.0") + CheckMagnitude(mySlave.SledTECCurrRead).Split(',')[1];


                adminWindow.SLEDTECCurrSetpointRealtimeRaw.Content = mySlave.SLEDTECCurrSetRaw;
                adminWindow.SLEDTECCurrSetpointRealtimeBox.Text = Math.Round((mySlave.SLEDTECCurrSet * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.SLEDTECCurrSet).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.SLEDTECCurrSet).Split(',')[1];

                adminWindow.SLEDTECCurrSetpointDefaultRaw.Content = mySlave.SLEDTECCurrSetDefaultRaw;
                adminWindow.SLEDTECCurrSetpointDefaultBox.Text = Math.Round((mySlave.SLEDTECCurrSetDefault * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.SLEDTECCurrSetDefault).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.SLEDTECCurrSetDefault).Split(',')[1];

                adminWindow.SLEDTECCapacityRealtimeBox.Text = mySlave.SledTECCapacity.ToString("0.0") + (" %");
                adminWindow.SLEDTECCapacityRealtimeRaw.Content = mySlave.SledTECCapacityRaw;

                if (mySlave.SledTECHeatOrCool == 0)
                    {
                        adminWindow.SLEDTECStatusBox.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                        adminWindow.SLEDTECStatusBox.Text = "Cooling";
                    }
                    else
                    {
                        adminWindow.SLEDTECStatusBox.Background = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                        adminWindow.SLEDTECStatusBox.Text = "Heating";
                    }
                    adminWindow.SLEDTECStatusRaw.Content = mySlave.SledTECHeatOrCool;

             
                        adminWindow.OSETempBox.Text = mySlave.OSEBodyTemp.ToString("0.0") + (" ⁰C");
                        adminWindow.OSETempRaw.Content = mySlave.OSEBodyTempRaw;
                    

                    adminWindow.OSECapacityBox.Text = mySlave.OSEBodyCapacity.ToString("0.0") + "%";
                    adminWindow.OSECapacityRaw.Content = mySlave.OSEBodyCapacityRaw;

                    
                        adminWindow.OSECurrentRaw.Content = mySlave.OSEBodyCurrRaw;
                        adminWindow.OSECurrentBox.Text = Math.Round((mySlave.OSEBodyCurr * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.OSEBodyCurr).Split(',')[0]))),1).ToString("0.0") + CheckMagnitude(mySlave.OSEBodyCurr).Split(',')[1];

                    


                        if (mySlave.OSEHeatOrCool == 0)
                        {
                            adminWindow.OSEStatusBox.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                            adminWindow.OSEStatusBox.Text = "Cooling";
                        }

                        else
                        {
                            adminWindow.OSEStatusBox.Background = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                            adminWindow.OSEStatusBox.Text = "Heating";
                        }

                        adminWindow.OSEStatusRaw.Content = mySlave.OSEHeatOrCool;

                       

                        adminWindow.OSEBodyTECTimeConstantBox.Text = mySlave.OSEBodyTECTimeConstant.ToString("0.0 s");
                        adminWindow.OSEBodyTECTimeConstantRaw.Content = mySlave.OSEBodyTECTimeConstantRaw;

                        adminWindow.KpBox.Text = mySlave.OSEBodyTECKpRealtimeRaw.ToString();
                        adminWindow.KpRaw.Content = mySlave.OSEBodyTECKpRealtimeRaw;

                        adminWindow.KdBox.Text = mySlave.OSEBodyTECKdRealtimeRaw.ToString();
                        adminWindow.KdRaw.Content = mySlave.OSEBodyTECKdRealtimeRaw;

                        adminWindow.KiBox.Text = mySlave.OSEBodyTECKiRealtimeRaw.ToString();
                        adminWindow.KiRaw.Content = mySlave.OSEBodyTECKiRealtimeRaw;

                        adminWindow.OSEBodyTECCoolingPIDBox.Text = mySlave.OSEBodyTECCoolingPID.ToString("0.0") + (" %");
                        adminWindow.OSEBodyTECCoolingPIDRaw.Content = mySlave.OSEBodyTECCoolingPIDRaw;

                        adminWindow.OSEBodyTECHeatingPIDBox.Text = mySlave.OSEBodyTECHeatingPID.ToString("0.0") + (" %");
                        adminWindow.OSEBodyTECHeatingPIDRaw.Content = mySlave.OSEBodyTECHeatingPIDRaw;

                        adminWindow.OSEBodyTECErrorSizeBox.Text = mySlave.OSEBodyTECErrorSize.ToString("0.0") + (" ⁰C");
                        adminWindow.OSEBodyTECErrorSizeRaw.Content = mySlave.OSEBodyTECErrorSizeRaw;

                        adminWindow.OSEBodyTECSpeedChangeValueBox.Text = mySlave.OSEBodySpeedChangeValue.ToString("0.0") + (" ⁰C");
                        adminWindow.OSEBodyTECSpeedChangeValueRaw.Content = mySlave.OSEBodySpeedChangeValueRaw;

                        //output.sprintf("%.1f °C", RoundToNearestTenth(OSEDirectionCorrect));
                        //threadParams->aboutForm->OSEDirectionCorrectBox->Text = output.c_str();
                        adminWindow.OSEBodyTECDirectionCorrectRaw.Content = mySlave.OSEDirectionCorrectRaw;

                        if (mySlave.OSEDirectionCorrectRaw == 0)
                        {
                            adminWindow.OSEBodyTECDirectionCorrectBox.Background = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                            adminWindow.OSEBodyTECDirectionCorrectBox.Text = "Right Direction";
                        }

                        else
                        {
                            adminWindow.OSEBodyTECDirectionCorrectBox.Background =  new SolidColorBrush(Color.FromRgb(205, 92, 92));
                            adminWindow.OSEBodyTECDirectionCorrectBox.Text = "Wrong Direction";
                        }

                if (mySlave.Capabilities[26] != 0)
                {
                    //PM ADMIN BOXES

                    adminWindow.PMTECTempSetpointDefaultBox.Text = mySlave.PMTECTempSetpointDefault.ToString("0.0") + (" ⁰C");
                    adminWindow.PMTECSetpointDefaultRaw.Content = mySlave.PMTECTempSetpointDefaultRaw;

                    adminWindow.PMTECTempSetpointRealtimeBox.Text = mySlave.PMTECTempSetpoint.ToString("0.0") + (" ⁰C");
                    adminWindow.PMTECTempSetpointRealtimeRaw.Content = mySlave.PMTECTempSetpointRaw;

                    adminWindow.PMTECTimeConstantBox.Text = mySlave.PMTECTimeConstantRealtime.ToString("0.0 s");
                    adminWindow.PMTECTimeConstantRaw.Content = mySlave.PMTECTimeConstantRealtimeRaw;

                    adminWindow.PMKpBox.Text = mySlave.PMTECKpRealtimeRaw.ToString();
                    adminWindow.PMKpRaw.Content = mySlave.PMTECKpRealtimeRaw;

                    adminWindow.PMKdBox.Text = mySlave.PMTECKdRealtimeRaw.ToString();
                    adminWindow.PMKdRaw.Content = mySlave.PMTECKdRealtimeRaw;

                    adminWindow.PMKiBox.Text = mySlave.PMTECKiRealtimeRaw.ToString();
                    adminWindow.PMKiRaw.Content = mySlave.PMTECKiRealtimeRaw;

                    adminWindow.PMTempBox.Text = mySlave.PMTECTemp.ToString("0.0") + (" ⁰C");
                    adminWindow.PMTempRaw.Content = mySlave.PMTECTempRaw;
                    adminWindow.PMCurrentRaw.Content = mySlave.PMTECCurrRaw;
                    adminWindow.PMCurrentBox.Text = Math.Round((mySlave.PMTECCurr * Math.Pow(10.0, int.Parse(CheckMagnitude(mySlave.PMTECCurr).Split(',')[0]))), 1).ToString("0.0") + CheckMagnitude(mySlave.PMTECCurr).Split(',')[1];
                    adminWindow.PMCapacityRaw.Content = mySlave.PMTECCapacityRaw;
                    adminWindow.PMCapacityBox.Text = mySlave.PMTECCapacity.ToString("0.0") + "%";
                    if (mySlave.PMTECHeatOrCool == 0)
                    {
                        adminWindow.PMStatusBox.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                        adminWindow.PMStatusBox.Text = "Cooling";
                    }

                    else
                    {
                        adminWindow.PMStatusBox.Background = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                        adminWindow.PMStatusBox.Text = "Heating";
                    }

                    adminWindow.PMStatusRaw.Content = mySlave.PMTECHeatOrCool;


                    adminWindow.PMTECCoolingPIDBox.Text = mySlave.PMTECCoolingPID.ToString("0.0") + (" %");
                    adminWindow.PMTECCoolingPIDRaw.Content = mySlave.PMTECCoolingPIDRaw;

                    adminWindow.PMTECHeatingPIDBox.Text = mySlave.PMTECHeatingPID.ToString("0.0") + (" %");
                    adminWindow.PMTECHeatingPIDRaw.Content = mySlave.PMTECHeatingPIDRaw;

                    adminWindow.PMTECErrorSizeBox.Text = mySlave.PMTECErrorSize.ToString("0.0") + (" ⁰C");
                    adminWindow.PMTECErrorSizeRaw.Content = mySlave.PMTECErrorSizeRaw;

                    adminWindow.PMTECSpeedChangeValueBox.Text = mySlave.PMSpeedChangeValue.ToString("0.0") + (" ⁰C");
                    adminWindow.PMTECSpeedChangeValueRaw.Content = mySlave.PMSpeedChangeValueRaw;

                    adminWindow.PMTECDirectionCorrectRaw.Content = mySlave.PMDirectionCorrectRaw;

                    if (mySlave.PMDirectionCorrectRaw == 0)
                    {
                        adminWindow.PMTECDirectionCorrectBox.Background = new SolidColorBrush(Colors.Green);
                        adminWindow.PMTECDirectionCorrectBox.Text = "Right Direction";
                    }

                    else
                    {
                        adminWindow.PMTECDirectionCorrectBox.Background = new SolidColorBrush(Colors.Red);
                        adminWindow.PMTECDirectionCorrectBox.Text = "Wrong Direction";
                    }


                   

                }
                ushort[] readData = mbClient.ReadInputRegisters((byte)mySlave.ModbusID,1000, 4);
                adminWindow.ADC_PM_READ_Box.Text = readData[0].ToString("0.0");


           
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

        public void GetSledCurrentsPCMode(ModBusSlave mySlave)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (mySlave.Capabilities[0] > 0)
                {
                    mySlave.Sled1CurrentSetpoint = slider1TrackBar.Value;
                    //sled1CurrentSetpoint = 50.0;
                }
            });
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

        private void modulationBut_Click(object sender, RoutedEventArgs e)
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            if (selectedSlaveID != 0)
            {

                //start modulation window  
                UpdateModulation(mySlave, modulationWindow);
                modulationWindow.Visibility = Visibility.Visible;
                modulationWindow.Activate();
                modulationWindow.WindowState = WindowState.Normal;
            }
            else
                MessageBox.Show("Not connected device detected.");
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

        private void sledTECOnOffBut_Click(object sender, RoutedEventArgs e)
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];

            if (mySlave.CurrentMode == PC_MODE && mySlave.STECOn == 1)
            {

                //Turn off SLEDs
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 16, 0);
                mySlave.SledsAreOn = 0;
                mySlave.STECOn = 0;
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 10, 0);
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 11, 0);
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 12, 0);
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 13, 0);
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 14, 0);
                mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 15, 0);

                sledTECOnOffBut.Content = "           Off";
                sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
                PrintStringToDiagnostics("SLED TEC Power Off successful.");
            }

            //Flip the logic
            else if (mySlave.CurrentMode == PC_MODE && mySlave.STECOn == 0)
            {
                mySlave.STECOn = 1;
                sledTECOnOffBut.Content = "           On ";
                sledTECOnOffIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
                PrintStringToDiagnostics("SLED TEC Power On successful.");
            }


            mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 50, (UInt16)mySlave.STECOn);

        }
        
        private void sledTECSaveBut_Click(object sender, RoutedEventArgs e)
        {

            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];
            if (sledTECSetPointEdit_temp != null)
            {
                if (sledTECSetPointEdit_temp.Length > 0 && IsTextNumeric(sledTECSetPointEdit_temp.Replace("⁰C", "")) == true)
                {

                    // PrintStringToDiagnostics(sledTECSetPointEdit.Text);
                    double givenTemp = double.Parse(sledTECSetPointEdit_temp.Replace("⁰C", ""));
                    if (showRawValues)
                    {
                        if (givenTemp < SLED_TEC_MIN_RAW_TEMP)
                        {
                            givenTemp = SLED_TEC_MIN_RAW_TEMP;
                        }
                        else if (givenTemp > SLED_TEC_MAX_RAW_TEMP)
                        {
                            givenTemp = SLED_TEC_MAX_RAW_TEMP;
                        }
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 60, (UInt16)givenTemp);

                        double regValue = Math.Round(CalculateSledTECTempFromReg(givenTemp, (int)mySlave.FirmwareVersion), 0);
                        PrintStringToDiagnostics("SLED TEC Setpoint set successful. SLED TEC Temp=" + regValue.ToString("0.0") + " ⁰C. Raw=" + givenTemp.ToString(""));
                        Keyboard.Focus(sledTECSaveBut);
                        Keyboard.ClearFocus();

                    }
                    else
                    {
                        if (givenTemp < SLED_TEC_MIN_TEMP)
                        {
                            givenTemp = SLED_TEC_MIN_TEMP;
                        }
                        else if (givenTemp > SLED_TEC_MAX_TEMP)
                        {
                            givenTemp = SLED_TEC_MAX_TEMP;
                        }

                        double regValue = Math.Round(CalculateSledTECRegFromTemp(givenTemp, (int)mySlave.FirmwareVersion), 0);
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 60, (UInt16)regValue);
                        Keyboard.Focus(sledTECSaveBut);
                        Keyboard.ClearFocus();
                        // MessageBox.Show
                        PrintStringToDiagnostics("SLED TEC Setpoint set successful. SLED TEC Temp=" + givenTemp.ToString("0.0") + " ⁰C. Raw=" + regValue.ToString(""));

                    }
                }
            }
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
        
        private void PMTECSaveBut_Click(object sender, RoutedEventArgs e)
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];
            if (PMTECSetPointEdit_temp != null)
            {
                if (PMTECSetPointEdit_temp.Length > 0 && IsTextNumeric(PMTECSetPointEdit_temp.Replace("⁰C", "").Replace(" ", "")) == true)
                {
                    double givenTemp = double.Parse(PMTECSetPointEdit_temp.Replace("⁰C", "").Replace(" ", ""));
                    if (showRawValues)
                    {

                        if (givenTemp < PM_TEC_MIN_RAW_TEMP)
                        {
                            givenTemp = PM_TEC_MIN_RAW_TEMP;
                        }
                        else if (givenTemp > PM_TEC_MAX_RAW_TEMP)
                        {
                            givenTemp = PM_TEC_MAX_RAW_TEMP;
                        }
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 47, (UInt16)givenTemp);

                        double regValue = Math.Round(CalculatePMTECTempFromReg(givenTemp, (int)mySlave.FirmwareVersion), 0);
                        PrintStringToDiagnostics("PM TEC Setpoint set successful. PM TEC Temp=" + regValue.ToString("0.0") + " ⁰C. Raw=" + givenTemp.ToString(""));
                        Keyboard.Focus(PMTECSaveBut);
                        Keyboard.ClearFocus();
                    }
                    else
                    {
                        if (givenTemp < PM_TEC_MIN_TEMP)
                        {
                            givenTemp = PM_TEC_MIN_TEMP;
                        }
                        else if (givenTemp > PM_TEC_MAX_TEMP)
                        {
                            givenTemp = PM_TEC_MAX_TEMP;
                        }

                        double regValue = Math.Round(CalculatePMTECRegFromTemp(givenTemp, (int)mySlave.FirmwareVersion), 0);
                        mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 47, (UInt16)regValue);
                        Keyboard.Focus(PMTECSaveBut);
                        Keyboard.ClearFocus();
                        PrintStringToDiagnostics("PM TEC Setpoint set successful. PM TEC Temp=" + givenTemp.ToString("0.0") + " ⁰C. Raw=" + regValue.ToString(""));
                    }
                }
            }
        }

        private void waveChangeBut_PM_Click(object sender, RoutedEventArgs e)
        {
            ModBusSlave mySlave = modbusSlaveList[selectedSlaveID];
            if (pmWaveLengthEdit_temp != null)
            {
                if (pmWaveLengthEdit_temp.Length > 0 && IsTextNumeric(pmWaveLengthEdit_temp.Replace("nm", "").Replace(" ", "")) == true)
                {

                    mySlave.PMWavelength = double.Parse(pmWaveLengthEdit_temp.ToString().Replace("nm", "").Replace(" ", ""));
                    Keyboard.Focus(waveChangeBut_PM);
                    Keyboard.ClearFocus();
                    PrintStringToDiagnostics("PM Wavelength set successful. PM Wavelength =" + mySlave.PMWavelength.ToString("0.0") + " nm");
                    this.Dispatcher.Invoke(() =>
                    {
                        pmWaveLengthEdit.Text = (mySlave.PMWavelength).ToString("0.0 nm");
                    });

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
                Keyboard.Focus(sledTECSaveBut);
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
    