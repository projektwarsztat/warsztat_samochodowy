using BibliotekaKlas;
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

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Klasa zawierająca implementację zakładki Nowe Zlecenie 
    /// </summary>
    public partial class NoweZlecenie : Page
    {
        private bool IfPojazdExist = false;

        Validation validation = new Validation();
        public NoweZlecenie()
        {
            InitializeComponent();

            //pola w formularzu poza nr_rejestracyjny domyslnie niedostepne
            PojazdTextBoxInaccessible(false);
            WlascicielTextBoxInaccessible(false);
        }

        /// <summary>
        /// Ustawienie dostępności pól w formularzu z danymi pojazdu
        /// </summary>
        public void PojazdTextBoxInaccessible(bool value)
        {
            Marka.IsEnabled = value;
            Model.IsEnabled = value;
            Numer_VIN.IsEnabled = value;
            Rok_produkcji.IsEnabled = value;
            Typ_paliwa.IsEnabled = value;

        }

        /// <summary>
        /// Ustawienie dostępności pól w formularzu z danymi właściciela pojazdu
        /// </summary>
        public void WlascicielTextBoxInaccessible(bool value)
        {
            imie.IsEnabled = value;
            nazwisko.IsEnabled = value;
            miejscowosc.IsEnabled = value;
            ulica.IsEnabled = value;
            numer.IsEnabled = value;
            kod_pocztowy.IsEnabled = value;
            telefon.IsEnabled = value;

        }

        /// <summary>
        /// Sprawdzanie czy pojazd o danym numerze rejestracyjnym istnieje w bazie
        /// </summary>
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (Numer_rejestracyjny.Text.Length > 3)
            {
                DataToForm();
            }

            if (Numer_rejestracyjny.Text.Length < 7)
            {
                ClearForm();

                PojazdTextBoxInaccessible(false);
                WlascicielTextBoxInaccessible(false);
            }



        }

        /// <summary>
        /// Sprawdzenie czy Mechanik istnieje 
        /// </summary>
        private bool IfMechanikExists()
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                string imieMechanika = ImieMechanika.Text;
                string nazwiskoMechanika = NazwiskoMechanika.Text;

                if (newConnection.Pracownicy.Any(p => p.Imie == imieMechanika && p.Nazwisko == nazwiskoMechanika))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Dodanie nowej naprawy wraz z pojazdem i właścicielem () 
        /// </summary>
        async private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                //Dane pojazdu
                string numer_rejestracyjny = Numer_rejestracyjny.Text;
                string marka = Marka.Text;
                string model = Model.Text;
                string numer_VIN = Numer_VIN.Text;
                int rok_produkcji = Convert.ToInt32(Rok_produkcji.Text);
                string typ_paliwa = Typ_paliwa.Text;

                //Dane wlasciciela pojazdu
                string imieWlasciciela = imie.Text;
                string nazwiskoWlasciciela = nazwisko.Text;
                int numerTelefonu = Convert.ToInt32(telefon.Text);

                //Dane adresowe
                string nazwaMiejscowosci = miejscowosc.Text;
                string nazwaUlicy = ulica.Text;
                string numerBudynku = numer.Text;
                string numerKoduPocztowego = kod_pocztowy.Text;

                //Dane mechanika
                string imieMechanika = ImieMechanika.Text;
                string nazwiskoMechanika = NazwiskoMechanika.Text;

                //Opis usterek do naprawy
                string opis_usterek = Opis_usterek.Text;

                if (IfMechanikExists())
                {

                    if (IfPojazdExist == false)
                    {
                        await Task.Run(() => newConnection.Naprawy.Add(
                         new Naprawa
                         {
                             PojazdNav = new Pojazd
                             {
                                 Numer_rejestracyjny = numer_rejestracyjny,
                                 KlientNav = new Klient
                                 {
                                     Imie = imieWlasciciela,
                                     Nazwisko = nazwiskoWlasciciela,
                                     AdresNav = new Adres
                                     {
                                         Ulica = nazwaUlicy,
                                         Numer = numerBudynku,
                                         Kod_pocztowy = numerKoduPocztowego,
                                         Miejscowosc = nazwaMiejscowosci
                                     },
                                     Telefon = numerTelefonu
                                 },
                                 Marka = marka,
                                 Model = model,
                                 Numer_VIN = numer_VIN,
                                 Rok_produkcji = rok_produkcji,
                                 Typ_paliwa = typ_paliwa
                             },
                             Data_przyjecia = DateTime.Now,
                             PracownikNav = newConnection.Pracownicy.Single<Pracownik>(
                                        p => p.Imie == imieMechanika &&
                                        p.Nazwisko == nazwiskoMechanika),
                             Status_naprawy = "Przyjety",
                             Opis_usterek = opis_usterek,
                             Wiadomosc_zwrotna = ""
                         }
                        ));


                        MessageBox.Show("Utowrzono nowe zlecenie naprawy");
                        ClearForm();
                        Numer_rejestracyjny.Clear();

                    }


                    if (IfPojazdExist == true)
                    {
                        await Task.Run(() => newConnection.Naprawy.Add(
                           new Naprawa
                           {
                               PojazdNav = newConnection.Pojazdy.Single<Pojazd>(
                                           p => p.Numer_rejestracyjny == numer_rejestracyjny),

                               Data_przyjecia = DateTime.Now,
                               PracownikNav = newConnection.Pracownicy.Single<Pracownik>(
                                          p => p.Imie == imieMechanika &&
                                          p.Nazwisko == nazwiskoMechanika),
                               Status_naprawy = "Przyjety",
                               Opis_usterek = opis_usterek,
                               Wiadomosc_zwrotna = ""
                           }
                          ));

                        MessageBox.Show("Utowrzono nowe zlecenie naprawy", "Komunikat", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        Numer_rejestracyjny.Clear();
                    }


                }

                else
                {
                    MessageBox.Show("Pracownik " + imieMechanika + " " + nazwiskoMechanika + " nie istnieje", "Błąd !", MessageBoxButton.OK, MessageBoxImage.Error);
                    ImieMechanika.Clear();
                    NazwiskoMechanika.Clear();
                }


                await newConnection.SaveChangesAsync();
            }

            ramkaKolor();

        }

        /// <summary>
        /// Jeśli podany numer rejestracyjny pojazdu istnieje (zapisany jest w bazie) to metoda uzupełnia cały formularz danymi z bazy
        /// </summary>
        private async void DataToForm()
        {

            using (databaseConnection newConnection = new databaseConnection())
            {
                Naprawa DefaultNaprawa = new Naprawa();
                Pojazd DefaultPojazd = new Pojazd();
                Klient DefaultKlient = new Klient();
                Adres DefaultAdres = new Adres();

                string nr = Numer_rejestracyjny.Text;
                if (await Task.Run(() => !newConnection.Pojazdy.Any(p => p.Numer_rejestracyjny == nr)))
                {
                    // MessageBox.Show("NIE MA TAKIEGO NUMERU");
                    //ClearForm();
                    PojazdTextBoxInaccessible(true);
                    WlascicielTextBoxInaccessible(true);
                    IfPojazdExist = false;
                }


                else
                {
                    IfPojazdExist = true;

                    await Task.Run(() => DefaultPojazd = newConnection.Pojazdy.Single<Pojazd>(p => p.Numer_rejestracyjny == nr));
                    await Task.Run(() => DefaultKlient = newConnection.Klienci.Single<Klient>(k => k.ID_Klient == DefaultPojazd.ID_Klient));
                    await Task.Run(() => DefaultAdres = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == DefaultKlient.ID_Adres));

                    Marka.Text = DefaultPojazd.Marka;
                    Model.Text = DefaultPojazd.Model;
                    Numer_VIN.Text = DefaultPojazd.Numer_VIN;
                    Rok_produkcji.Text = DefaultPojazd.Rok_produkcji.ToString();
                    Typ_paliwa.Text = DefaultPojazd.Typ_paliwa;
                
                    imie.Text = DefaultKlient.Imie;
                    nazwisko.Text = DefaultKlient.Nazwisko;
                    telefon.Text = DefaultKlient.Telefon.ToString();
         
                    miejscowosc.Text = DefaultAdres.Miejscowosc;
                    ulica.Text = DefaultAdres.Ulica;
                    numer.Text = DefaultAdres.Numer;
                    kod_pocztowy.Text = DefaultAdres.Kod_pocztowy;
               }
            }
        }

        /// <summary>
        /// Wyczyszczenie Formularza
        /// </summary>
        private void ClearForm()
        {
            // Numer_rejestracyjny_input.Clear();
            Marka.Clear();
            Model.Clear();
            Numer_VIN.Clear();
            Rok_produkcji.Clear();
            Typ_paliwa.Clear();
            imie.Clear();
            nazwisko.Clear();
            ImieMechanika.Clear();
            NazwiskoMechanika.Clear();
            ulica.Clear();
            miejscowosc.Clear();
            telefon.Clear();
            numer.Clear();
            kod_pocztowy.Clear();
            Opis_usterek.Clear();
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
            validation.colorRestore(Numer_rejestracyjny);
            validation.colorRestore(Numer_VIN);
            validation.colorRestore(Marka);
            validation.colorRestore(Model);
            validation.colorRestore(Rok_produkcji);
            validation.colorRestore(Typ_paliwa);
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
}
