using BibliotekaKlas;
using PasswordCryptography;
using Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using WarsztatV2.SMS;

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
            SplashScreen subWindow = new SplashScreen();

            socketConnection socketConnection = new socketConnection();
            Task.Run(() => socketConnection.sCConnection());

            subWindow.Show();
            loading();
            subWindow.Close();

            RightContent.Content = new Aktualnosci();
            // domyslnie podmenu zlecienie - ukryte
            HideSubmenu();

            //Obsługa zamykania aplikacji - wyświetlenie odpowiedniego komunikatu
            Closing += (s, e) =>
            {
                e.Cancel = true; //Wstrzymanie zamykania okna 
                if (MessageBox.Show("Czy na pewno chcesz zamknąć aplikację?", "Zamykanie aplikacji Warsztat v2.0", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) //Wyświetlenie boxa z informacją o akcji zamknięcia
                {
                    e.Cancel = false; //Po potwierdzeniu woli zamknięcia ustawiam flagę na false - zamykanie trwa
                }
            };
        }

        /// <summary>
        /// Metoda ładująca dane po raz pierwszy
        /// </summary>
        void loading()
        {
            List<Naprawa> naprawaL;
            List<Pracownik> pracownikL;
            List<Pojazd> pojazdL;
            Warsztat warsztatL;


            using (databaseConnection newConnection = new databaseConnection())
            {
                naprawaL = newConnection.Naprawy.ToList<Naprawa>();
                pracownikL = newConnection.Pracownicy.ToList<Pracownik>();
                pojazdL = newConnection.Pojazdy.ToList<Pojazd>();
                warsztatL = newConnection.Warsztaty.FirstOrDefault<Warsztat>();
            }
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
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }

        private void KlienciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Klienci();
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }

        private void PracownicyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Pracownicy();
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }


        private void AktualnosciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Aktualnosci();
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }

        private void SamochodyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Samochody();
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }

        private void CzesciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Czesci();
            if (NoweZlecenie.IsVisible == true)
                HideSubmenu();
        }

        private void NoweZlecenieClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new NoweZlecenie();
        }
        private void DoNaprawyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new DoNaprawy();
        }

        private void DoOdbioruClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new DoOdbioru();
        }

        private void HistoriaZlecenClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new HistoriaZlecen();
        }


    }
}