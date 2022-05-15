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
    /// Interaction logic for NoweZlecenie.xaml
    /// </summary>
    public partial class NoweZlecenie : Page
    {
        private bool IfPojazdExist = false;

        public NoweZlecenie()
        {
            InitializeComponent();

            //pola w formularzu poza nr_rejestracyjny domyslnie niedostepne
            PojazdTextBoxInaccessible(false);
            WlascicielTextBoxInaccessible(false);
        }

        public void PojazdTextBoxInaccessible(bool value)
        {
            Marka.IsEnabled = value;
            Model.IsEnabled = value;
            Numer_VIN.IsEnabled = value;
            Rok_produkcji.IsEnabled = value;
            Typ_paliwa.IsEnabled = value;

        }

        public void WlascicielTextBoxInaccessible(bool value)
        {
            Imie.IsEnabled = value;
            Nazwisko.IsEnabled = value;
            Miejscowosc.IsEnabled = value;
            Ulica.IsEnabled = value;
            Numer.IsEnabled = value;
            Kod_pocztowy.IsEnabled = value;
            Telefon.IsEnabled = value;

        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (Numer_rejestracyjny_input.Text.Length == 7)
            {
                //sprawdz w bazie czy istnieje taki numer
                //jesli tak to wypisz dane do formularzy
                //jesli nie to umozliwij wprowadzanie danych i dodaj do tabeli samochody i klienci odpowiednie dane
                DataToForm();
                // PojazdTextBoxInaccessible(true);
                // WlascicielTextBoxInaccessible(true);
            }

            if (Numer_rejestracyjny_input.Text.Length < 7)
            {
                ClearForm();

                PojazdTextBoxInaccessible(false);
                WlascicielTextBoxInaccessible(false);
            }



        }

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

        async private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                //Dane pojazdu
                string numer_rejestracyjny = Numer_rejestracyjny_input.Text;
                string marka = Marka.Text;
                string model = Model.Text;
                string numer_VIN = Numer_VIN.Text;
                int rok_produkcji = Convert.ToInt32(Rok_produkcji.Text);
                string typ_paliwa = Typ_paliwa.Text;

                //Dane wlasciciela pojazdu
                string imieWlasciciela = Imie.Text;
                string nazwiskoWlasciciela = Nazwisko.Text;
                int numerTelefonu = Convert.ToInt32(Telefon.Text);

                //Dane adresowe
                string nazwaMiejscowosci = Miejscowosc.Text;
                string nazwaUlicy = Ulica.Text;
                string numerBudynku = Numer.Text;
                string numerKoduPocztowego = Kod_pocztowy.Text;

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
                        Numer_rejestracyjny_input.Clear();

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
                        Numer_rejestracyjny_input.Clear();
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

        }

        private async void DataToForm()
        {

            using (databaseConnection newConnection = new databaseConnection())
            {
                Naprawa DefaultNaprawa = new Naprawa();
                Pojazd DefaultPojazd = new Pojazd();
                Klient DefaultKlient = new Klient();
                Adres DefaultAdres = new Adres();

                string nr = Numer_rejestracyjny_input.Text;
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
                    Marka.Text = DefaultPojazd.Marka;
                    Model.Text = DefaultPojazd.Model;
                    Numer_VIN.Text = DefaultPojazd.Numer_VIN;
                    Rok_produkcji.Text = DefaultPojazd.Rok_produkcji.ToString();
                    Typ_paliwa.Text = DefaultPojazd.Typ_paliwa;

                    await Task.Run(() => DefaultKlient = newConnection.Klienci.Single<Klient>(k => k.ID_Klient == DefaultPojazd.ID_Klient));
                    Imie.Text = DefaultKlient.Imie;
                    Nazwisko.Text = DefaultKlient.Nazwisko;
                    Telefon.Text = DefaultKlient.Telefon.ToString();

                    await Task.Run(() => DefaultAdres = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == DefaultKlient.ID_Adres));
                    Miejscowosc.Text = DefaultAdres.Miejscowosc;
                    Ulica.Text = DefaultAdres.Ulica;
                    Numer.Text = DefaultAdres.Numer;
                    Kod_pocztowy.Text = DefaultAdres.Kod_pocztowy;

                }

            }
        }

        private void ClearForm()
        {
            // Numer_rejestracyjny_input.Clear();
            Marka.Clear();
            Model.Clear();
            Numer_VIN.Clear();
            Rok_produkcji.Clear();
            Typ_paliwa.Clear();
            Imie.Clear();
            Nazwisko.Clear();
            ImieMechanika.Clear();
            NazwiskoMechanika.Clear();
            Ulica.Clear();
            Miejscowosc.Clear();
            Telefon.Clear();
            Numer.Clear();
            Kod_pocztowy.Clear();
            Opis_usterek.Clear();
        }
    }
}
