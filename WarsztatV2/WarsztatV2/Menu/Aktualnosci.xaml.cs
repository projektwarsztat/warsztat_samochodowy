using BibliotekaKlas;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for Aktualnosci.xaml
    /// </summary>
    public partial class Aktualnosci : Page
    { 
        public Aktualnosci()
        {
            InitializeComponent();

            //Zakrycie opisów liczb na czas pobierania danych
            readyStatusLabel.Visibility = Visibility.Hidden;
            repairStatusLabel.Visibility = Visibility.Hidden;
            allVehicleLabel.Visibility = Visibility.Hidden;

            //Uruchonienie metody wypełniającej informacje o liczbie napraw w przeciągu tygodnia
            Task.Run(() => ilePojazdowNaTydzien());

            //Wywołanie metody wypełniającej informacji w postaci ilości konkretnych zjawisk
            Task.Run(() => pojazdyWLiczbach());

            //Wywołanie metody wypełniającej informacji w postaci danych pracownika, który wykonał najwięcej napraw rozumianego jako najlepszego
            Task.Run(() => najwiecejPracujacyPracownik());

            //Wywołanie metody wypisującej informacje w postaci numeru rejestracyjnego, modelu pojazdu oraz przyczyny ostatniej wizyty, dla pojazdu który najczęściej się pojawia we warsztacie
            Task.Run(() => najczesciejWizytujacyPojazd());

            //Wywołanie metody wypisującej informację o średnim czasie spędzonym przez pojazd we warsztacie, od czasu zgłoszenia do czasu odebrania pojazdu przez klienta
            Task.Run(() => sredniCzasPobytu());
        }

        /// <summary>
        /// Metoda wypełniająca pierwsze subokienko w panelu informacyjnym danymi na temat, ile pojazdów zostało przyjętych do warsztatu w ciągu siedmiu dni.
        /// </summary>
        private async void ilePojazdowNaTydzien()
        {
            List<Naprawa> naprawy;
            DateTime dzis = DateTime.Now;
            string[] dniTygodnia = new string[7];
            int[] liczbaPojazdow = new int[7] { 0, 0, 0, 0, 0, 0, 0};
            bool[] czyWeekend = new bool[7] { false, false, false, false, false, false, false};

            using (databaseConnection newConnection = new databaseConnection())
            {
                naprawy = await Task.Run(
                    () => newConnection.Naprawy.ToList<Naprawa>() //Pobranie danych o naprawach
                );
            }

            for (int i = 0; i < naprawy.Count; i++)
            {
                TimeSpan interwal = dzis - (DateTime)naprawy[i].Data_przyjecia;
                int indeks = interwal.Days;
                if(indeks >= 0 && indeks <= 6)
                {
                    liczbaPojazdow[indeks]++;
                }
            }

            for(int i = 0; i < 7; i++)
            {
                dniTygodnia[i] = String.Format(new CultureInfo("pl-pl"), "{0:d ddd}", DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0))); //Odejmowanie i dni od bieżącej daty, aby pobrać skrót dnia tygodnia
                if(dniTygodnia[i].Contains("sob") || dniTygodnia[i].Contains("niedz")) //Jeśli weekend to...
                {
                    czyWeekend[i] = true;
                }
            }

            //Odwrócenie tablic
            Array.Reverse(liczbaPojazdow); 
            Array.Reverse(dniTygodnia);
            Array.Reverse(czyWeekend);

            await Task.Run(
                () =>
                {
                    this.Dispatcher.Invoke(
                        () =>
                            {
                                informationLabel1.Visibility = Visibility.Hidden; //Ukrycie informacji dt. pobierania danych
                                malowanieTekstu(czyWeekend); //Pomalowanie tekstu zgodnie z zasadą - weekend - czerwonawy, pozostałe - szary
                                wypelnijLiczbaPojazdow(liczbaPojazdow); //Wypełnianie liczby pojazdów
                                wypelnijDzienTygodnia(dniTygodnia); //Wypełnianie numeru i nazwy skrótowej dnia tygodnia
                            }
                        );
                }
            );
        }

        /// <summary>
        /// Metoda pobierająca dane z bazy danych na temat liczby: wszystkich samochodów, które kiedykolwiek były we warsztacie, samochodów, które są gotowe go odebrania przez klienta, oraz samochodów, które aktualnie przebywają w naprawie. Następnie dane te umieszcza w odpowiednie miejsca na to przeznaczone.
        /// </summary>
        private async void pojazdyWLiczbach()
        {
            int wNaprawie;
            int doOdbioru;
            int wszystkiePojazdy;

            using (databaseConnection newConnection = new databaseConnection()) //Pobranie danych z tabeli
            {
                wNaprawie = await Task.Run(
                    () => newConnection.Naprawy.Count(n => n.Status_naprawy == "Przyjety") //Pobranie liczby tych napraw, które zostały przyjęte i mogą być naprawiane
                );
                doOdbioru = await Task.Run(
                    () => newConnection.Naprawy.Count(n => n.Status_naprawy == "DoOdbioru") //Pobranie liczby tych napraw, które można odebrać (czekają na wydanie)
                );
                wszystkiePojazdy = await Task.Run(
                    () => newConnection.Naprawy.Count() //Pobranie ilości wszystkich napraw
                );
            }

            //Aktualizacja danych w Panelu Informacyjnym
            await Task.Run(
                () => this.Dispatcher.Invoke(
                    () =>
                    {
                        informationLabel2.Visibility = Visibility.Hidden; //Ukrycie informacji dt. pobierania danych

                        //Pokazanie opisów danych liczbowych 
                        repairStatusLabel.Visibility = Visibility.Visible;
                        readyStatusLabel.Visibility = Visibility.Visible; 
                        allVehicleLabel.Visibility = Visibility.Visible;

                        repairStatusC.Text = wNaprawie.ToString(); //Wstawienie liczby pojazdów w naprawie
                        readyStatusC.Text = doOdbioru.ToString(); //Wstawienie liczby pojazdów gotowych do odbioru
                        allVehicle.Text = wszystkiePojazdy.ToString(); //Wstawienie liczby wszystkich pojazdów
                    }
                )
            );
        }

        ///<summary>
        ///Metoda pobierająca dane z bazy danych na temat pracownika, który wykonał najwięcej napraw. Następnie dane te umieszczane są w stosownym miejscu. W przypadku kiedy wielu pracowników posiada tę samą liczbę napraw, wybrany zostanie ten, którego indeks jest mniejszy.
        /// </summary>
        private async void najwiecejPracujacyPracownik()
        {
            Dictionary<int, int> liczbaNapraw;
            List<Pracownik> pracownik;
            int indeks;

            using (databaseConnection newConnection = new databaseConnection())
            {
                pracownik = await Task.Run(
                    () => newConnection.Pracownicy.ToList<Pracownik>() //Pobranie danych o pracownikach
                );
                liczbaNapraw = await Task.Run( //Grupowanie napraw ze względu na id pracownika
                    () => newConnection.Naprawy.GroupBy(n => n.ID_Pracownik)
                            .Select(s => new {ID = s.Key, Liczba = s.Count()})
                            .ToDictionary(n => n.ID, n => n.Liczba) //Przerzucenie danych do dictionary
                );
            }

            if (liczbaNapraw.Count() != 0)
            {
                indeks = liczbaNapraw.OrderByDescending(lN => lN.Value).First().Key; //Znalezienie odpowiedniego indeksu
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel4.Visibility = Visibility.Hidden; //Ukrycie informacji dt. pobierania danych
                                firstNameW.Text = pracownik.Where(p => p.ID_Pracownik == indeks).First().Imie; //Wstawienie imienia pracownika
                                surnameW.Text = pracownik.Where(p => p.ID_Pracownik == indeks).First().Nazwisko; //Wstawienie nazwiska pracownika
                                repairWorkerC.Text = liczbaNapraw.Where(lN => lN.Key == indeks).First().Value.ToString(); //Wstawienie ilości wykonanych napraw przed danego pracownika
                            }
                        );
                    }
                );
            }
            else
            {
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel4.Content = "Brak danych do wyświetlenia"; //Wyświetlenie informacji o braku danych do wypisania
                            }
                        );
                    }
                );
            }
        }

        ///<summary>
        ///Metoda pobierająca dane z bazy danych na temat pojazdu o największej częstości pojawiania się w naprawie. W przypadku wystąpienia wielu pojazdów o tej samej liczbie napraw zostanie wybrany losowy.
        /// </summary>
        private async void najczesciejWizytujacyPojazd()
        {
            Dictionary<string, int> liczbaNapraw;
            List<Naprawa> naprawy;
            List<Pojazd> pojazdy;
            string numerRejestracyjny;

            using (databaseConnection newConnection = new databaseConnection())
            {
                liczbaNapraw = await Task.Run( //Grupowanie napraw ze względu na id pracownika
                    () => newConnection.Naprawy.GroupBy(n => n.Numer_rejestracyjny)
                            .Select(s => new { ID = s.Key, Liczba = s.Count() }) //Wybieranie nowej jednostki jako pary (numer rejestracyjny, liczba napraw) 
                            .ToDictionary(n => n.ID, n => n.Liczba) //Przerzucenie danych do dictionary
                );
                pojazdy = await Task.Run(
                    () => newConnection.Pojazdy.ToList<Pojazd>() //Pobranie danych o pojazdach 
                );
                naprawy = await Task.Run(
                    () => newConnection.Naprawy.ToList<Naprawa>() //Pobranie danych o naprawach
                );
            }

            if(liczbaNapraw.Count() != 0)
            {
                numerRejestracyjny = liczbaNapraw.OrderByDescending(lN => lN.Value).First().Key; //Znalezienie odpowiedniego numeru rejestracyjnego

                //Zmiana wartości odpowiednich elementów na właściwe treści
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel3.Visibility = Visibility.Hidden; //Ukrycie informacji dt. pobierania danych
                                numberVehicle.Text = numerRejestracyjny; //Wstawienie numeru rejestracyjnego
                                modelVehicle.Text = pojazdy.Where(p => p.Numer_rejestracyjny == numerRejestracyjny).First().Model; //Wstawienie nazwy modelu pojazdu
                            }
                        );
                    }
                );
            }
            else
            {
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel3.Content = "Brak danych do wyświetlenia"; //Wyświetlenie informacji o braku danych do wypisania
                            }
                        );
                    }
                );
            }
            
        }

        ///<summary>
        ///Metoda pobierająca dane z bazy danych na temat średniej długości czasu spędzonego przez pojazd we warsztacie samochodowym. W przypadku braku informacji wyświetla informację o braku danych.
        /// </summary>
        private async void sredniCzasPobytu()
        {
            List<Naprawa> naprawy;

            using (databaseConnection newConnection = new databaseConnection())
            {
                naprawy = await Task.Run(
                    () => newConnection.Naprawy.Where(n => n.Status_naprawy == "Wydany").ToList() //Pobranie danych o zakończonych naprawach
                );
            }

            if (naprawy.Count() != 0)
            {
                TimeSpan sredniCzas = TimeSpan.FromMinutes(naprawy.Select(n => ((TimeSpan)(n.Data_wydania - n.Data_przyjecia)).TotalMinutes)
                    .Sum() / naprawy.Count());

                //Zmiana wartości odpowiednich elementów na właściwe treści
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel5.Visibility = Visibility.Hidden; //Ukrycie informacji dt. pobierania danych
                                averageTime.Text = $"{sredniCzas.Hours}h {sredniCzas.Minutes}min";
                            }
                        );
                    }
                );
            }
            else
            {
                await Task.Run(
                    () =>
                    {
                        this.Dispatcher.Invoke(
                            () =>
                            {
                                informationLabel3.Content = "Brak danych do wyświetlenia"; //Wyświetlenie informacji o braku danych do wypisania
                            }
                        );
                    }
                );
            }

        }

        ///<summary>
        ///Metoda zamalowywująca na odpowiedni kolor liczbę przyjętych pojazdów do warsztatu oraz numer i skrót nazwy dnia tygodnia, jeżeli przyjęcie pojazdu miało miejsce w sobotę lub niedzielę.
        ///</summary>
        private void malowanieTekstu(bool[] cW)
        {
            day1.Foreground = cW[0] ? Brushes.Firebrick : Brushes.DimGray;
            day1n.Foreground = cW[0] ? Brushes.Firebrick : Brushes.DimGray;
            day2.Foreground = cW[1] ? Brushes.Firebrick : Brushes.DimGray;
            day2n.Foreground = cW[1] ? Brushes.Firebrick : Brushes.DimGray;
            day3.Foreground = cW[2] ? Brushes.Firebrick : Brushes.DimGray;
            day3n.Foreground = cW[2] ? Brushes.Firebrick : Brushes.DimGray;
            day4.Foreground = cW[3] ? Brushes.Firebrick : Brushes.DimGray;
            day4n.Foreground = cW[3] ? Brushes.Firebrick : Brushes.DimGray;
            day5.Foreground = cW[4] ? Brushes.Firebrick : Brushes.DimGray;
            day5n.Foreground = cW[4] ? Brushes.Firebrick : Brushes.DimGray;
            day6.Foreground = cW[5] ? Brushes.Firebrick : Brushes.DimGray;
            day6n.Foreground = cW[5] ? Brushes.Firebrick : Brushes.DimGray;
            day7.Foreground = cW[6] ? Brushes.Firebrick : Brushes.DimGray;
            day7n.Foreground = cW[6] ? Brushes.Firebrick : Brushes.DimGray;
        }

        ///<summary>
        ///Metoda wypełniająca w odpowiednie miejsca informacje o liczbie przyjętych pojazdów.
        ///</summary>
        private void wypelnijLiczbaPojazdow(int[] lP)
        {
            day1.Text = lP[0].ToString();
            day2.Text = lP[1].ToString();
            day3.Text = lP[2].ToString();
            day4.Text = lP[3].ToString();
            day5.Text = lP[4].ToString();
            day6.Text = lP[5].ToString();
            day7.Text = lP[6].ToString();
        }

        ///<summary>
        ///Metoda wypełniająca w odpowiednie miejsca informacje o dniu tygodnia.
        ///</summary>
        private void wypelnijDzienTygodnia(string[] dT)
        {
            day1n.Text = dT[0];
            day2n.Text = dT[1];
            day3n.Text = dT[2];
            day4n.Text = dT[3];
            day5n.Text = dT[4];
            day6n.Text = dT[5];
            day7n.Text = dT[6];
        }

    }
}
