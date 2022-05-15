using BibliotekaKlas;
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

            /* ŁĄCZENIE SIECIOWE - DZIAŁA WYMAGA PRACY, ABY MOŻE TUTAJ LUŹNO NIE LEŻAŁO */ //Dodać obsługę błędów!!!
            Socket serverSocketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Utworzenie gniazda
            EndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 19164); //Utworzenie adresu
            serverSocketConnection.Bind(endPoint); //Dowiązanie adresu do gniazda
            serverSocketConnection.Listen(50); //Ustalenie długości kolejki

            Task.Run(() => {
                while (true)
                {
                    Socket socketAccepted = serverSocketConnection.Accept();
                    Task.Run(async () => {
                        if (socketAccepted.Connected)
                        { 
                            NetworkStream networkStream = new NetworkStream(socketAccepted); //Utworzenie strumienia do komunikacji
                            BinaryFormatterAsync bF = new BinaryFormatterAsync(); //Umożliwia serializację danych
                            while (true)
                            {
                                string data = await bF.DeserializeAsync<string>(networkStream);

                                //Logowanie do aplikacji - pobranie swojego ID, aby dalej pracować
                                if (data == "LOG")
                                {
                                    string login = await bF.DeserializeAsync<string>(networkStream); //Pozyskanie danych logowania od klienta
                                    string haslo = await bF.DeserializeAsync<string>(networkStream);
                                    using (databaseConnection newConnection = new databaseConnection()) //Sprawdzenie danych w bazie danych (póki co nie ma jeszcze szyfrowania danych)
                                    {
                                        Pracownik pracownik = newConnection.Pracownicy.SingleOrDefault(dL => dL.Dane_logowaniaNav.Login == login && dL.Dane_logowaniaNav.Haslo == haslo);
                                        if (pracownik == null)
                                        {
                                            await bF.SerializeAsync<string>(networkStream, "NON_EXIST"); //Brak takich danych
                                            break; //Kończymy i zamykany strumień
                                        }
                                        else
                                        {
                                            await bF.SerializeAsync<string>(networkStream, pracownik.ID_Pracownik.ToString());
                                        }
                                    }
                                }

                                //Pobieranie danych o naprawach
                                else if (data.Contains("NDATA"))
                                {
                                    int ID = Convert.ToInt32(data.Substring(5));
                                    using (databaseConnection newConnection = new databaseConnection()) //Połączenie do bazy
                                    {
                                        List<Naprawa> naprawaL = newConnection.Naprawy.Where(n => n.ID_Pracownik == ID).ToList<Naprawa>();
                                        Pracownik pracownik = newConnection.Pracownicy.Single(p => p.ID_Pracownik == ID);
                                        if (naprawaL.Count() == 0)
                                            await bF.SerializeAsync<string>(networkStream, "NON_EXIST"); //Brak danych - pusta lista
                                        else
                                        {
                                            List<Pojazd> pojazdL = newConnection.Pojazdy.ToList<Pojazd>();
                                            await bF.SerializeAsync<string>(networkStream, pracownik.Imie + " " + pracownik.Nazwisko);
                                            await bF.SerializeAsync<List<Naprawa>>(networkStream, naprawaL);
                                            await bF.SerializeAsync<List<Pojazd>>(networkStream, pojazdL);
                                        }
                                    }
                                }

                                //Zakończenie połączenia
                                else if (data == "END_CONN") { break; }
                            }
                            networkStream.Close(); //Zamknięcie strumienia połączenia po zakończeniu pracy
                            socketAccepted.Close(); //Zamknięcie socketu po zakończeniu pracy
                        }
                    });
                }
            });
            /* ŁĄCZENIE SIECIOWE */

            subWindow.Show();
            loading();
            subWindow.Close();

            RightContent.Content = new Aktualnosci();
            // domyslnie podmenu zlecienie - ukryte
            HideSubmenu();

            // wypisanie danych aktualnie zalogowanego uzytkownika
            IdUser.Text = "Witaj ! \nJan Kowalski";

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
