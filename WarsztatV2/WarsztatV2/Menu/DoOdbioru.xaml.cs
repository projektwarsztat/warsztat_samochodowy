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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarsztatV2.Faktury;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BibliotekaKlas;
using System.IO;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for DoOdbioru.xaml
    /// </summary>
    public partial class DoOdbioru : Page
    {
        private int PracownikID { get; set; }
        private int NaprawaID { get; set; }

        public DoOdbioru()
        {

            InitializeComponent();
            NaprawaID = -1;
            _ = pobierzDaneNaprawy();
            WystawFaktureBtn.IsEnabled = false;
            WydajPojazd.IsEnabled = false;

        }

        // wypelnienie listviewbox
        private async Task pobierzDaneNaprawy()
        {
            List<Naprawa> naprawaL;
            List<Pracownik> pracownikL;
            List<Pojazd> pojazdL;


            ObservableCollection<DaneNaprawa> wynikL = new ObservableCollection<DaneNaprawa>();
            using (databaseConnection newConnection = new databaseConnection())
            {
                naprawaL = await Task.Run(() => { return newConnection.Naprawy.ToList<Naprawa>(); });
                pracownikL = await Task.Run(() => { return newConnection.Pracownicy.ToList<Pracownik>(); });
                pojazdL = await Task.Run(() => { return newConnection.Pojazdy.ToList<Pojazd>(); });


                for (int i = 0; i < naprawaL.Count; i++)
                {
                    for (int j = 0; j < pracownikL.Count; j++)
                    {
                        for (int k = 0; k < pojazdL.Count; k++)
                            if (naprawaL[i].ID_Pracownik == pracownikL[j].ID_Pracownik && naprawaL[i].Numer_rejestracyjny == pojazdL[k].Numer_rejestracyjny && naprawaL[i].Status_naprawy == "DoOdbioru")
                            {
                                wynikL.Add(new DaneNaprawa
                                {
                                    Imie = pracownikL[j].Imie,
                                    Nazwisko = pracownikL[j].Nazwisko,
                                    Data_przyjecia = naprawaL[i].Data_przyjecia,
                                    Numer_rejestracyjny = naprawaL[i].Numer_rejestracyjny,
                                    MarkaModel = pojazdL[k].Marka + " " + pojazdL[k].Model,
                                    Opis_usterek = naprawaL[i].Opis_usterek,
                                    Wiadomosc_zwrotna = naprawaL[i].Wiadomosc_zwrotna,
                                    ID_Naprawa = naprawaL[i].ID_Naprawa,
                                });
                            }
                    }
                }
                Thread informationLabelT = new Thread(() => this.Dispatcher.Invoke(() => informationLabel.Visibility = Visibility.Hidden));
                informationLabelT.Start();
            }

            lvDataBinding.ItemsSource = wynikL;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
            view.Filter = UserFilter;
        }

        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                DaneNaprawa naprawa = (DaneNaprawa)element.Content;
                await Task.Run(() =>
                {
                    this.Dispatcher
                        .Invoke(() => {
                            Opis_usterek.Text = naprawa.Opis_usterek;
                            Wiadomosc_zwrotna.Text = naprawa.Wiadomosc_zwrotna;
                        }
                    );
                    NaprawaID = naprawa.ID_Naprawa;

                    if (IfFakturaExists())
                    {
                        WystawFaktureBtn.Dispatcher.Invoke(() => { WystawFaktureBtn.IsEnabled = false; });
                        WydajPojazd.Dispatcher.Invoke(() => { WydajPojazd.IsEnabled = true; });
                    }


                    else
                    {
                        WystawFaktureBtn.Dispatcher.Invoke(() => { WystawFaktureBtn.IsEnabled = true; });
                        WydajPojazd.Dispatcher.Invoke(() => { WydajPojazd.IsEnabled = false; });
                    }



                });
            }
        }


        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text))
                return true;
            else
                return ((item as DaneNaprawa).Numer_rejestracyjny.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);

        }

        private void searchTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }

        public class DaneNaprawa
        {
            public int ID_Naprawa { get; set; }
            public string Numer_rejestracyjny { get; set; }
            public string MarkaModel { get; set; }

            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public int ID_Pracownik { get; set; }
            public DateTime? Data_przyjecia { get; set; }
            public string Status_naprawy { get; set; }
            public string Opis_usterek { get; set; }
            public string Wiadomosc_zwrotna { get; set; }

        }

        //sprawdzenie czy faktura o danym Id juz istnieje
        private bool IfFakturaExists()
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                if (newConnection.Faktury.Any(p => p.ID_Naprawa == NaprawaID))
                    return true;
                else
                    return false;
            }
        }

        //wystawienie faktury
        private async void WystawFakture(object sender, RoutedEventArgs e)
        {

            GenerowanieFaktur.GetData();

            if (Directory.Exists(GenerowanieFaktur.SciezkaDoZapisu))
            {
                Naprawa NaprawaFav = new Naprawa();
                Warsztat WarsztatFav = new Warsztat();
                Pojazd PojazdFav = new Pojazd();

                Faktura fav1 = new Faktura();

                using (databaseConnection newConnection = new databaseConnection())
                {
                    WarsztatFav = await Task.Run(() => { return newConnection.Warsztaty.First<Warsztat>(); });
                    NaprawaFav = await Task.Run(() => { return newConnection.Naprawy.Single<Naprawa>(a => a.ID_Naprawa == NaprawaID); });
                    PojazdFav = await Task.Run(() => { return newConnection.Pojazdy.Single<Pojazd>(a => a.Numer_rejestracyjny == NaprawaFav.Numer_rejestracyjny); });

                    fav1.ID_Warsztat = WarsztatFav.ID_Warsztat;
                    fav1.ID_Klient = PojazdFav.ID_Klient;
                    fav1.ID_Naprawa = NaprawaFav.ID_Naprawa;

                    if (IfFakturaNotNull(fav1))
                    {
                        await Task.Run(() => newConnection.Faktury.Add(fav1));

                        await newConnection.SaveChangesAsync();
                    
                         GenerowanieFaktur.GenerujFakture(fav1);

                         WystawFaktureBtn.Dispatcher.Invoke(() => { WystawFaktureBtn.IsEnabled = false; });
                         WydajPojazd.Dispatcher.Invoke(() => { WydajPojazd.IsEnabled = true; });
                    }

                    else
                    {
                        MessageBox.Show("Wygenerowanie faktury jest niemożliwe, ponieważ nie zawiera żadnych elementów.", "Błąd !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }

            }
            else
            {
                MessageBox.Show("Błędna ścieżka do zapisu faktury. Sprawdź ścieżkę w zkaładce O Firmie", "Błąd !", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private bool IfFakturaNotNull(Faktura fav)
        {
            List<Uzyte_czesci> czesci = new List<Uzyte_czesci>();

            using (databaseConnection newConnection = new databaseConnection())
            {
                czesci = newConnection.Uzyte_czesci.Where<Uzyte_czesci>(u => u.ID_Naprawa == fav.ID_Naprawa).ToList();
                if (czesci.Count() != 0)
                    return true;

                else return false;

            }
        }

        // wydanie pojazdu - zmiana statusu naprawy i uzupenienie daty wydania
        private async void WydajPojazdClick(object sender, RoutedEventArgs e)
        {

            await Task.Run(() =>
            {
                using (databaseConnection newConnection = new databaseConnection())
                {
                    Naprawa naprawaModyfikacja = newConnection.Naprawy.Single<Naprawa>(p => p.ID_Naprawa == NaprawaID);

                    this.Dispatcher.Invoke(() =>
                    {
                        naprawaModyfikacja.Status_naprawy = "Wydany";
                        naprawaModyfikacja.Data_wydania = DateTime.Now;

                    });
                    newConnection.SaveChanges();
                }
            });

            _ = pobierzDaneNaprawy();
        }
    }
}
