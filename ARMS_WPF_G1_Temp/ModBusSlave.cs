using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace ARMS_WPF_G1_Temp
{
    public class ModBusSlave
    {
        private int slaveID = 0;
        private int modbusID = 255;
        private int slider6TrackBar = 0;
        private int whichCycle = -1;
        private double firmwareVersion;
        private double model;
        private int[] capabilities = new int[33] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };
        public bool continuousSet = false;
        private bool continuousFirstRead = false;
        private int chartMode = -1;
        private string slavePublicLogFilePath;
        private int currentLeftIndex = -1;
        private int currentRightIndex = -1;
        private int publicLogfileLineCount;
        private int adminLogfileLineCount;
        public FileStream slavePublicLogFileOutputStream;
        public FileStream publicIStreamLeft;
        public FileStream publicIStreamRight;
        public StreamReader publicFSReaderLeft;
        public StreamReader publicFSReaderRight;
        public StreamWriter slavePublicLogFileFsWriter;
        public FileStream slaveAdminLogFileOutputStream;
        public FileStream adminIStreamLeft;
        public FileStream adminIStreamRight;
        public StreamReader adminFSReaderLeft;
        public StreamReader adminFSReaderRight;
        public StreamWriter slaveAdminLogFileFsWriter;
        private List<string> currentLogStrings = new List<string>();
        private List<string> currentPublicLogStrings = new List<string>();
        private int mode;
        private int sTECOn;
        private int sledsAreOn;
        private double sled1CurrentSetpoint;
        private double sled1CurrentSetpointRaw;
        private double sled2CurrentSetpoint;
        private double sled2CurrentSetpointRaw;
        private double sled3CurrentSetpoint;
        private double sled3CurrentSetpointRaw;
        private double sled4CurrentSetpoint;
        private double sled4CurrentSetpointRaw;
        private double sled5CurrentSetpoint;
        private double sled5CurrentSetpointRaw;
        private double sled6CurrentSetpoint;
        private double sled6CurrentSetpointRaw;
        private double sledTECTempSetpointRaw;
        private double sledTECTempSetpoint;
        private double sledTECTempSetpointDefaultRaw;
        private double sledTECTempSetpointDefault;
        private int sledTECCurrSetRaw;
        private double sledTECCurrSet;
        private int sledTECCurrSetDefaultRaw;
        private double sledTECCurrSetDefault;
        private int sledTECCurrSetFactoryDefaultRaw;
        private double sledTECCurrSetFactoryDefault;
        private double sled1CurrSenseRaw;
        private double actualCurr1ReadVal;
        private double monDiode1ReadRaw;
        private int monDiode1RawLogString;
        private double monDiode1ReadVal;
        private double boardTemperatureRaw;
        private double boardTemperatureN;
        private int posOrNegS6Submount;
        private double oSEBodyCapacityRaw;
        private double oSEBodyCapacity;
        private double sledTECSTempSetpointDefaultBootRaw;
        private double sledTECSTempSetpointDefaultBoot;
        private double sledTECSTempSetpointDefaultRealtimeRaw;
        private double sledTECSTempSetpointDefaultRealtime;
        private double heatSinkSetpointRead;
        private double heatSinkSetpointReadRaw;
        private double oSEBodyTemp;
        private double oSEBodyTempRaw;
        private double oSEBodyCurr;
        private double oSEBodyCurrRaw;
        private double oSEHeatOrCool;
        private int posOrNegHeatSink;
        private double sledTECCurr;
        private double sledTECCurrRaw;
        private int sledTECCurrentRawLogString;
        private double sledTECCurrRead;
        private int sledTECHeatOrCool;
        private double sledTECCapacityRaw;
        private double sledTECCapacity;
        private double sledTECCapacityDefaultRaw;
        private double sledTECCapacityDefault;
        private double fanSpeedReadRaw;
        private double fanSpeed;
        private double sledTECFactoryDefaultTemp;
        private int existingModbusID;
        private string existingIP;
        private int existingPort;
        private double existingPMTECTemperatureSetpointDefaultBoot;
        private double existingPcCurr1;
        private double existingPcCurr2;
        private double existingPcCurr3;
        private double existingPcCurr4;
        private double existingPcCurr5;
        private double existingPcCurr6;
        private double existingManCurr1;
        private double existingManCurr2;
        private double existingManCurr3;
        private double existingManCurr4;
        private double existingManCurr5;
        private double existingManCurr6;
        private double sledTECTempRaw;
        private double heatSinkTempRaw;
        private int currentMode;
        private bool editingSledTECTemp;
        private double sledTECTemp;
        private double heatSinkTemp;

        private int oSEBodyTECTimeConstantDefaultBootRaw;
        private double oSEBodyTECTimeConstantDefaultBoot;
        private int oSEBodyTECTimeConstantRaw;
        private double oSEBodyTECTimeConstant;
        private int oSEBodyTECKpRealtimeRaw;
        private double oSEBodyTECKpRealtime;
        private int oSEBodyTECKpDefaultBootRaw;
        private double oSEBodyTECKpDefaultBoot;
        private int oSEBodyTECKiRealtimeRaw;
        private double oSEBodyTECKiRealtime;
        private double oSEBodyTECKdRealtime;
        private int oSEBodyTECKdRealtimeRaw;
        private double oSEBodyTECKdDefaultBoot;
        private int oSEBodyTECKdDefaultBootRaw;
        private int oSEBodyTECKiDefaultBootRaw;
        private double oSEBodyTECKiDefaultBoot;
        private int oSEBodyTECCoolingPIDRaw;
        private double oSEBodyTECCoolingPID;
        private int oSEBodyTECHeatingPIDRaw;
        private double oSEBodyTECHeatingPID;
        private int oSEDirectionCorrectRaw;
        private double oSEDirectionCorrect;
        private int oSEBodyTECErrorSizeRaw;
        private double oSEBodyTECErrorSize;
        private int oSEBodySpeedChangeValueRaw;
        private double oSEBodySpeedChangeValue;

        private double manufacturerDefPcCurr1;
        private double manufacturerDefPcCurr2;
        private double manufacturerDefPcCurr3;
        private double manufacturerDefPcCurr4;
        private double manufacturerDefPcCurr5;
        private double manufacturerDefPcCurr6;

        private double manufacturerDefManCurr1;
        private double manufacturerDefManCurr2;
        private double manufacturerDefManCurr3;
        private double manufacturerDefManCurr4;
        private double manufacturerDefManCurr5;
        private double manufacturerDefManCurr6;

        private double existingSTECTempSetpointDefaultBoot;
        private double existingOSEBodyTECTimeConstant;
        private double existingOSEBodyTECKpFactorDefault;
        private double existingOSEBodyTECKdFactorDefault;
        private double existingOSEBodyTECKiFactorDefault;

        private double manufacturerSTECTempSetpointDefaultBoot;
        private double manufacturerOSEBodyTECTimeConstant;
        private double manufacturerOSEBodyTECKpFactorDefault;
        private double manufacturerOSEBodyTECKdFactorDefault;
        private double manufacturerOSEBodyTECKiFactorDefault;

        private double manufacturerDefModbusID;
        private string manufacturerDefIP;
        private double manufacturerDefPort;

        private bool adminLogginBut;
        private bool newDiagnosticFileBut;


        private string slaveAdminLogFilePath;
        private UInt32 modulationFrequencyRegVal;
        private UInt32 dutyCycleRegVal;


        private int pMReadRaw;
        private double pMRead;
        private double pMWavelength;
        private int pMPowerRaw;
        private double pMPower;

        private int pMTECTempSetpointRaw;
        private double pMTECTempSetpoint;
        private int pMTECTempSetpointDefaultRaw;
        private double pMTECTempSetpointDefault;
        private int pMTECTempRaw;
        private double pMTECTemp;


        private double pMTECCurr;
        private double pMTECCurrRaw;
        private int pMTECCurrentRawLogString;
        private double pMTECCurrRead;
        private int pMTECHeatOrCool;
        private double pMTECCapacityRaw;
        private double pMTECCapacity;

        private double pMTECFactoryDefaultTemp;
        private int pMTECTimeConstantDefaultBootRaw;
        private double pMTECTimeConstantDefaultBoot;
        private int pMTECTimeConstantRealtimeRaw;
        private double pMTECTimeConstantRealtime;
        private int pMTECKpRealtimeRaw;
        private double pMTECKpRealtime;
        private int pMTECKpDefaultBootRaw;
        private double pMTECKpDefaultBoot;
        private int pMTECKiRealtimeRaw;
        private double pMTECKiRealtime;
        private double pMTECKdRealtime;
        private int pMTECKdRealtimeRaw;
        private double pMTECKdDefaultBoot;
        private int pMTECKdDefaultBootRaw;
        private int pMTECKiDefaultBootRaw;
        private double pMTECKiDefaultBoot;
        private int pMTECCoolingPIDRaw; 
        private double pMTECCoolingPID;
        private int pMTECHeatingPIDRaw;
        private double pMTECHeatingPID;
        private int pMDirectionCorrectRaw;
        private double pMDirectionCorrect;
        private int pMTECErrorSizeRaw;
        private double pMTECErrorSize;
        private int pMSpeedChangeValueRaw;
        private double pMSpeedChangeValue;

        private double existingPMTECTempSetpointDefault;
        private double existingPMTECTimeConstant;
        private double existingPMTECKpFactorDefault;
        private double existingPMTECKdFactorDefault;
        private double existingPMTECKiFactorDefault;

        private double manufacturerPMTECTempSetpointDefault;
        private double manufacturerPMTECTimeConstant;
        private double manufacturerPMTECKpFactorDefault;
        private double manufacturerPMTECKdFactorDefault;
        private double manufacturerPMTECKiFactorDefault;


        public UInt32 ModulationFrequencyRegVal

        {
            get { return modulationFrequencyRegVal; }
            set { modulationFrequencyRegVal = value; }
        }

        public UInt32 DutyCycleRegVal

        {
            get { return dutyCycleRegVal; }
            set { dutyCycleRegVal = value; }
        }

        public bool AdminLogginBut
        {
            get { return adminLogginBut; }
            set { adminLogginBut = value; }
        }
        public bool NewDiagnosticFileBut
        {
            get { return newDiagnosticFileBut; }
            set { newDiagnosticFileBut = value; }
        }

        public double ManufacturerDefModbusID
        {
            get { return manufacturerDefModbusID; }
            set { manufacturerDefModbusID = value; }
        }
        public string ManufacturerDefIP
        {
            get { return manufacturerDefIP; }
            set { manufacturerDefIP = value; }
        }
        public double ManufacturerDefPort
        {
            get { return manufacturerDefPort; }
            set { manufacturerDefPort = value; }
        }

        public double ManufacturerSTECTempSetpointDefaultBoot
        {
            get { return manufacturerSTECTempSetpointDefaultBoot; }
            set { manufacturerSTECTempSetpointDefaultBoot = value; }
        }

        public double ManufacturerOSEBodyTECTimeConstant
        {
            get { return manufacturerOSEBodyTECTimeConstant; }
            set { manufacturerOSEBodyTECTimeConstant = value; }
        }

        public double ManufacturerOSEBodyTECKpFactorDefault
        {
            get { return manufacturerOSEBodyTECKpFactorDefault; }
            set { manufacturerOSEBodyTECKpFactorDefault = value; }
        }

        public double ManufacturerOSEBodyTECKdFactorDefault
        {
            get { return manufacturerOSEBodyTECKdFactorDefault; }
            set { manufacturerOSEBodyTECKdFactorDefault = value; }
        }

        public double ManufacturerOSEBodyTECKiFactorDefault
        {
            get { return manufacturerOSEBodyTECKiFactorDefault; }
            set { manufacturerOSEBodyTECKiFactorDefault = value; }
        }


        public double ExistingSTECTempSetpointDefaultBoot
        {
            get { return existingSTECTempSetpointDefaultBoot; }
            set { existingSTECTempSetpointDefaultBoot = value; }
        }

        public double ExistingOSEBodyTECTimeConstant
        {
            get { return existingOSEBodyTECTimeConstant; }
            set { existingOSEBodyTECTimeConstant = value; }
        }

        public double ExistingOSEBodyTECKpFactorDefault
        {
            get { return  existingOSEBodyTECKpFactorDefault;  }
            set { existingOSEBodyTECKpFactorDefault = value; }
        }

        public double ExistingOSEBodyTECKdFactorDefault
        {
            get { return existingOSEBodyTECKdFactorDefault; }
            set { existingOSEBodyTECKdFactorDefault = value; }
        }

        public double ExistingOSEBodyTECKiFactorDefault
        {
            get { return existingOSEBodyTECKiFactorDefault; }
            set { existingOSEBodyTECKiFactorDefault = value; }
        }


        public double ManufacturerDefManCurr1
        {
            get { return manufacturerDefManCurr1; }
            set { manufacturerDefManCurr1 = value; }
        }
        public double ManufacturerDefManCurr2
        {
            get { return manufacturerDefManCurr2; }
            set { manufacturerDefManCurr2 = value; }
        }
        public double ManufacturerDefManCurr3
        {
            get { return manufacturerDefManCurr3; }
            set { manufacturerDefManCurr3 = value; }
        }
        public double ManufacturerDefManCurr4
        {
            get { return manufacturerDefManCurr4; }
            set { manufacturerDefManCurr4 = value; }
        }
        public double ManufacturerDefManCurr5
        {
            get { return manufacturerDefManCurr5; }
            set { manufacturerDefManCurr5 = value; }
        }
        public double ManufacturerDefManCurr6
        {
            get { return manufacturerDefManCurr6; }
            set { manufacturerDefManCurr6 = value; }
        }
        public double ManufacturerDefPcCurr1
        {
            get { return manufacturerDefPcCurr1; }
            set { manufacturerDefPcCurr1 = value; }
        }
        public double ManufacturerDefPcCurr2
        {
            get { return manufacturerDefPcCurr2; }
            set { manufacturerDefPcCurr2 = value; }
        }
        public double ManufacturerDefPcCurr3
        {
            get { return manufacturerDefPcCurr3; }
            set { manufacturerDefPcCurr3 = value; }
        }
        public double ManufacturerDefPcCurr4
        {
            get { return manufacturerDefPcCurr4; }
            set { manufacturerDefPcCurr4 = value; }
        }
        public double ManufacturerDefPcCurr5
        {
            get { return manufacturerDefPcCurr5; }
            set { manufacturerDefPcCurr5 = value; }
        }
        public double ManufacturerDefPcCurr6
        {
            get { return manufacturerDefPcCurr6; }
            set { manufacturerDefPcCurr6 = value; }
        }

        public double OSEBodySpeedChangeValue
        {
            get { return oSEBodySpeedChangeValue; }
            set { oSEBodySpeedChangeValue = value; }
        }

        public int OSEBodySpeedChangeValueRaw
        {
            get { return oSEBodySpeedChangeValueRaw; }
            set { oSEBodySpeedChangeValueRaw = value; }
        }
        public int OSEBodyTECErrorSizeRaw
        {
            get { return oSEBodyTECErrorSizeRaw; }
            set { oSEBodyTECErrorSizeRaw = value; }
        }
        public double OSEBodyTECErrorSize
        {
            get { return oSEBodyTECErrorSize; }
            set { oSEBodyTECErrorSize = value; }
        }
        public int OSEDirectionCorrectRaw
        {
            get { return oSEDirectionCorrectRaw; }
            set { oSEDirectionCorrectRaw = value; }
        }
        public double OSEDirectionCorrect
        {
            get { return oSEDirectionCorrect; }
            set { oSEDirectionCorrect = value; }
        }

        public int OSEBodyTECHeatingPIDRaw
        {
            get { return oSEBodyTECHeatingPIDRaw; }
            set { oSEBodyTECHeatingPIDRaw = value; }
        }
        public double OSEBodyTECHeatingPID
        {
            get { return oSEBodyTECHeatingPID; }
            set { oSEBodyTECHeatingPID = value; }
        }

        public int OSEBodyTECCoolingPIDRaw
        {
            get { return oSEBodyTECCoolingPIDRaw; }
            set { oSEBodyTECCoolingPIDRaw = value; }
        }
        public double OSEBodyTECCoolingPID
        {
            get { return oSEBodyTECCoolingPID; }
            set { oSEBodyTECCoolingPID = value; }
        }

        public double OSEBodyTECKdDefaultBoot
        {
            get { return oSEBodyTECKdDefaultBoot; }
            set { oSEBodyTECKdDefaultBoot = value; }
        }
        public double OSEBodyTECKiDefaultBoot
        {
            get { return oSEBodyTECKiDefaultBoot; }
            set { oSEBodyTECKiDefaultBoot = value; }
        }

        public int OSEBodyTECKiDefaultBootRaw
        {
            get { return oSEBodyTECKiDefaultBootRaw; }
            set { oSEBodyTECKiDefaultBootRaw = value; }
        }

        public int OSEBodyTECKdDefaultBootRaw
        {
            get { return oSEBodyTECKdDefaultBootRaw; }
            set { oSEBodyTECKdDefaultBootRaw = value; }
        }

        public double OSEBodyTECKdRealtime
        {
            get { return oSEBodyTECKdRealtime; }
            set { oSEBodyTECKdRealtime = value; }
        }
        public int OSEBodyTECKdRealtimeRaw
        {
            get { return oSEBodyTECKdRealtimeRaw; }
            set { oSEBodyTECKdRealtimeRaw = value; }
        }
        public int OSEBodyTECTimeConstantDefaultBootRaw
        {
            get { return oSEBodyTECTimeConstantDefaultBootRaw; }
            set { oSEBodyTECTimeConstantDefaultBootRaw = value; }
        }

        public double OSEBodyTECTimeConstantDefaultBoot
        {
            get { return oSEBodyTECTimeConstantDefaultBoot; }
            set { oSEBodyTECTimeConstantDefaultBoot = value; }
        }

        public int OSEBodyTECTimeConstantRaw
        {
            get { return oSEBodyTECTimeConstantRaw; }
            set { oSEBodyTECTimeConstantRaw = value; }
        }
        public double OSEBodyTECTimeConstant
        {
            get { return oSEBodyTECTimeConstant; }
            set { oSEBodyTECTimeConstant = value; }
        }
        public int OSEBodyTECKpRealtimeRaw
        {
            get { return oSEBodyTECKpRealtimeRaw; }
            set { oSEBodyTECKpRealtimeRaw = value; }
        }
        public double OSEBodyTECKpRealtime
        {
            get { return oSEBodyTECKpRealtime; }
            set { oSEBodyTECKpRealtime = value; }
        }
        public int OSEBodyTECKpDefaultBootRaw
        {
            get { return oSEBodyTECKpDefaultBootRaw; }
            set { oSEBodyTECKpDefaultBootRaw = value; }
        }
        public double OSEBodyTECKpDefaultBoot
        {
            get { return oSEBodyTECKpDefaultBoot; }
            set { oSEBodyTECKpDefaultBoot = value; }
        }
        public int OSEBodyTECKiRealtimeRaw
        {
            get { return oSEBodyTECKiRealtimeRaw; }
            set { oSEBodyTECKiRealtimeRaw = value; }
        }
        public double OSEBodyTECKiRealtime
        {
            get { return oSEBodyTECKiRealtime; }
            set { oSEBodyTECKiRealtime = value; }
        }

        public double HeatSinkTemp
        {
            get { return heatSinkTemp; }
            set { heatSinkTemp = value; }
        }
        public double SledTECTemp
        {
            get { return sledTECTemp; }
            set { sledTECTemp = value; }
        }

        public bool EditingSledTECTemp
        {
            get { return editingSledTECTemp; }
            set { editingSledTECTemp = value; }
        }
        public int CurrentMode
        {
            get { return currentMode; }
            set { currentMode = value; }
        }
        public double HeatSinkTempRaw
        {
            get { return heatSinkTempRaw; }
            set { heatSinkTempRaw = value; }
        }
        public double ExistingManCurr1
        {
            get { return existingManCurr1; }
            set { existingManCurr1 = value; }
        }
        public double ExistingManCurr2
        {
            get { return existingManCurr2; }
            set { existingManCurr2 = value; }
        }
        public double ExistingManCurr3
        {
            get { return existingManCurr3; }
            set { existingManCurr3 = value; }
        }
        public double ExistingManCurr4
        {
            get { return existingManCurr4; }
            set { existingManCurr4 = value; }
        }
        public double ExistingManCurr5
        {
            get { return existingManCurr5; }
            set { existingManCurr5 = value; }
        }
        public double ExistingManCurr6
        {
            get { return existingManCurr6; }
            set { existingManCurr6 = value; }
        }
        public double ExistingPcCurr1
        {
            get { return existingPcCurr1; }
            set { existingPcCurr1 = value; }
        }
        public double ExistingPcCurr2
        {
            get { return existingPcCurr2; }
            set { existingPcCurr2 = value; }
        }
        public double ExistingPcCurr3
        {
            get { return existingPcCurr3; }
            set { existingPcCurr3 = value; }
        }
        public double ExistingPcCurr4
        {
            get { return existingPcCurr4; }
            set { existingPcCurr4 = value; }
        }
        public double ExistingPcCurr5
        {
            get { return existingPcCurr5; }
            set { existingPcCurr5 = value; }
        }
        public double ExistingPcCurr6
        {
            get { return existingPcCurr6; }
            set { existingPcCurr6 = value; }
        }

        public double ExistingPMTECTemperatureSetpointDefaultBoot
        {
            get { return existingPMTECTemperatureSetpointDefaultBoot; }
            set { existingPMTECTemperatureSetpointDefaultBoot = value; }
        }

        public int ExistingModbusID
        {
            get { return existingModbusID; }
            set { existingModbusID = value; }
        }


        public string ExistingIP
        {
            get { return existingIP; }
            set { existingIP = value; }
        }

        public int ExistingPort
        {
            get { return existingPort; }
            set { existingPort = value; }
        }
        public double SledTECFactoryDefaultTemp
        {
            get { return sledTECFactoryDefaultTemp; }
            set { sledTECFactoryDefaultTemp = value; }
        }
        public double FanSpeedReadRaw
        {
            get { return fanSpeedReadRaw; }
            set { fanSpeedReadRaw = value; }
        }
        public double FanSpeed
        {
            get { return fanSpeed; }
            set { fanSpeed = value; }

        }
        public double SledTECCapacityDefaultRaw
        {
            get { return sledTECCapacityDefaultRaw; }
            set { sledTECCapacityDefaultRaw = value; }
        }
        public double SledTECCapacityDefault
        {
            get { return sledTECCapacityDefault; }
            set { sledTECCapacityDefault = value; }
        }

        public double SledTECCapacityRaw
        {
            get { return sledTECCapacityRaw; }
            set { sledTECCapacityRaw = value; }
        }
        public double SledTECCapacity
        {
            get { return sledTECCapacity; }
            set { sledTECCapacity = value; }

        }
        public int SledTECCurrentRawLogString
        {
            get { return sledTECCurrentRawLogString; }
            set { sledTECCurrentRawLogString = value; }
        }
        public double SledTECCurrRead
        {
            get { return sledTECCurrRead; }
            set { sledTECCurrRead = value; }
        }
        public int SledTECHeatOrCool
        {
            get { return sledTECHeatOrCool; }
            set { sledTECHeatOrCool = value; }
        }


        public double SledTECCurr
        {
            get { return sledTECCurr; }
            set { sledTECCurr = value; }
        }
        public double SledTECCurrRaw
        {
            get { return sledTECCurrRaw; }
            set { sledTECCurrRaw = value; }
        }

        public int PosOrNegHeatSink
        {
            get { return posOrNegHeatSink; }
            set { posOrNegHeatSink = value; }

        }
        public double OSEBodyTemp
        {
            get { return oSEBodyTemp; }
            set { oSEBodyTemp = value; }
        }
        public double OSEBodyTempRaw
        {
            get { return oSEBodyTempRaw; }
            set { oSEBodyTempRaw = value; }
        }
        public double OSEBodyCurr
        {
            get { return oSEBodyCurr; }
            set { oSEBodyCurr = value; }
        }
        public double OSEBodyCurrRaw
        {
            get { return oSEBodyCurrRaw; }
            set { oSEBodyCurrRaw = value; }
        }
        public double OSEHeatOrCool
        {
            get { return oSEHeatOrCool; }
            set { oSEHeatOrCool = value; }
        }

        public double HeatSinkSetpointRead
        {
            get { return heatSinkSetpointRead; }
            set { heatSinkSetpointRead = value; }
        }
        public double HeatSinkSetpointReadRaw
        {
            get { return heatSinkSetpointReadRaw; }
            set { heatSinkSetpointReadRaw = value; }
        }

        public double SledTECSTempSetpointDefaultRealtimeRaw
        {
            get { return sledTECSTempSetpointDefaultRealtimeRaw; }
            set { sledTECSTempSetpointDefaultRealtimeRaw = value; }
        }
        public double SledTECSTempSetpointDefaultRealtime
        {
            get { return sledTECSTempSetpointDefaultRealtime; }
            set { sledTECSTempSetpointDefaultRealtime = value; }
        }

        public double SledTECSTempSetpointDefaultBootRaw
        {
            get { return sledTECSTempSetpointDefaultBootRaw; }
            set { sledTECSTempSetpointDefaultBootRaw = value; }
        }
        public double SledTECSTempSetpointDefaultBoot
        {
            get { return sledTECSTempSetpointDefaultBoot; }
            set { sledTECSTempSetpointDefaultBoot = value; }
        }

        public double OSEBodyCapacityRaw
        {
            get { return oSEBodyCapacityRaw; }
            set { oSEBodyCapacityRaw = value; }
        }
        public double OSEBodyCapacity
        {
            get { return oSEBodyCapacity; }
            set { oSEBodyCapacity = value; }
        }

        public int PosOrNegS6Submount
        {
            get { return posOrNegS6Submount; }
            set { posOrNegS6Submount = value; }
        }
        public double BoardTemperatureN
        {
            get { return boardTemperatureN; }
            set { boardTemperatureN = value; }
        }
        public double BoardTemperatureRaw
        {
            get { return boardTemperatureRaw; }
            set { boardTemperatureRaw = value; }
        }
        public double MonDiode1ReadVal
        {
            get { return monDiode1ReadVal; }
            set { monDiode1ReadVal = value; }
        }
      
      
        public int MonDiode1RawLogString
        {
            get { return monDiode1RawLogString; }
            set { monDiode1RawLogString = value; }
        }
      

        public double MonDiode1ReadRaw
        {
            get { return monDiode1ReadRaw; }
            set { monDiode1ReadRaw = value; }
        }
        
        public double ActualCurr1ReadVal
        {
            get { return actualCurr1ReadVal; }
            set { actualCurr1ReadVal = value; }
        }
       
        public double Sled1CurrSenseRaw
        {
            get { return sled1CurrSenseRaw; }
            set { sled1CurrSenseRaw = value; }
        }
       
        public double SledTECTempSetpointDefaultRaw
        {
            get { return sledTECTempSetpointDefaultRaw; }
            set { sledTECTempSetpointDefaultRaw = value; }
        }
        public double SledTECTempSetpointDefault
        {
            get { return sledTECTempSetpointDefault; }
            set { sledTECTempSetpointDefault = value; }
        }
        public double SledTECTempSetpointRaw
        {
            get { return sledTECTempSetpointRaw; }
            set { sledTECTempSetpointRaw = value; }
        }
        public double SledTECTempSetpoint
        {
            get { return sledTECTempSetpoint; }
            set { sledTECTempSetpoint = value; }
        }

        public int SLEDTECCurrSetDefaultRaw
        {
            get { return sledTECCurrSetDefaultRaw; }
            set { sledTECCurrSetDefaultRaw = value; }
        }
        public double SLEDTECCurrSetDefault
        {
            get { return sledTECCurrSetDefault; }
            set { sledTECCurrSetDefault = value; }
        }
        public int SLEDTECCurrSetRaw
        {
            get { return sledTECCurrSetRaw; }
            set { sledTECCurrSetRaw = value; }
        }
        public double SLEDTECCurrSet
        {
            get { return sledTECCurrSet; }
            set { sledTECCurrSet = value; }
        }
        public int SLEDTECCurrSetFactoryDefaultRaw
        {
            get { return sledTECCurrSetFactoryDefaultRaw; }
            set { sledTECCurrSetFactoryDefaultRaw = value; }
        }
        public double SLEDTECCurrSetFactoryDefault
        {
            get { return sledTECCurrSetFactoryDefault; }
            set { sledTECCurrSetFactoryDefault = value; }
        }


        public double Sled1CurrentSetpoint
        {
            get { return sled1CurrentSetpoint; }
            set { sled1CurrentSetpoint = value; }
        }
        public double Sled1CurrentSetpointRaw
        {
            get { return sled1CurrentSetpointRaw; }
            set { sled1CurrentSetpointRaw = value; }
        }
        public double SledTECTempRaw
        {
            get { return sledTECTempRaw; }
            set { sledTECTempRaw = value; }
        }
        public double Sled2CurrentSetpoint
        {
            get { return sled2CurrentSetpoint; }
            set { sled2CurrentSetpoint = value; }
        }
        public double Sled2CurrentSetpointRaw
        {
            get { return sled2CurrentSetpointRaw; }
            set { sled2CurrentSetpointRaw = value; }
        }

        public double Sled3CurrentSetpoint
        {
            get { return sled3CurrentSetpoint; }
            set { sled3CurrentSetpoint = value; }
        }
        public double Sled3CurrentSetpointRaw
        {
            get { return sled3CurrentSetpointRaw; }
            set { sled3CurrentSetpointRaw = value; }
        }

        public double Sled4CurrentSetpoint
        {
            get { return sled4CurrentSetpoint; }
            set { sled4CurrentSetpoint = value; }
        }
        public double Sled4CurrentSetpointRaw
        {
            get { return sled4CurrentSetpointRaw; }
            set { sled4CurrentSetpointRaw = value; }
        }

        public double Sled5CurrentSetpoint
        {
            get { return sled5CurrentSetpoint; }
            set { sled5CurrentSetpoint = value; }
        }
        public double Sled5CurrentSetpointRaw
        {
            get { return sled5CurrentSetpointRaw; }
            set { sled5CurrentSetpointRaw = value; }
        }

         public double Sled6CurrentSetpoint
        {
            get { return sled6CurrentSetpoint; }
            set { sled6CurrentSetpoint = value; }
        }
        public double Sled6CurrentSetpointRaw
        {
            get { return sled6CurrentSetpointRaw; }
            set { sled6CurrentSetpointRaw = value; }
        }

        public int SlaveID
        {
            get { return slaveID; }
            set { slaveID = value; }
        }

        public int ModbusID
        {
            get { return modbusID; }
            set { modbusID = value; }
        }

        public int SledsAreOn
        {
            get { return sledsAreOn; }
            set { sledsAreOn = value; }
        }

        public int STECOn
        {
            get { return sTECOn; }
            set { sTECOn = value; }
        }

        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public List<string> CurrentLogStrings
        {
            get { return currentLogStrings; }
            set { currentLogStrings = value; }
        }

        public List<string> CurrentPublicLogStrings
        {
            get { return currentPublicLogStrings; }
            set { currentPublicLogStrings = value; }
        }
        public int PublicLogfileLineCount
        {
            get { return publicLogfileLineCount; }
            set { publicLogfileLineCount = value; }
        }
        public int AdminLogfileLineCount
        {
            get { return adminLogfileLineCount; }
            set { adminLogfileLineCount = value; }
        }
        public int CurrentRightIndex
        {
            get { return currentRightIndex; }
            set { currentRightIndex = value; }
        }
        public int CurrentLeftIndex
        {
            get { return currentLeftIndex; }
            set { currentLeftIndex = value; }
        }

        public int Slider6TrackBar
        {
            get { return slider6TrackBar; }
            set { slider6TrackBar = value; }
        }

        public int WhichCycle
        {
            get { return whichCycle; }
            set { whichCycle = value; }
        }
        public double FirmwareVersion
        {
            get { return firmwareVersion; }
            set { firmwareVersion = value; }
        }
       
        public double Model
        {
            get { return model; }
            set { model = value; }
        }
        public string SlaveAdminLogFilePath
        {
            get { return slaveAdminLogFilePath; }
            set { slaveAdminLogFilePath = value; }
        }

        public string SlavePublicLogFilePath
        {
            get { return slavePublicLogFilePath; }
            set { slavePublicLogFilePath = value; }
        }

        public int[] Capabilities
        {
            get { return capabilities; }
            set { capabilities = value; }
        }

        public bool ContinuousFirstRead
        {
            get { return continuousFirstRead; }
            set { continuousFirstRead = value; }
        }

        public int ChartMode
        {
            get { return chartMode; }
            set { chartMode = value; }
        }
        public int PMTECTempSetpointDefaultRaw
        {
            get { return pMTECTempSetpointDefaultRaw; }
            set { pMTECTempSetpointDefaultRaw = value; }
        }
        public double PMTECTempSetpointDefault
        {
            get { return pMTECTempSetpointDefault; }
            set { pMTECTempSetpointDefault = value; }
        }
        public int PMTECTempSetpointRaw
        {
            get { return pMTECTempSetpointRaw; }
            set { pMTECTempSetpointRaw = value; }
        }
        public double PMTECTempSetpoint
        {
            get { return pMTECTempSetpoint; }
            set { pMTECTempSetpoint = value; }
        }

        public int PMTECTempRaw
        {
            get { return pMTECTempRaw; }
            set { pMTECTempRaw = value; }
        }
        public double PMTECTemp
        {
            get { return pMTECTemp; }
            set { pMTECTemp = value; }
        }

        public double PMTECFactoryDefaultTemp
        {
            get { return pMTECFactoryDefaultTemp; }
            set { pMTECFactoryDefaultTemp = value; }
        }

        public double PMTECKdDefaultBoot
        {
            get { return pMTECKdDefaultBoot; }
            set { pMTECKdDefaultBoot = value; }
        }
        public double PMTECKiDefaultBoot
        {
            get { return pMTECKiDefaultBoot; }
            set { pMTECKiDefaultBoot = value; }
        }

        public int PMTECKiDefaultBootRaw
        {
            get { return pMTECKiDefaultBootRaw; }
            set { pMTECKiDefaultBootRaw = value; }
        }

        public int PMTECKdDefaultBootRaw
        {
            get { return pMTECKdDefaultBootRaw; }
            set { pMTECKdDefaultBootRaw = value; }
        }

        public double PMTECKdRealtime
        {
            get { return pMTECKdRealtime; }
            set { pMTECKdRealtime = value; }
        }
        public int PMTECKdRealtimeRaw
        {
            get { return pMTECKdRealtimeRaw; }
            set { pMTECKdRealtimeRaw = value; }
        }
        public int PMTECTimeConstantRealtimeRaw
        {
            get { return pMTECTimeConstantRealtimeRaw; }
            set { pMTECTimeConstantRealtimeRaw = value; }
        }
        public double PMTECTimeConstantRealtime
        {
            get { return pMTECTimeConstantRealtime; }
            set { pMTECTimeConstantRealtime = value; }
        }

        public int PMTECTimeConstantDefaultBootRaw
        {
            get { return pMTECTimeConstantDefaultBootRaw; }
            set { pMTECTimeConstantDefaultBootRaw = value; }
        }

        public double PMTECTimeConstantDefaultBoot
        {
            get { return pMTECTimeConstantDefaultBoot; }
            set { pMTECTimeConstantDefaultBoot = value; }
        }


        public int PMTECKpRealtimeRaw
        {
            get { return pMTECKpRealtimeRaw; }
            set { pMTECKpRealtimeRaw = value; }
        }
        public double PMTECKpRealtime
        {
            get { return pMTECKpRealtime; }
            set { pMTECKpRealtime = value; }
        }
        public int PMTECKpDefaultBootRaw
        {
            get { return pMTECKpDefaultBootRaw; }
            set { pMTECKpDefaultBootRaw = value; }
        }
        public double PMTECKpDefaultBoot
        {
            get { return pMTECKpDefaultBoot; }
            set { pMTECKpDefaultBoot = value; }
        }
        public int PMTECKiRealtimeRaw
        {
            get { return pMTECKiRealtimeRaw; }
            set { pMTECKiRealtimeRaw = value; }
        }
        public double PMTECKiRealtime
        {
            get { return pMTECKiRealtime; }
            set { pMTECKiRealtime = value; }
        }

        public double PMSpeedChangeValue
        {
            get { return pMSpeedChangeValue; }
            set { pMSpeedChangeValue = value; }
        }

        public int PMSpeedChangeValueRaw
        {
            get { return pMSpeedChangeValueRaw; }
            set { pMSpeedChangeValueRaw = value; }
        }
        public int PMTECErrorSizeRaw
        {
            get { return pMTECErrorSizeRaw; }
            set { pMTECErrorSizeRaw = value; }
        }
        public double PMTECErrorSize
        {
            get { return pMTECErrorSize; }
            set { pMTECErrorSize = value; }
        }
        public int PMDirectionCorrectRaw
        {
            get { return pMDirectionCorrectRaw; }
            set { pMDirectionCorrectRaw = value; }
        }
        public double PMDirectionCorrect
        {
            get { return pMDirectionCorrect; }
            set { pMDirectionCorrect = value; }
        }

        public int PMTECHeatingPIDRaw
        {
            get { return pMTECHeatingPIDRaw; }
            set { pMTECHeatingPIDRaw = value; }
        }
        public double PMTECHeatingPID
        {
            get { return pMTECHeatingPID; }
            set { pMTECHeatingPID = value; }
        }

        public int PMTECCoolingPIDRaw
        {
            get { return pMTECCoolingPIDRaw; }
            set { pMTECCoolingPIDRaw = value; }
        }
        public double PMTECCoolingPID
        {
            get { return pMTECCoolingPID; }
            set { pMTECCoolingPID = value; }
        }

        public double ManufacturerPMTECTempSetpointDefault
        {
            get { return manufacturerPMTECTempSetpointDefault; }
            set { manufacturerPMTECTempSetpointDefault = value; }
        }
        public double ManufacturerPMTECTimeConstant
        {
            get { return manufacturerPMTECTimeConstant; }
            set { manufacturerPMTECTimeConstant = value; }
        }

        public double ManufacturerPMTECKpFactorDefault
        {
            get { return manufacturerPMTECKpFactorDefault; }
            set { manufacturerPMTECKpFactorDefault = value; }
        }

        public double ManufacturerPMTECKdFactorDefault
        {
            get { return manufacturerPMTECKdFactorDefault; }
            set { manufacturerPMTECKdFactorDefault = value; }
        }

        public double ManufacturerPMTECKiFactorDefault
        {
            get { return manufacturerPMTECKiFactorDefault; }
            set { manufacturerPMTECKiFactorDefault = value; }
        }


        public double ExistingPMTECTempSetpointDefault
        {
            get { return existingPMTECTempSetpointDefault; }
            set { existingPMTECTempSetpointDefault = value; }
        }

        public double ExistingPMTECTimeConstant
        {
            get { return existingPMTECTimeConstant; }
            set { existingPMTECTimeConstant = value; }
        }

        public double ExistingPMTECKpFactorDefault
        {
            get { return existingPMTECKpFactorDefault; }
            set { existingPMTECKpFactorDefault = value; }
        }

        public double ExistingPMTECKdFactorDefault
        {
            get { return existingPMTECKdFactorDefault; }
            set { existingPMTECKdFactorDefault = value; }
        }

        public double ExistingPMTECKiFactorDefault
        {
            get { return existingPMTECKiFactorDefault; }
            set { existingPMTECKiFactorDefault = value; }
        }

        public double PMTECCapacityRaw
        {
            get { return pMTECCapacityRaw; }
            set { pMTECCapacityRaw = value; }
        }
        public double PMTECCapacity
        {
            get { return pMTECCapacity; }
            set { pMTECCapacity = value; }

        }
        public int PMTECCurrentRawLogString
        {
            get { return pMTECCurrentRawLogString; }
            set { pMTECCurrentRawLogString = value; }
        }
        public int PMTECHeatOrCool
        {
            get { return pMTECHeatOrCool; }
            set { pMTECHeatOrCool = value; }
        }

        public double PMTECCurr
        {
            get { return pMTECCurr; }
            set { pMTECCurr = value; }
        }
        public double PMTECCurrRaw
        {
            get { return pMTECCurrRaw; }
            set { pMTECCurrRaw = value; }
        }

        public int PMReadRaw
        {
            get { return pMReadRaw; }
            set { pMReadRaw = value; }
        }
        public double PMRead
        {
            get { return pMRead; }
            set { pMRead = value; }
        }

        public double PMWavelength
        {
            get { return pMWavelength; }
            set { pMWavelength = value; }
        }
        public int PMPowerRaw
        {
            get { return pMPowerRaw; }
            set { pMPowerRaw = value; }
        }
        public double PMPower
        {
            get { return pMPower; }
            set { pMPower = value; }
        }
    }


}
