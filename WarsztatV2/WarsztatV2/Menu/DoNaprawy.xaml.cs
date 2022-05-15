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
using System.Collections.ObjectModel;
using System.Diagnostics;
using BibliotekaKlas;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for DoNaprawy.xaml
    /// </summary>
    public partial class DoNaprawy : Page
    {

        private int PracownikID { get; set; }
        private int NaprawaID { get; set; }

        public DoNaprawy()
        {

            InitializeComponent();
            NaprawaID = -1;
            _ = pobierzDaneNaprawy();

        }

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
                            if (naprawaL[i].ID_Pracownik == pracownikL[j].ID_Pracownik && naprawaL[i].Numer_rejestracyjny == pojazdL[k].Numer_rejestracyjny && naprawaL[i].Status_naprawy == "Przyjety")
                            {
                                wynikL.Add(new DaneNaprawa
                                {
                                    Imie = pracownikL[j].Imie,
                                    Nazwisko = pracownikL[j].Nazwisko,
                                    Data_przyjecia = naprawaL[i].Data_przyjecia,
                                    Numer_rejestracyjny = naprawaL[i].Numer_rejestracyjny,
                                    MarkaModel = pojazdL[k].Marka + " " + pojazdL[k].Model,
                                    Opis_usterek = naprawaL[i].Opis_usterek,
                                    ID_Naprawa = naprawaL[i].ID_Naprawa
                                }); ;
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
                            ImieMechanika.Text = naprawa.Imie;
                            NazwiskoMechanika.Text = naprawa.Nazwisko;
                        }
                    );
                    //KlientID = pojazd.ID_Klient;
                    //AdresID = pojazd.ID_Adres;
                    //NumerRejestracyjny = naprawa.Numer_rejestracyjny;
                    NaprawaID = naprawa.ID_Naprawa;
                    //Trace.WriteLine("nap: " + NaprawaID);
                });
            }
        }


        private async void modyfikujClick(object sender, RoutedEventArgs e)
        {
            if (NaprawaID != -1 && IfMechanikExists())
            {
                string Im = ImieMechanika.Text, Nm = NazwiskoMechanika.Text;
                using (databaseConnection newConnection = new databaseConnection())
                {
                    Naprawa naprawaModyfikacja = await Task.Run(() => { return newConnection.Naprawy.Single<Naprawa>(p => p.ID_Naprawa == NaprawaID); });
                    Pracownik pracownikModyfikacja = await Task.Run(() => { return newConnection.Pracownicy.Single<Pracownik>(p => p.Imie == Im && p.Nazwisko == Nm); });
                    this.Dispatcher.Invoke(() =>
                    {
                        naprawaModyfikacja.Opis_usterek = Opis_usterek.Text;
                        naprawaModyfikacja.ID_Pracownik = pracownikModyfikacja.ID_Pracownik;

                    });
                    newConnection.SaveChanges();
                }

                _ = pobierzDaneNaprawy();
            }
            else
            {
                MessageBox.Show("Pracownik " + ImieMechanika.Text + " " + NazwiskoMechanika.Text + " nie istnieje", "Błąd !", MessageBoxButton.OK, MessageBoxImage.Error);
                ImieMechanika.Clear();
                NazwiskoMechanika.Clear();
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

            // public string Mechanik { get; set; }
            public string Imie { get; set; }

            public string Nazwisko { get; set; }

            public int ID_Pracownik { get; set; }
            public DateTime? Data_przyjecia { get; set; }

            public string Status_naprawy { get; set; }
            public string Opis_usterek { get; set; }



        }
    }
}
