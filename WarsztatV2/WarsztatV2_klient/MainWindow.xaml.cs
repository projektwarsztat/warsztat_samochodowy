using BibliotekaKlas;
using Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int NaprawaID { get; set; }
        List<Naprawa> naprawaL;
        List<Pojazd> pojazdL;
        List<Czesc> czescL;
        List<Uzyte_czesci> uzyteCzesciL = new List<Uzyte_czesci>();
        public MainWindow(string ID)
        {
            InitializeComponent();
            this.ID = ID;
            NaprawaID = -1; //Żadna naprawa nie została wybrana
            Task.Run(() => firstDataDownload());
            lvDataBinding3.ItemsSource = uzyteCzesciL;

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

        /// <summary>
        /// Obsługa klikania w ListView zawierający listę napraw do wykonania
        /// </summary>
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                Naprawa naprawa = (Naprawa)element.Content;
                if (naprawa != null)
                {
                    await Task.Run(() => fillOutputs(naprawa));
                    NaprawaID = naprawa.ID_Naprawa; //Została wybrana konkretna naprawa
                    Dispatcher.Invoke(() => { acceptButton.Content = "Zatwierdz bieżącą napawę jako ukończoną (" + naprawa.Numer_rejestracyjny + ")"; });

                    //Filtrowanie wyników
                    CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding3.ItemsSource);
                    view3.Filter = (o) => { if ((o as Uzyte_czesci).NaprawaNav.ID_Naprawa == NaprawaID) return true; else return false; };
                    view3.Refresh();
                }
            }
        }

        /// <summary>
        /// Obsługa klikania w ListView zawierający listę części - dodawanie elementu
        /// </summary>
        private void ListViewItemPreviewMouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            if (NaprawaID != -1)
            {
                ListViewItem element = sender as ListViewItem;
                if (element != null)
                {
                    Czesc czesc = (Czesc)element.Content;
                    if (czesc != null)
                    {
                        if (!uzyteCzesciL.Exists(uc => uc.CzescNav.ID_Czesci == czesc.ID_Czesci && uc.NaprawaNav.ID_Naprawa == NaprawaID))
                        {
                            uzyteCzesciL.Add(new Uzyte_czesci { CzescNav = czescL.Single<Czesc>(c => c.ID_Czesci == czesc.ID_Czesci), NaprawaNav = naprawaL.Single<Naprawa>(n => n.ID_Naprawa == NaprawaID), Ilosc = 1 });
                        }
                        else
                        {
                            int index = uzyteCzesciL.FindIndex(uc => uc.CzescNav.ID_Czesci == czesc.ID_Czesci && uc.NaprawaNav.ID_Naprawa == NaprawaID);
                            if (index >= 0) uzyteCzesciL[index].Ilosc++;
                        }
                        Dispatcher.Invoke(() => { lvDataBinding3.ItemsSource = uzyteCzesciL; });
                        CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding3.ItemsSource);
                        view3.Refresh();
                    }
                }
            }
        }
        /// <summary>
        /// Obsługa klikania w ListView zawierający listę części - usuwanie elementu
        /// </summary>
        private void ListViewItemPreviewMouseRightButtonDown2(object sender, MouseButtonEventArgs e)
        {
            if (NaprawaID != -1)
            {
                ListViewItem element = sender as ListViewItem;
                if (element != null)
                {
                    Czesc czesc = (Czesc)element.Content;
                    if (czesc != null)
                    {
                        if (uzyteCzesciL.Exists(uc => uc.CzescNav.ID_Czesci == czesc.ID_Czesci && uc.NaprawaNav.ID_Naprawa == NaprawaID))
                        {
                            int index = uzyteCzesciL.FindIndex(uc => uc.CzescNav.ID_Czesci == czesc.ID_Czesci && uc.NaprawaNav.ID_Naprawa == NaprawaID);
                            if (index >= 0) uzyteCzesciL[index].Ilosc--;
                        }
                        uzyteCzesciL.RemoveAll(uC => uC.Ilosc == 0);
                        Dispatcher.Invoke(() => { lvDataBinding3.ItemsSource = uzyteCzesciL; });
                        CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding3.ItemsSource);
                        view3.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Obsługa przycisku odświeżającego listę napraw
        /// </summary>
        private void refreshViewButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => refreshData());
        }

        /// <summary>
        /// Obsługa przycisku wyświetlającego w okienku opis usterek
        /// </summary>
        private void opisUsterekShow_Click(object sender, RoutedEventArgs e)
        {
            if (NaprawaID != -1) //Sprawdzenie, czy jakaś naprawa została wybrana
                MessageBox.Show(naprawaL.Where(n => n.ID_Naprawa == NaprawaID).Single().Opis_usterek, "Opis usterki dla: " + naprawaL.Where(n => n.ID_Naprawa == NaprawaID).Single().Numer_rejestracyjny, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        /// <summary>
        /// Metoda łącząca się ze serwerem w celu pobrania danych o naprawach danego pracownikach, które zostały jemu powieżone, pobrania jego danych (imienia i nazwiska), listy dostępnych części w warsztacie 
        /// </summary>
        private async Task firstDataDownload()
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
                    naprawaL = await bF.DeserializeAsync<List<Naprawa>>(networkStream); //Pobranie listy napraw dla konkretnego pracownika
                    pojazdL = await bF.DeserializeAsync<List<Pojazd>>(networkStream); //Pobranie listy wszystkich samochodów
                    Dispatcher.Invoke(() => { lvDataBinding.ItemsSource = naprawaL; }); //Wypełnienie ListView odpowiednimi danymi
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);

                    await bF.SerializeAsync(networkStream, "CDATA"); //Wysłanie powiadomienia do serwera prośby o udostępnienie danych
                    czescL = await bF.DeserializeAsync<List<Czesc>>(networkStream);
                    Dispatcher.Invoke(() => { lvDataBinding2.ItemsSource = czescL; }); //Wypełnienie ListView odpowiednimi danymi
                    Dispatcher.Invoke(() => { searchTextBox.IsEnabled = true; });

                    Dispatcher.Invoke(() => { //Możliwość filtrowania danych
                        CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding2.ItemsSource);
                        PropertyGroupDescription groupDescription = new PropertyGroupDescription("Nazwa");
                        view2.GroupDescriptions.Add(groupDescription);
                        view2.SortDescriptions.Add(new SortDescription("Nazwa", ListSortDirection.Ascending));
                        view2.Filter = czescFilter;
                    });

                    bF.Serialize<string>(networkStream, "END_CONN"); //Kończenie połączenia
                    networkStream.Close();
                    clientSocketConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Metoda wypełniająca pola dotyczące szczegółowego opisu pojazdu danymi danego pojazdu
        /// </summary>
        private void fillOutputs(Naprawa naprawa)
        {
            this.Dispatcher.Invoke(
                () =>
                {
                    numerRejestracyjnyIn.Text = naprawa.Numer_rejestracyjny;
                    markaIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Marka;
                    modelIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Model;
                    numerVINIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Numer_VIN;
                    rokProdukcjiIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Rok_produkcji.ToString();
                    typPaliwaIn.Text = pojazdL.Where(p => p.Numer_rejestracyjny == naprawa.Numer_rejestracyjny).Single().Typ_paliwa;
                    opisUsterekButtonShow.IsEnabled = true; //Przycisk jest możliwy do klikania
                    commentTextBox.IsEnabled = true;
                    acceptButton.IsEnabled = true;
                }
            );
        }

        /// <summary>
        /// Metoda czyszcząca miejsca z szczegółowymi danymi o pojeżdzie
        /// </summary>
        private void clearOutputs()
        {
            this.Dispatcher.Invoke(
                () =>
                {
                    numerRejestracyjnyIn.Text = "";
                    markaIn.Text = "";
                    modelIn.Text = "";
                    numerVINIn.Text = "";
                    rokProdukcjiIn.Text = "";
                    typPaliwaIn.Text = "";
                    opisUsterekButtonShow.IsEnabled = false; //Blokada klikania przycisku
                    acceptButton.IsEnabled = false;
                    commentTextBox.IsEnabled = false;
                    acceptButton.Content = "Zatwierdz bieżącą napawę jako ukończoną";
                    commentTextBox.Text = " ";
                }
            );
        }

        /// <summary>
        /// Metoda odświeżająca listę z naprawami
        /// </summary>
        private async Task refreshData()
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

                    string name = await bF.DeserializeAsync<string>(networkStream); //Odczytanie imienia i nazwiska, ale nic nie robi z tym

                    NaprawaID = -1; //Żadna naprawa nie została wybrana z listy, ale została ta która została wybrana poprzednio
                    naprawaL = await bF.DeserializeAsync<List<Naprawa>>(networkStream); //Pobranie listy napraw konkretnego pracownika
                    pojazdL = await bF.DeserializeAsync<List<Pojazd>>(networkStream); //Pobranie listy samochodów

                    Dispatcher.Invoke(() => { lvDataBinding.ItemsSource = naprawaL; }); //Wstawienie zaaktualizowanych danych
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
                    view.Refresh();

                    await Task.Run(() => clearOutputs()); //Wyczyszczenie opisów

                    bF.Serialize<string>(networkStream, "END_CONN"); //Kończenie połączenia
                    networkStream.Close();
                    clientSocketConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Metoda zwracająca prawdę dla rekordów, które spełniają tę własność, że wpisany ciąg znaków do searchBara jest w dowolnym stopniu podobny do danych z kolumny Nazwa (części)
        /// </summary>
        private bool czescFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text)) return true;
            else
                return ((item as Czesc).Nazwa.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Metoda odświeżająca dane w ListView - używana do filtrowania wyników
        /// </summary>
        private void searchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding2.ItemsSource).Refresh();
        }

        private async void acceptButton_Click(object sender, RoutedEventArgs e)
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

                    await bF.SerializeAsync<string>(networkStream, "SNDT"); //Wysłanie powiadomienia do serwera informacji, że będziemy przekazywać jemu dane
                    await bF.SerializeAsync<int>(networkStream, NaprawaID); //Wysłanie indeksu naprawy, którą należy ustawić na ukończoną
                    await bF.SerializeAsync<string>(networkStream, commentTextBox.Text); //Wysłanie komentarza zwrotnego od mechanika

                    List<Uzyte_czesci> uzyte_Czesci = uzyteCzesciL.Where(uc => uc.NaprawaNav.ID_Naprawa.Equals(NaprawaID)).ToList(); //Przygotowanie części użytych w konkretnej naprawie
                    await bF.SerializeAsync<List<Uzyte_czesci>>(networkStream, uzyte_Czesci); //Wysłanie części

                    uzyteCzesciL.RemoveAll(uC => uC.NaprawaNav.ID_Naprawa == NaprawaID); //Usunięcie niepotrzebnych części z listy

                    CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding3.ItemsSource); //Odświeżenie widoku
                    view3.Refresh();
                    clearOutputs();

                    bF.Serialize<string>(networkStream, "END_CONN"); //Kończenie połączenia
                    MessageBox.Show("Naprawa została ukończona!", "Sukces!", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK); //Powiadomienie o ukończonej naprawie
                    networkStream.Close();
                    clientSocketConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

            await Task.Run(() => refreshData()); //Odświeżenie danych
        }
    }
}