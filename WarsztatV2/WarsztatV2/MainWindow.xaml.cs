using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarsztatV2.Menu;
using WarsztatV2.Tables;

namespace WarsztatV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RightContent.Content = new Aktualnosci();
            // domyslnie podmenu zlecienie - ukryte
            HideSubmenu();

            // wypisanie danych aktualnie zalogowanego uzytkownika
            IdUser.Text = "Witaj ! \nJan Kowalski";
        }

        // metoda rozwijajaca podmenu zlecenia
        private void ShowSubmenu()
        {
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();

            else
            {
                NoweZlecenie.Visibility = Visibility.Visible;
                DoNaprawy.Visibility = Visibility.Visible;
                DoOdbioru.Visibility = Visibility.Visible;
                HistoriaZlecen.Visibility = Visibility.Visible;
            }
        }

        // metoda ukrywajaca podmenu zlecenia
        private void HideSubmenu()
        {
            NoweZlecenie.Visibility = Visibility.Collapsed;
            DoNaprawy.Visibility = Visibility.Collapsed;
            DoOdbioru.Visibility = Visibility.Collapsed;
            HistoriaZlecen.Visibility = Visibility.Collapsed;

        }

        //metoda rozwijajaca podmenu zlecenia
        private void ZleceniaClick(object sender, RoutedEventArgs e)
        {
            ShowSubmenu();
        }

        //------------ metody przelaczajace zawartosc prawej czesci aplikacji ------------//
        private void OFirmieClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new OFirmie();
        }

        private void KlienciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Klienci();
        }

        private void PracownicyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Pracownicy();
        }


        private void AktualnosciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Aktualnosci();
        }

        private void SamochodyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Samochody();
        }

        private void NoweZlecenieClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new NoweZlecenie();
        }


    }
}
