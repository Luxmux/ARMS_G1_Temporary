using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class Modulation : Window
	{
		public MainWindow mainWindow;
		public ModBusSlave mySlave;
		public int ModEnabled = 0;
		UInt16 modFreqHighBytes = 0;
		UInt16 modFreqLowBytes = 0;
		UInt16 dutyCycleHighBytes = 0;
		UInt16 dutyCycleLowBytes = 0;

		public Modulation(MainWindow _mainWindow)
		{
			InitializeComponent();
			this.mainWindow = _mainWindow;
			mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
			mainWindow.UpdateModulation(mySlave, this);
			//if (ModEnabled == 0)
			//{
			//	if (mySlave.SledsAreOn == 0)
			//	{
			//		MessageBox.Show("Cannot control modulation when Sleds are OFF.");
			//	}

			//	else //Itziar Revisar
			//	{
			//		//Turn modulation on
			//		mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,200, 1);
			//		DateTime CurrentTime = DateTime.Now;
			//		while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] != 1 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
			//		if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] == 1)
			//		{
			//			ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
			//			EnableBut.Content = "      On";
			//			ModEnabled = 1;
			//		}
			//	}
			//}
			//else
			//{
			//	mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,200, 0);
			//	DateTime CurrentTime = DateTime.Now;
			//	while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] != 0 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
			//	if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] == 0)
			//	{
			//		mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,201, 0);
			//		CurrentTime = DateTime.Now;
			//		while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,201, 1)[0] != 0 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
			//		if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,201, 1)[0] == 0)
			//		{
			//			ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
			//			EnableBut.Content = "     Off";
			//			ModEnabled = 0;
			//		}
			//	}
			//}
			//mainWindow.UpdateModulation(mySlave, this);
		}

		private void Modulation_Click(object sender, RoutedEventArgs e)
		{
			//Get status of modulation registers and display
			ushort[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 2);

			UInt16 whichSledsRegVal;
			UInt16 modulationEnabled;

			modulationEnabled = readData[0];
			whichSledsRegVal = readData[1];

			//Use bitwise mask to find which sleds are modulating
			UInt16 sled1ModOn = (UInt16)(whichSledsRegVal & 1);
			UInt16 sled2ModOn = (UInt16)(whichSledsRegVal & 2);
			UInt16 sled3ModOn = (UInt16)(whichSledsRegVal & 4);
			UInt16 sled4ModOn = (UInt16)(whichSledsRegVal & 8);
			UInt16 sled5ModOn = (UInt16)(whichSledsRegVal & 16);
			UInt16 sled6ModOn = (UInt16)(whichSledsRegVal & 32);

			UInt16 ValToWrite = 0;

			if ((sender as Button).Name == "EnableBut")
			{

				if (ModEnabled == 0)
				{
					if (mySlave.SledsAreOn == 0)
					{
						MessageBox.Show("Cannot control modulation when SLED is OFF.");
					}

					else
					{
						//Turn modulation on
						mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,200, 1);
						DateTime CurrentTime = DateTime.Now;
						while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] != 1 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
						if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] == 1)
						{
							ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34));
							EnableBut.Content = "      On";
							ModEnabled = 1;
						}
					}
				}
				else
				{
					mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,200, 0);
					DateTime CurrentTime = DateTime.Now;
					while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] != 0 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
					if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,200, 1)[0] == 0)
					{
						mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,201, 0);
						CurrentTime = DateTime.Now;
						while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,201, 1)[0] != 0 && DateTime.Now < CurrentTime.AddSeconds(2)) { }
						if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,201, 1)[0] == 0)
						{
							ModulationModeIndicator.Fill = new SolidColorBrush(Color.FromRgb(205, 92, 92));
							EnableBut.Content = "     Off";
							ModEnabled = 0;
						}
					}
				}
				mainWindow.UpdateModulation(mySlave, this);
				}
			
			if ((sender as Button).Name == "Mod1")
			{
				if (mySlave.Capabilities[0] != 0)
				{
					if (sled1ModOn > 0)
					{
						ValToWrite = Convert.ToUInt16(whichSledsRegVal - 1);
					}
					else
					{
						ValToWrite = Convert.ToUInt16(whichSledsRegVal + 1);
					}
					mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,201, ValToWrite);
					DateTime CurrentTime = DateTime.Now;
					while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,201, 1)[0] != ValToWrite && DateTime.Now < CurrentTime.AddSeconds(2)) { }
							
				}
				else
				{
					DcOff1.Text = "N/A";
					Mod1.IsEnabled = false;
				}
				mainWindow.UpdateModulation(mySlave, this);
			}

		}


		private void modulationFreq_Click(object sender, RoutedEventArgs e)
		{
			if (mainWindow.ModFreqEdit_temp != null && mainWindow.DutyCycleEdit_temp != null)
			{
				if (mainWindow.ModFreqEdit_temp.Length > 0 && IsTextNumeric(mainWindow.ModFreqEdit_temp.Replace(" ", "")) == true && mainWindow.DutyCycleEdit_temp.Length > 0 && IsTextNumeric(mainWindow.DutyCycleEdit_temp.Replace(" ", "")) == true)
				{

					if (double.Parse(mainWindow.ModFreqEdit_temp) >= 0.016)
					{
						double regValToWriteDbl;
						if (double.Parse(mainWindow.ModFreqEdit_temp) >= 1000)
						{
							regValToWriteDbl = Math.Round((13.5 * Math.Pow(10, 6)) / 1000, 0);
						}
						else
						{
							//Get reg value
							regValToWriteDbl = Math.Round((13.5 * Math.Pow(10, 6)) / double.Parse(mainWindow.ModFreqEdit_temp), 0);
						}
						//Get high and low words
						modFreqHighBytes = (UInt16)((UInt32)regValToWriteDbl >> 16);
						modFreqLowBytes = (UInt16)((UInt32)regValToWriteDbl & 0xFFFF);

						//Attempt to write values in new thread
						mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 202, modFreqHighBytes);
						DateTime CurrentTime = DateTime.Now;
						while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 1)[0] != modFreqHighBytes && DateTime.Now < CurrentTime.AddSeconds(10)) { }
						if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 202, 1)[0] == modFreqHighBytes)
						{
							mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 203, modFreqLowBytes);
							CurrentTime = DateTime.Now;
							int temp = 0;
							temp = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 203, 1)[0];
							while (temp != modFreqLowBytes - 2 && DateTime.Now < CurrentTime.AddSeconds(10))
							{
								temp = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 203, 1)[0];
							}

							ushort[] readtemp = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 203, 1);

							if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 203, 1)[0] == modFreqLowBytes - 2)//check why the -2, if it's firmware or what. Itziar
							{
								double userInputDbl = double.Parse(mainWindow.DutyCycleEdit_temp);
								regValToWriteDbl = Math.Round(regValToWriteDbl * userInputDbl / 100.0, 0);

								if (userInputDbl >= 10 || userInputDbl <= 90)
								{
									//Get high and low words
									mainWindow.DutyCycleEdit_temp = regValToWriteDbl.ToString();
									dutyCycleHighBytes = (UInt16)(UInt32.Parse(mainWindow.DutyCycleEdit_temp) >> 16);
									dutyCycleLowBytes = (UInt16)(UInt32.Parse(mainWindow.DutyCycleEdit_temp) & 0xFFFF);

									//Attempt to write values in new thread
									mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 204, dutyCycleHighBytes);
									CurrentTime = DateTime.Now;
									while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 204, 1)[0] != dutyCycleHighBytes && DateTime.Now < CurrentTime.AddSeconds(10)) { }
									if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 204, 1)[0] == dutyCycleHighBytes)
									{
										mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 205, dutyCycleLowBytes);
										CurrentTime = DateTime.Now;
										while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 205, 1)[0] != dutyCycleLowBytes && DateTime.Now < CurrentTime.AddSeconds(10)) { }
										if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 205, 1)[0] == dutyCycleLowBytes)
										{
											Keyboard.Focus(ChangeModFreqBut);
											Keyboard.ClearFocus();
										}
										else
										{
											MessageBox.Show("Error: Fail to update Duty Cycle Value");
										}
									}
									else
									{
										MessageBox.Show("Error: Fail to update Duty Cycle Value");
									}
								}
								else
								{
									MessageBox.Show("Error: Invalid Duty Cycle Value");
								}
							}
							else
							{
								MessageBox.Show("Error: Fail to update Modulation Frequency");
							}
						}
						else
						{
							MessageBox.Show("Error: Fail to update Modulation Frequency");
						}

					}
					else
					{
						MessageBox.Show("Error: Invalid Frequency");
					}
				}
				else
				{
					MessageBox.Show("Error: Invalid Frequency Value");
				}
			}
		}

		private void dutyCycle_Click(object sender, RoutedEventArgs e)
		{
			if (mainWindow.DutyCycleEdit_temp != null)
			{
				if (mainWindow.DutyCycleEdit_temp.Length > 0 && IsTextNumeric(mainWindow.DutyCycleEdit_temp.Replace(" ", "")) == true)
				{
					double userInputDbl = double.Parse(mainWindow.DutyCycleEdit_temp);
					double regValToWriteDbl = Math.Round(mySlave.ModulationFrequencyRegVal * userInputDbl / 100.0, 0);

					if (userInputDbl >= 10 || userInputDbl <= 90)
					{
						//Get high and low words
						dutyCycleHighBytes = (UInt16)((UInt32)regValToWriteDbl >> 16);
						dutyCycleLowBytes = (UInt16)((UInt32)regValToWriteDbl & 0xFFFF);

						//Attempt to write values in new thread
						mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 204, dutyCycleHighBytes);
						DateTime CurrentTime = DateTime.Now;
						while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 204, 1)[0] != dutyCycleHighBytes && DateTime.Now < CurrentTime.AddSeconds(2)) { }
						if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 204, 1)[0] == dutyCycleHighBytes)
						{
							mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID, 205, dutyCycleLowBytes);
							CurrentTime = DateTime.Now;
							while (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 205, 1)[0] != dutyCycleLowBytes && DateTime.Now < CurrentTime.AddSeconds(2)) { }
							if (mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID, 205, 1)[0] == dutyCycleLowBytes)
							{
								Keyboard.Focus(ChangeDutyCycleBut);
								Keyboard.ClearFocus();
							}
							else
							{
								MessageBox.Show("Error: Fail to update Duty Cycle Value");
							}
						}
						else
						{
							MessageBox.Show("Error: Fail to update Duty Cycle Value");
						}
					}
					else
					{
						MessageBox.Show("Error: Invalid Duty Cycle Value");
					}
				}
				else
				{
					MessageBox.Show("Error: Invalid Duty Cycle Value");
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