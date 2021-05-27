using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public partial class Admin : Window
    {
        public MainWindow mainWindow;
        ModBusSlave mySlave;

        public Admin(MainWindow _mainWindow)
        {
            InitializeComponent();
            this.mainWindow = _mainWindow;
            mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
			
			//update UI
			this.Dispatcher.Invoke(() =>
            {
				
				BoardTempBox.Visibility = Visibility.Visible;
				BoardTempRaw.Visibility = Visibility.Visible;
				FunctionCode.Text = "";
			});
        }

        private void ManualPollChanged (object sender, RoutedEventArgs e)
        {

            ComboBoxItem new_function = FunctionCode.SelectedItem as ComboBoxItem;

            if (new_function != null)
            {
                if (new_function.Content.ToString() == "Read Holding Reg")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Addr.IsEnabled = true;
                        NumReg.IsEnabled = true;
                        Value.IsEnabled = false;
                        Value.Text = "";
                    });
                }
                else if (new_function.Content.ToString() == "Read Input Reg")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Addr.IsEnabled = true;
                        NumReg.IsEnabled = true;
                        Value.IsEnabled = false;
                        Value.Text = "";
                    });
                }
                else if (new_function.Content.ToString() == "Write Single Reg")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Addr.IsEnabled = true;
                        NumReg.IsEnabled = false;
                        Value.IsEnabled = true;
                        NumReg.Text = "";
                    });
                }
            }



        }

        private void ManualModbusSend_Click(object sender, RoutedEventArgs e)
        {
            string result = "";

            if (FunctionCode.SelectedIndex==0) //read holding register
            {
				try
				{
					UInt16[] readData = mainWindow.mbClient.ReadHoldingRegisters((byte)mySlave.ModbusID,int.Parse(Addr.Text), int.Parse(NumReg.Text));
					for (int x = 0; x < int.Parse(NumReg.Text); x++)
					{
						result += readData[x].ToString() + "\r\n";
					}
					ListBox1.Text = result;
				}
				catch
				{
					ListBox1.Text = "Error Reading Reg";
				}
			}
            else if (FunctionCode.SelectedIndex == 1) // read input register
            {
				try
				{
					UInt16[] readData = mainWindow.mbClient.ReadInputRegisters((byte)mySlave.ModbusID,int.Parse(Addr.Text), int.Parse(NumReg.Text));
					for (int x = 0; x < int.Parse(NumReg.Text); x++)
					{
						result += readData[x].ToString() + "\r\n";
					}
					ListBox1.Text = result;
				}
				catch
				{
					ListBox1.Text = "Error Reading Reg";
				}
			}
            else if (FunctionCode.SelectedIndex == 2) // rwite single register
            {
				try
				{
					mainWindow.mbClient.WriteSingleRegister((byte)mySlave.ModbusID,int.Parse(Addr.Text), UInt16.Parse(Value.Text));
					ListBox1.Text = "success";
				}
                catch
                {
					ListBox1.Text = "Error Writing Reg";
				}
            }
        }

		private void Show_Raw_Checked(object sender, RoutedEventArgs e) {
			mainWindow.showRawValues = true;
		}
		private void Show_Raw_Unchecked(object sender, RoutedEventArgs e) {
			mainWindow.showRawValues = false;
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