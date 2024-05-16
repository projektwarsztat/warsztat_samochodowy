using BibliotekaKlas;
using PasswordCryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for Pracownicy.xaml
    /// </summary>
    public partial class Pracownicy : Page
    {
        private int PracownikID { get; set; } //Własność do przechowywania indeksu edytowanego klienta
        private int AdresID { get; set; } //Własność używana do przechowywania indeksu edytowanego adresu
        private int Dane_logowaniaID { get; set; } //Własność używana do przechowywania indeksu edytowanych danych logowania

        List<Pracownik> pracownikL; //Lista pracowników
        List<Adres> adresL; //Lista adresów
        List<Dane_logowania> dane_logowaniaL; //Lista danych logowania
        List<Naprawa> naprawaL; //Lista napraw
        Validation validation = new Validation();
        public Pracownicy()
        {
            InitializeComponent();
            zmienDostepnosc(false);
            pobieranieDanych();

            PracownikID = -1; //Ustawienie indeksów na wartości "nullowe" - brak indeksu
            AdresID = -1;
            Dane_logowaniaID = -1;
        }

        /// <summary>
        /// Pobranie danych o pracownikach warsztatu - pobranie informacji z tabeli pracownicy oraz z tabeli adresy
        /// </summary>
        private async void pobieranieDanych()
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                pracownikL = await Task.Run(() => { return newConnection.Pracownicy.ToList<Pracownik>(); }); //Pobranie danych (pobranie danych o pracownikach)
                adresL = await Task.Run(() => { return newConnection.Adresy.ToList<Adres>(); });
                dane_logowaniaL = await Task.Run(() => { return newConnection.Dane_logowania.ToList<Dane_logowania>(); });
                naprawaL = await Task.Run(() => { return newConnection.Naprawy.ToList<Naprawa>(); });

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

            //Sortowanie i mapowanie rekordu w ListView po imieniu i/lub nazwisku
            lvDataBinding.ItemsSource = pracownikL;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ID_Pracownik");

            view.GroupDescriptions.Add(groupDescription);
            view.SortDescriptions.Add(new SortDescription("ID_Pracownik", ListSortDirection.Ascending));
            view.Filter = pracownikFilter;
        }

        /// <summary>
        /// Metoda zwracająca prawdę dla rekordów, które spełniają tę własność, że wpisany ciąg znaków do searchBara jest w dowolnym stopniu podobny do danych z kolumny Imię, lub Nazwisko
        /// </summary>
        private bool pracownikFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text)) return true;
            else
            {
                if ((item as Pracownik).Imie.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ((item as Pracownik).Imie.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else
                {
                    return ((item as Pracownik).Nazwisko.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
        }

        /// <summary>
        /// Odświeża widok ListView po każdej aktualizacji związanej z wyszukiwaniem pracownika
        /// </summary>
        private void searchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }

        /// <summary>
        /// Obsługa kliknięcia lewym przyciskiem myszy na rekord z danymi pracownika. Lista boczna wypełni się danymi
        /// </summary>
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                Pracownik pracownik = (Pracownik)element.Content;
                if (pracownik != null)
                {
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(
                            () => {
                                imie.Text = pracownik.Imie;
                                nazwisko.Text = pracownik.Nazwisko;
                                miejscowosc.Text = pracownik.AdresNav.Miejscowosc;
                                ulica.Text = pracownik.AdresNav.Ulica;
                                numer.Text = pracownik.AdresNav.Numer;
                                kod_pocztowy.Text = pracownik.AdresNav.Kod_pocztowy;
                                telefon.Text = pracownik.Telefon.ToString();
                                login.Text = pracownik.Dane_logowaniaNav.Login;
                                haslo.Password = passwordCryptography.Decrypt(pracownik.Dane_logowaniaNav.Haslo);
                                pokazHaslo.IsEnabled = true;

                                if (wykonujeNaprawe(naprawaL, pracownik.ID_Pracownik)) usun.IsEnabled = false;
                                else usun.IsEnabled = true;
                            }
                        );
                    });
                    PracownikID = pracownik.ID_Pracownik;
                    AdresID = pracownik.ID_Adres;
                    Dane_logowaniaID = pracownik.ID_Dane_logowania;
                    ramkaKolor();
                }
            }
        }

        /// <summary>
        /// Obsluga dodawania nowego pracownika
        /// </summary>
        private async void dodajClick(object sender, RoutedEventArgs e)
        {
            string imieD = imie.Text;
            string nazwiskoD = nazwisko.Text;
            string telefonD = telefon.Text;
            string miejscowoscD = miejscowosc.Text;
            string ulicaD = ulica.Text;
            string numerD = numer.Text;
            string kodPocztowyD = kod_pocztowy.Text;
            string loginD = login.Text;
            string hasloD = haslo.Password;

            bool istnieje = await Task.Run(() => istniejeLoginWBazie(loginD));
            if(istnieje == false)
            {
                if (imieD.Length != 0 && nazwiskoD.Length != 0 && telefonD.Length != 0 && miejscowoscD.Length != 0 && ulicaD.Length != 0 && numerD.Length != 0 && kodPocztowyD.Length != 0 && loginD.Length != 0 && hasloD.Length != 0)
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
                                newConnection.Pracownicy.Add(
                                    new Pracownik
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
                                        },
                                        Dane_logowaniaNav = new Dane_logowania
                                        {
                                            Login = loginD,
                                            Haslo = passwordCryptography.Encrypt(hasloD)
                                        }
                                    }
                                );
                                newConnection.SaveChanges();
                            }
                        }
                    );
                    pracownikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                    await Task.Run(
                        () => this.Dispatcher.Invoke(
                            () => pobieranieDanych()
                        )
                    );

                    watek.Start();
                    ramkaKolor();
                }
                else
                {
                    MessageBox.Show("Nie wszystkie pola formularza zostały wypełnione!", "Puste pola formularza", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
            else
            {
                MessageBox.Show("Któryś z pracowników posiada już taki login!", "Identyczny login", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Obsługa usuwania prawcownika
        /// </summary>
        private async void usunClick(object sender, RoutedEventArgs e)
        {
            if (PracownikID != -1 && AdresID != -1)
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
                        newConnection.Pracownicy.Remove(newConnection.Pracownicy.Single<Pracownik>(p => p.ID_Pracownik == PracownikID));
                        newConnection.Dane_logowania.Remove(newConnection.Dane_logowania.Single<Dane_logowania>(dl => dl.ID_Dane_logowania == Dane_logowaniaID));
                        newConnection.SaveChanges();
                    }
                }
                );
                pracownikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                adresL.Clear();
                await Task.Run(
                    () => this.Dispatcher.Invoke(
                        () => pobieranieDanych()
                    )
                );

                watek.Start();

                PracownikID = -1; //Zerowanie danych na temat ostatnio wybranego rekordu
                AdresID = -1;
                Dane_logowaniaID = -1;
                ramkaKolor();
            }
        }

        /// <summary>
        /// Obsługa modyfikacji danych pracownika
        /// </summary>
        private async void modyfikujClick(object sender, RoutedEventArgs e)
        {
            if (PracownikID != -1 && AdresID != -1)
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
                        Pracownik pracownikM = newConnection.Pracownicy.Single<Pracownik>(k => k.ID_Pracownik == PracownikID);
                        Adres adresM = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == AdresID);
                        Dane_logowania daneM = newConnection.Dane_logowania.Single<Dane_logowania>(dn => dn.ID_Dane_logowania == pracownikM.ID_Dane_logowania);
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                pracownikM.Imie = imie.Text;
                                pracownikM.Nazwisko = nazwisko.Text;
                                pracownikM.Telefon = Convert.ToInt32(telefon.Text);
                                adresM.Miejscowosc = miejscowosc.Text;
                                adresM.Ulica = ulica.Text;
                                adresM.Numer = numer.Text;
                                adresM.Kod_pocztowy = kod_pocztowy.Text;
                                daneM.Login = login.Text;
                                daneM.Haslo = passwordCryptography.Encrypt(haslo.Password);
                            }
                        );
                        newConnection.SaveChanges();
                    }
                });

                pracownikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
                adresL.Clear();
                await Task.Run(
                    () => this.Dispatcher.Invoke(
                        () => pobieranieDanych()
                    )
                );

                watek.Start(); //Uruchomienie wątka

                PracownikID = -1; //Zerowanie danych na temat ostatnio wybranego rekordu
                AdresID = -1;
                Dane_logowaniaID = -1;
                ramkaKolor();
            }
        }

        /// <summary>
        /// Metoda zmieniająca widoczność (zakrycie / odkrycie) inputów i przycisków
        /// </summary>
        private void zmienDostepnosc(bool wartosc)
        {
            imie.IsEnabled = wartosc;
            nazwisko.IsEnabled = wartosc;
            miejscowosc.IsEnabled = wartosc;
            ulica.IsEnabled = wartosc;
            numer.IsEnabled = wartosc;
            kod_pocztowy.IsEnabled = wartosc;
            telefon.IsEnabled = wartosc;
            login.IsEnabled = wartosc;
            haslo.IsEnabled = wartosc;
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
            login.Clear();
            haslo.Clear();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy pracownik wykonuje naprawę
        /// </summary>
        private bool wykonujeNaprawe(List<Naprawa> listaNapraw, int indeksPracownika)
        {
            for (int i = 0; i < listaNapraw.Count; i++)
            {
                if (listaNapraw[i].ID_Pracownik == indeksPracownika) return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda sprawdzająca, czy dany login już znajduje się w bazie danych
        /// </summary>
        private async Task<bool> istniejeLoginWBazie(string login)
        {
            bool result = false;
            using(databaseConnection newConnection = new databaseConnection())
            {
                result = await Task.Run(() => newConnection.Dane_logowania.Any<Dane_logowania>(dn => dn.Login.Equals(login)));
            }
            return result;
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
            validation.colorRestore(login);
            validation.colorRestore(haslo);
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

        private void pokazHasloClick(object sender, RoutedEventArgs e)
        {
            if (haslo.Visibility == Visibility.Visible)
            {
                hasloShow.Text = haslo.Password;
                haslo.Visibility = Visibility.Collapsed;
                hasloShow.Visibility = Visibility.Visible;
                pokazHaslo.Content = "Ukryj hasło";
            }
            else if (hasloShow.Visibility == Visibility.Visible)
            {
                haslo.Visibility = Visibility.Visible;
                hasloShow.Visibility = Visibility.Collapsed;
                pokazHaslo.Content = "Pokaż hasło";
            }
        }
    }
}
