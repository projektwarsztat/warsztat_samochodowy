using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Threading;
using BibliotekaKlas;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Klasa zawierająca implementację zakładki Samochody: dodawanie, podgląd wszystkich pojazdów
    /// </summary>
    public partial class Samochody : Page
    {
        private string NumerRejestracyjny { get; set; }
        private int KlientID { get; set; }
        private int AdresID { get; set; }

        public Samochody()
        {
            InitializeComponent();
            KlientID = -1;
            AdresID = -1;
            NumerRejestracyjny = "-1";
            _ = pobierzDanePojazdow();
        }

        /// <summary>
        /// Metoda pobierająca dane pojazdów, klientów i ich adresów z bazy danych, wypełnienie ListViewBox
        /// </summary>
        private async Task pobierzDanePojazdow()
        {
            List<Pojazd> pojazdL;
            List<Klient> klientL;
            List<Adres> adresL;

            ObservableCollection<DanePojazd> wynikL = new ObservableCollection<DanePojazd>();
            using (databaseConnection newConnection = new databaseConnection())
            {
                klientL = await Task.Run(() => { return newConnection.Klienci.ToList<Klient>(); });
                pojazdL = await Task.Run(() => { return newConnection.Pojazdy.ToList<Pojazd>(); });
                adresL = await Task.Run(() => { return newConnection.Adresy.ToList<Adres>(); });
                for (int i = 0; i < pojazdL.Count; i++)
                {
                    for (int j = 0; j < klientL.Count; j++)
                    {
                        for (int k = 0; k < adresL.Count; k++)
                            if (pojazdL[i].ID_Klient == klientL[j].ID_Klient && klientL[j].ID_Adres == adresL[k].ID_Adres)
                            {
                                wynikL.Add(new DanePojazd
                                {
                                    ID_Klient = klientL[j].ID_Klient,
                                    Imie = klientL[j].Imie,
                                    Nazwisko = klientL[j].Nazwisko,
                                    Telefon = klientL[j].Telefon,
                                    Numer_rejestracyjny = pojazdL[i].Numer_rejestracyjny,
                                    Marka = pojazdL[i].Marka,
                                    Model = pojazdL[i].Model,
                                    Numer_VIN = pojazdL[i].Numer_VIN,
                                    Rok_produkcji = pojazdL[i].Rok_produkcji,
                                    Typ_paliwa = pojazdL[i].Typ_paliwa,
                                    ID_Adres = adresL[k].ID_Adres,
                                    Miejscowosc = adresL[k].Miejscowosc,
                                    Ulica = adresL[k].Ulica,
                                    Numer = adresL[k].Numer,
                                    Kod_pocztowy = adresL[k].Kod_pocztowy
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

        /// <summary>
        /// Obsługa klikania w ListView zawierający listę zleceń do naprawy
        /// </summary>
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                DanePojazd pojazd = (DanePojazd)element.Content;
                await Task.Run(() =>
                {
                    this.Dispatcher
                        .Invoke(() => {
                            Imie.Text = pojazd.Imie;
                            Nazwisko.Text = pojazd.Nazwisko;
                            Telefon.Text = pojazd.Telefon.ToString();
                            Numer_rejestracyjny.Text = pojazd.Numer_rejestracyjny;
                            Marka.Text = pojazd.Marka;
                            Model.Text = pojazd.Model;
                            Numer_VIM.Text = pojazd.Numer_VIN;
                            Rok_produkcji.Text = pojazd.Rok_produkcji.ToString();
                            Typ_paliwa.Text = pojazd.Typ_paliwa;
                            Miejscowosc.Text = pojazd.Miejscowosc;
                            Ulica.Text = pojazd.Ulica;
                            Numer.Text = pojazd.Numer;
                            KodPocztowy.Text = pojazd.Kod_pocztowy;

                        }
                    );
                    KlientID = pojazd.ID_Klient;
                    AdresID = pojazd.ID_Adres;
                    NumerRejestracyjny = pojazd.Numer_rejestracyjny;
                });
            }
        }

        /// <summary>
        /// Metoda modyfikująca dane w bazie danych, wykonująca się po kliknięciu przycisku
        /// </summary>
        private async void modyfikujClick(object sender, RoutedEventArgs e)
        {
            if (KlientID != -1 && AdresID != -1 && NumerRejestracyjny != "-1")
            {
                await Task.Run(() =>
                {
                    using (databaseConnection newConnection = new databaseConnection())
                    {
                        Klient klientModyfikacja = newConnection.Klienci.Single<Klient>(k => k.ID_Klient == KlientID);
                        Adres adresModyfikacja = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == AdresID);
                        Pojazd pojazdModyfikacja = newConnection.Pojazdy.Single<Pojazd>(p => p.Numer_rejestracyjny == NumerRejestracyjny);
                        this.Dispatcher.Invoke(() =>
                        {
                            klientModyfikacja.Imie = Imie.Text;
                            klientModyfikacja.Nazwisko = Nazwisko.Text;
                            klientModyfikacja.Telefon = Convert.ToInt32(Telefon.Text);
                            adresModyfikacja.Miejscowosc = Miejscowosc.Text;
                            adresModyfikacja.Ulica = Ulica.Text;
                            adresModyfikacja.Numer = Numer.Text;
                            adresModyfikacja.Kod_pocztowy = KodPocztowy.Text;
                            pojazdModyfikacja.Numer_rejestracyjny = Numer_rejestracyjny.Text;
                            pojazdModyfikacja.Marka = Marka.Text;
                            pojazdModyfikacja.Model = Model.Text;
                            pojazdModyfikacja.Numer_VIN = Numer_VIM.Text;
                            pojazdModyfikacja.Rok_produkcji = Convert.ToInt32(Rok_produkcji.Text);
                            pojazdModyfikacja.Typ_paliwa = Typ_paliwa.Text;

                        });
                        newConnection.SaveChanges();
                    }
                });
                _ = pobierzDanePojazdow();
            }
        }

        /// <summary>
        /// Metoda zwracająca prawdę dla rekordów, które spełniają tę własność, że wpisany ciąg znaków do searchBara jest w dowolnym stopniu podobny do danych z kolumny NumerRejestracyjny
        /// </summary>
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text))
                return true;
            else
                return ((item as DanePojazd).Numer_rejestracyjny.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);

        }

        /// <summary>
        /// Wyszukiwanie w ListVievBox frazy z pola formularza 
        /// </summary>
        private void searchTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }


        /// <summary>
        /// Klasa pojedynczego wiersza w ListView (potrzebna, by poprawnie generować odpowiednie wiersze)
        /// </summary>
        public class DanePojazd
        {
            public string Numer_rejestracyjny { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public string Numer_VIN { get; set; }
            public int Rok_produkcji { get; set; }
            public string Typ_paliwa { get; set; }
            public int ID_Klient { get; set; }
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public int Telefon { get; set; }
            public int ID_Adres { get; set; }
            public string Ulica { get; set; }
            public string Numer { get; set; }
            public string Kod_pocztowy { get; set; }
            public string Miejscowosc { get; set; }

        }
    }
}
