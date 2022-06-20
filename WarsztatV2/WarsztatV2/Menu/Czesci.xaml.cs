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
using System.Threading;
using System.ComponentModel;
using WarsztatV2.Faktury;
using BibliotekaKlas;
using System.Text.RegularExpressions;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Klasa zawierająca implementację zakładki Cześci 
    /// </summary>
    public partial class Czesci : Page
    {

        private int CzescID { get; set; } //Własność używana do przechowywania indeksu edytowanego adresu
        private List<DaneCzesc> wynikL = new List<DaneCzesc>(); // Lista elementów - do wyświetlenia w ListBoxie
        Validation validation = new Validation();
        public Czesci()
        {
            InitializeComponent();
            zmienDostepnosc(false);
            pobieranieDanych();
            CzescID = -1;
        }


        /// <summary>
        /// Metoda pobierająca dane części i użytych części z bazy danych, wypełnienie ListViewBox
        /// </summary>
        private async void pobieranieDanych()
        {

            List<Czesc> czescL;
            List<Uzyte_czesci> uzyteL;

            using (databaseConnection newConnection = new databaseConnection())
            {

                czescL = await Task.Run(() => { return newConnection.Czesci.ToList<Czesc>(); });
                uzyteL = await Task.Run(() => { return newConnection.Uzyte_czesci.ToList<Uzyte_czesci>(); });
                for (int i = 0; i < czescL.Count; i++) //Łączenie klientów ze swoimi adresami
                {

                    wynikL.Add(new DaneCzesc { ID_Czesc = czescL[i].ID_Czesci, Nazwa = czescL[i].Nazwa, Cena = czescL[i].Cena, CzyWUzyciu = CzyWUzyciu(uzyteL, czescL[i].ID_Czesci) });

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
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ID_Czesc");

            view.GroupDescriptions.Add(groupDescription);
            view.SortDescriptions.Add(new SortDescription("ID_Czesc", ListSortDirection.Ascending));
            view.Filter = czescFilter;
        }

        /// <summary>
        /// Metoda zwracająca prawdę dla rekordów, które spełniają tę własność, że wpisany ciąg znaków do searchBara jest w dowolnym stopniu podobny do danych z kolumny nazwaCzesci
        /// </summary>
        private bool czescFilter(object item) 
        {
            if (String.IsNullOrEmpty(searchTextBox.Text)) return true;
            else
            {

                return ((item as DaneCzesc).Nazwa.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            }
        }

        /// <summary>
        /// Wyszukiwanie w ListVievBox frazy z pola formularza 
        /// </summary>
        private void searchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }

        /// <summary>
        /// Obsługa klikania w ListView zawierający listę części
        /// </summary>
        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                DaneCzesc czesc = (DaneCzesc)element.Content;
                if (czesc != null)
                {
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(
                            () => {
                                NazwaCzesci.Text = czesc.Nazwa;
                                Cena.Text = czesc.Cena.ToString();
                                if (czesc.CzyWUzyciu) usun.IsEnabled = false;
                                else usun.IsEnabled = true;

                            }
                        );
                    });
                    CzescID = czesc.ID_Czesc;
                    ramkaKolor();
                }
            }
        }

        /// <summary>
        /// Metoda dodająca dane do bazy danych, wykonująca się po kliknięciu przycisku
        /// </summary>
        private async void dodajClick(object sender, RoutedEventArgs e)
        {
            string NazwaCzesciD = NazwaCzesci.Text;
            string CenaD = Cena.Text;


            if (NazwaCzesciD.Length != 0 && CenaD.Length != 0)
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
                            newConnection.Czesci.Add(
                                new Czesc
                                {
                                    Nazwa = NazwaCzesciD,
                                    Cena = Convert.ToDouble(CenaD),

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
                ramkaKolor();

            }
            else
            {
                MessageBox.Show("Nie wszystkie pola formularza zostały wypełnione!", "Puste pola formularza", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Metoda usuwająca dane (pod warunkiem, że nie są w użyciu) z bazy danych, wykonująca się po kliknięciu przycisku
        /// </summary>
        private async void usunClick(object sender, RoutedEventArgs e)
        {
            if (CzescID != -1)
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
                        newConnection.Czesci.Remove(newConnection.Czesci.Single<Czesc>(k => k.ID_Czesci == CzescID));
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
                ramkaKolor();

                CzescID = -1;
            }
        }

        /// <summary>
        /// Metoda modyfikująca dane w bazie danych, wykonująca się po kliknięciu przycisku
        /// </summary>
        private async void modyfikujClick(object sender, RoutedEventArgs e)
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

                    Czesc czescM = newConnection.Czesci.Single<Czesc>(a => a.ID_Czesci == CzescID);
                    this.Dispatcher.Invoke(
                        () =>
                        {
                            czescM.Nazwa = NazwaCzesci.Text;
                            czescM.Cena = Convert.ToDouble(Cena.Text);
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
            ramkaKolor();


        }

        /// <summary>
        /// Metoda zmieniająca dostępność formularza i przycisków
        /// </summary>
        private void zmienDostepnosc(bool wartosc)
        {
            NazwaCzesci.IsEnabled = wartosc;
            Cena.IsEnabled = wartosc;
            dodaj.IsEnabled = wartosc;
            modyfikuj.IsEnabled = wartosc;
            usun.IsEnabled = wartosc;
            lvDataBinding.IsEnabled = wartosc;
        }

        /// <summary>
        /// Metoda czyszcząca zawartość formularza
        /// </summary>
        private void wyczyscZawartosc()
        {
            NazwaCzesci.Clear();
            Cena.Clear();

        }

        /// <summary>
        /// Metoda sprawdza czy dana część jest w użyciu
        /// </summary>
        private bool CzyWUzyciu(List<Uzyte_czesci> listaUzytychCzesci, int indeksIdCzesci)
        {
            for (int i = 0; i < listaUzytychCzesci.Count; i++)
            {
                if (listaUzytychCzesci[i].ID_Czesci == indeksIdCzesci) return true;
            }
            return false;
        }


        
        /// <summary>
        /// Klasa pojedynczego wiersza w ListView (potrzebna, by poprawnie generować odpowiednie wiersze)
        /// </summary>
        public class DaneCzesc
        {
            public int ID_Czesc { get; set; }
            public string Nazwa { get; set; }
            public double Cena { get; set; }
            public bool CzyWUzyciu { get; set; }

        }

        /// <summary>
        /// Metoda wywołująca wczytywanie faktur po klikniecu przycisku
        /// </summary>
        private void WczytajFakure_Click(object sender, RoutedEventArgs e)
        {
            OdczywywanieCzesciZFaktur.ReadPDF();

            wynikL.Clear(); //Wyczyszczenie i załadowanie danych na nowo
            pobieranieDanych();


        }

        /// <summary>
        /// Metoda ustawiający pierwotny kolor obramowania TextBoxów
        /// </summary>
        private void ramkaKolor()
        {
            validation.colorRestore(NazwaCzesci);
            validation.colorRestore(Cena);
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
