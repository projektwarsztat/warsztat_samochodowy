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

namespace WarsztatV2_klient
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputPin.Text == "1234")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close();

            }

            else
            {

                MessageBox.Show("Błędny PIN", "Błąd !", MessageBoxButton.OK, MessageBoxImage.Error);
                InputPin.Clear();
            }
        }
    }
}
