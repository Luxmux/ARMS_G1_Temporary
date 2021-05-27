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
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Photosensitivity : Window
    {
        public MainWindow mainWindow;
        ModBusSlave mySlave;

        public Photosensitivity(MainWindow _mainWindow)
        {
            InitializeComponent();
            this.mainWindow = _mainWindow;
            mySlave = MainWindow.modbusSlaveList[mainWindow.selectedSlaveID];
            

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
