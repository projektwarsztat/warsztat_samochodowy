using BibliotekaKlas;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

namespace WarsztatV2_klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string ID;
        List<Naprawa> naprawaL;
        List<Pojazd> pojazdL;
        public MainWindow(string ID)
        {
            InitializeComponent();
            this.ID = ID;
            Task.Run(async() =>
            {
                Socket clientSocketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Utworzenie gniazda
                EndPoint serverSocketConnection = new IPEndPoint(IPAddress.Loopback, 19164); //Utworzenie adresu
                try
                {
                    clientSocketConnection.Connect(serverSocketConnection);
                    if (clientSocketConnection.Connected)
                    {
                        NetworkStream networkStream = new NetworkStream(clientSocketConnection); //Utworzenie strumienia do komunikacji
                        BinaryFormatterAsync bF = new BinaryFormatterAsync(); //Umożliwia serializację danych

                        await bF.SerializeAsync(networkStream, "NDATA" + ID); //Wysłanie powiadomienia do serwera prośby o udostępnienie danych

                        string name = await bF.DeserializeAsync<string>(networkStream); //Odczytanie imienia i nazwiska, oraz poprawienie tytułu strony
                        Dispatcher.Invoke(() => { this.Title += ". Zalogowano jako: " + name; });

                        naprawaL = await bF.DeserializeAsync<List<Naprawa>>(networkStream);
                        pojazdL = await bF.DeserializeAsync<List<Pojazd>>(networkStream);
                        Dispatcher.Invoke(() => { lvDataBinding.ItemsSource = naprawaL; });
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);

                        bF.Serialize<string>(networkStream, "END_CONN"); //Kończenie połączenia
                        networkStream.Close();
                        clientSocketConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            });

            //Obsługa zamykania aplikacji - wyświetlenie odpowiedniego komunikatu
            Closing += (s, e) =>
            {
                e.Cancel = true; //Wstrzymanie zamykania okna 
                if (MessageBox.Show("Czy na pewno chcesz zamknąć aplikację?", "Zamykanie aplikacji Warsztat v2.0 - Mechanik", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) //Wyświetlenie boxa z informacją o akcji zamknięcia
                {
                    e.Cancel = false; //Po potwierdzeniu woli zamknięcia ustawiam flagę na false - zamykanie trwa
                }
            };
        }
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                Naprawa naprawa = (Naprawa)element.Content;
                if (naprawa != null)
                {
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(
                            () => {
                                numerRejestracyjnyIn.Text = naprawa.Numer_rejestracyjny;
                                markaIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Marka;
                                modelIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Model;
                                numerVINIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Numer_VIN;
                                rokProdukcjiIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Rok_produkcji.ToString();
                                typPaliwaIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Typ_paliwa;
                            }
                        );
                    });
                    //KlientID = klient.ID_Klient;
                    //AdresID = klient.ID_Adres;
                }
            }
        }
        private void refreshViewButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                Socket clientSocketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Utworzenie gniazda
                EndPoint serverSocketConnection = new IPEndPoint(IPAddress.Loopback, 19164); //Utworzenie adresu
                try
                {
                    clientSocketConnection.Connect(serverSocketConnection);
                    if (clientSocketConnection.Connected)
                    {
                        NetworkStream networkStream = new NetworkStream(clientSocketConnection); //Utworzenie strumienia do komunikacji
                        BinaryFormatterAsync bF = new BinaryFormatterAsync(); //Umożliwia serializację danych

                        await bF.SerializeAsync(networkStream, "NDATA" + ID); //Wysłanie powiadomienia do serwera prośby o udostępnienie danych

                        string name = await bF.DeserializeAsync<string>(networkStream); //Odczytanie imienia i nazwiska, oraz poprawienie tytułu strony

                        naprawaL = await bF.DeserializeAsync<List<Naprawa>>(networkStream);
                        pojazdL = await bF.DeserializeAsync<List<Pojazd>>(networkStream);
                        Dispatcher.Invoke(() => { lvDataBinding.ItemsSource = naprawaL; });
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);

                        bF.Serialize<string>(networkStream, "END_CONN"); //Kończenie połączenia
                        networkStream.Close();
                        clientSocketConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            });
        }
    }
}
