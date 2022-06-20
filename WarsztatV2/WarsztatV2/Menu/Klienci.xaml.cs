using BibliotekaKlas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using WarsztatV2;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for Klienci.xaml
    /// </summary>

    public partial class Klienci : Page
    {
        private int KlientID { get; set; } //Własność do przechowywania indeksu edytowanego klienta
        private int AdresID { get; set; } //Własność używana do przechowywania indeksu edytowanego adresu
        private List<DaneKlient> wynikL = new List<DaneKlient>(); // Lista elementów - do wyświetlenia w ListBoxie
        Validation validation = new Validation();
        public Klienci()
        {
            InitializeComponent();
            zmienDostepnosc(false);
            pobieranieDanych();
            KlientID = -1; //Ustawienie indeksów na wartości "nullowe" - brak indeksu
            AdresID = -1;
        }


        /// <summary>
        /// Metoda pobierająca dane z bazy danych i wypełniająca odpowiedni ListView danymi
        /// </summary>
        private async void pobieranieDanych()
        {
            List<Klient> klientL;
            List<Adres> adresL;
            List<Pojazd> pojazdL;
            using (databaseConnection newConnection = new databaseConnection())
            {
                klientL = await Task.Run(() => { return newConnection.Klienci.ToList<Klient>(); }); //Pobranie danych (pobieramy dane o pojazdach, bo usunąć będziemy mogli tylko tego klienta, który z jakiś powodów nie miał i nie ma żadnego pojazdu w naprawie)
                adresL = await Task.Run(() => { return newConnection.Adresy.ToList<Adres>(); });
                pojazdL = await Task.Run(() => { return newConnection.Pojazdy.ToList<Pojazd>(); });
                for (int i = 0; i < klientL.Count; i++) //Łączenie klientów ze swoimi adresami
                {
                    for (int j = 0; j < adresL.Count; j++)
                    {
                        if (klientL[i].ID_Adres == adresL[j].ID_Adres)
                        {
                            wynikL.Add(new DaneKlient { ID_Klient = klientL[i].ID_Klient, Imie = klientL[i].Imie, Nazwisko = klientL[i].Nazwisko, Telefon = klientL[i].Telefon, ID_Adres = klientL[i].ID_Adres, Ulica = adresL[j].Ulica, Numer = adresL[j].Numer, Kod_pocztowy = adresL[j].Kod_pocztowy, Miejscowosc = adresL[j].Miejscowosc, PosiadaPojazd = posiadaPojazd(pojazdL, klientL[i].ID_Klient) });
                        }
                    }
                }

                Thread updateInfoLAndSearchBT = new Thread( //Aktualizacja informacji / Pojawienie się pola do wyszukiwania
                    () => this.Dispatcher.Invoke(
                        () => {
                            informationLabel.Visibility = Visibility.Hidden;
                            searchTextBox.Visibility = Visibility.Visible;
                            zmienDostepnosc(true);
                        })
                    );
                updateInfoLAndSearchBT.Start();
            }

            //Sortowanie i mapowanie rekordu w ListView
            lvDataBinding.ItemsSource = wynikL;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ID_Klient");

            view.GroupDescriptions.Add(groupDescription);
            view.SortDescriptions.Add(new SortDescription("ID_Klient", ListSortDirection.Ascending));
            view.Filter = klientFilter;
        }

        /// <summary>
        /// Metoda zwracająca prawdę dla rekordów, które spełniają tę własność, że wpisany ciąg znaków do searchBara jest w dowolnym stopniu podobny do danych z kolumny Imię, lub Nazwisko
        /// </summary>
        /// <param name="item">Rekord z ListView zawierający dane klienta</param>
        private bool klientFilter(object item) 
        {
            if (String.IsNullOrEmpty(searchTextBox.Text)) return true;
            else
            {
                if ((item as DaneKlient).Imie.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ((item as DaneKlient).Imie.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else
                {
                    return ((item as DaneKlient).Nazwisko.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
        }

        private void searchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }

        /// <summary>
        /// Obsługa klikania w ListView zawierający listę klientów
        /// </summary>
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                DaneKlient klient = (DaneKlient)element.Content;
                if (klient != null)
                {
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(
                            () => {
                                imie.Text = klient.Imie;
                                nazwisko.Text = klient.Nazwisko;
                                miejscowosc.Text = klient.Miejscowosc;
                                ulica.Text = klient.Ulica;
                                numer.Text = klient.Numer;
                                kod_pocztowy.Text = klient.Kod_pocztowy;
                                telefon.Text = klient.Telefon.ToString();
                                if (klient.PosiadaPojazd) usun.IsEnabled = false;
                                else usun.IsEnabled = true;
                            }
                        );
                    });
                    KlientID = klient.ID_Klient;
                    AdresID = klient.ID_Adres;
                    ramkaKolor(); //Przywrócenie koloru ramek
                }
            }
        }

        /// <summary>
        /// Metoda zajmująca się obsługa przycisku dodaj
        /// </summary>
        /// /// <param name="sender">Przycisk dodaj, bądź każdy inny mający spełniać taką rolę</param>
        /// <param name="e"></param>
        private async void dodajClick(object sender, RoutedEventArgs e)
        {
            string imieD = imie.Text;
            string nazwiskoD = nazwisko.Text;
            string telefonD = telefon.Text;
            string miejscowoscD = miejscowosc.Text;
            string ulicaD = ulica.Text;
            string numerD = numer.Text;
            string kodPocztowyD = kod_pocztowy.Text;

            if (imieD.Length != 0 && nazwiskoD.Length != 0 && telefonD.Length != 0 && miejscowoscD.Length != 0 && ulicaD.Length != 0 && numerD.Length != 0 && kodPocztowyD.Length != 0)
            {
                Thread watek = new Thread(
                    () => this.Dispatcher.Invoke(
                        () =>
                        {
                            zmienDostepnosc(false);
                            wyczyscZawartosc();
                            MessageBox.Show("Proces dodawania nowego rekordu do bazy danych przebiegł pomyślnie!", "Dodawanie danych", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        }
                    )
                );
                await Task.Run(
                    () =>
                    {
                        using (databaseConnection newConnection = new databaseConnection())
                        {
                            newConnection.Klienci.Add(
                                new Klient
                                {
                                    Imie = imieD,
                                    Nazwisko = nazwiskoD,
                                    Telefon = Convert.ToInt32(telefonD),
                                    AdresNav = new Adres
                                    {
                                        Ulica = ulicaD,
                                        Numer = numerD,
                                        Kod_pocztowy = kodPocztowyD,
                                        Miejscowosc = miejscowoscD
                                    }
                                }
                            );
                            newConnection.SaveChanges();
                        }
                    }
                );
                wynikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                await Task.Run(
                    () => this.Dispatcher.Invoke(
                        () => pobieranieDanych()
                    )
                );

                watek.Start();

                ramkaKolor(); //Przywrócenie koloru ramek
            }
            else
            {
                MessageBox.Show("Nie wszystkie pola formularza zostały wypełnione!", "Puste pola formularza", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Metoda zajmująca się obsługą przycisku usun
        /// </summary>
        /// /// <param name="sender">Przycisk usuń, bądź każdy inny mający spełniać taką rolę</param>
        /// <param name="e"></param>
        private async void usunClick(object sender, RoutedEventArgs e)
        {
            if (KlientID != -1 && AdresID != -1)
            {
                Thread watek = new Thread(
                    () => this.Dispatcher.Invoke(
                        () =>
                        {
                            zmienDostepnosc(true);
                            wyczyscZawartosc();
                            MessageBox.Show("Proces usuwania wskazanego rekordu z bazy danych przebiegł pomyślnie!", "Usuwanie danych", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        }
                    )
                );
                await Task.Run(
                () =>
                {
                    this.Dispatcher.Invoke(
                        () => zmienDostepnosc(false)
                    );
                    using (databaseConnection newConnection = new databaseConnection())
                    {
                        newConnection.Klienci.Remove(newConnection.Klienci.Single<Klient>(k => k.ID_Klient == KlientID));
                        newConnection.SaveChanges();
                    }
                }
                );
                wynikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                await Task.Run(
                    () => this.Dispatcher.Invoke(
                        () => pobieranieDanych()
                    )
                );

                watek.Start();

                KlientID = -1; //Zerowanie danych na temat ostatnio wybranego rekordu
                AdresID = -1;

                ramkaKolor(); //Przywrócenie koloru ramek
            }
        }

        /// <summary>
        /// Metoda zajmująca się obsługą przycisku do modyfikacji
        /// </summary>
        /// <param name="sender">Przycisk modyfikuj, bądź każdy inny mający spełniać taką rolę</param>
        /// <param name="e"></param>
        private async void modyfikujClick(object sender, RoutedEventArgs e)
        {
            if (KlientID != -1 && AdresID != -1)
            {
                Thread watek = new Thread(
                () => this.Dispatcher.Invoke(
                    () =>
                    {
                        zmienDostepnosc(false);
                        wyczyscZawartosc();
                        MessageBox.Show("Proces modyfikacji wskazanego rekordu z bazy danych przebiegł pomyślnie!", "Modyfikacja danych", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    })
                );
                await Task.Run(() =>
                {
                    using (databaseConnection newConnection = new databaseConnection())
                    {
                        Klient klientM = newConnection.Klienci.Single<Klient>(k => k.ID_Klient == KlientID);
                        Adres adresM = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == AdresID);
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                klientM.Imie = imie.Text;
                                klientM.Nazwisko = nazwisko.Text;
                                klientM.Telefon = Convert.ToInt32(telefon.Text);
                                adresM.Miejscowosc = miejscowosc.Text;
                                adresM.Ulica = ulica.Text;
                                adresM.Numer = numer.Text;
                                adresM.Kod_pocztowy = kod_pocztowy.Text;
                            }
                        );
                        newConnection.SaveChanges();
                    }
                });

                wynikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                await Task.Run(
                    () => this.Dispatcher.Invoke(
                        () => pobieranieDanych()
                    )
                );

                watek.Start(); //Uruchomienie wątka

                KlientID = -1; //Zerowanie danych na temat ostatnio wybranego rekordu
                AdresID = -1;

                ramkaKolor(); //Przywrócenie koloru ramek
            }
        }

        /// <summary>
        /// Metoda zmieniająca widoczność (zakrycie / odkrycie) inputów i przycisków
        /// </summary>
        /// <param name="wartosc">Wartość bool, jeśli true: pola formularza zostaną odkryte do edycji, jeśli false: pola formularza zostaną zakryte przed edycją</param>
        private void zmienDostepnosc(bool wartosc)
        {
            imie.IsEnabled = wartosc;
            nazwisko.IsEnabled = wartosc;
            miejscowosc.IsEnabled = wartosc;
            ulica.IsEnabled = wartosc;
            numer.IsEnabled = wartosc;
            kod_pocztowy.IsEnabled = wartosc;
            telefon.IsEnabled = wartosc;
            dodaj.IsEnabled = wartosc;
            modyfikuj.IsEnabled = wartosc;
            usun.IsEnabled = wartosc;
            lvDataBinding.IsEnabled = wartosc;
        }

        /// <summary>
        /// Metoda czyszcząca zawartość inputów w formularzu
        /// </summary>
        private void wyczyscZawartosc()
        {
            imie.Clear();
            nazwisko.Clear();
            telefon.Clear();
            miejscowosc.Clear();
            ulica.Clear();
            numer.Clear();
            kod_pocztowy.Clear();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy klient posiada pojazd
        /// </summary>
        /// <param name="listaPojazdow">Lista typu Pojazd zawierająca pojazdy z bazy danych</param>
        /// <param name="indeksKlienta">Numer identyfikacyjny klienta</param>
        private bool posiadaPojazd(List<Pojazd> listaPojazdow, int indeksKlienta)
        {
            for (int i = 0; i < listaPojazdow.Count; i++)
            {
                if (listaPojazdow[i].ID_Klient == indeksKlienta) return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda ustawiający pierwotny kolor obramowania TextBoxów
        /// </summary>
        private void ramkaKolor()
        {
            validation.colorRestore(imie);
            validation.colorRestore(nazwisko);
            validation.colorRestore(miejscowosc);
            validation.colorRestore(ulica);
            validation.colorRestore(numer);
            validation.colorRestore(kod_pocztowy);
            validation.colorRestore(telefon);
        }

        /// <summary>
        /// Metoda zajmująca się walidacją danych wprowadzonych przez użytkownika
        /// </summary>
        /// <param name="sender">Obiekt typu TextBox z odpowiednią nazwą</param>
        /// <param name="e"></param>
        private void textBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tB = sender as TextBox;
                validation.checkData(tB);
            }
        }
    }

    //Klasa pojedynczego wiersza w ListView (potrzebna, by poprawnie generować odpowiednie wiersze)
    public class DaneKlient
    {
        public int ID_Klient { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int Telefon { get; set; }
        public int ID_Adres { get; set; }
        public string Ulica { get; set; }
        public string Numer { get; set; }
        public string Kod_pocztowy { get; set; }
        public string Miejscowosc { get; set; }
        public bool PosiadaPojazd { get; set; }
    }
}