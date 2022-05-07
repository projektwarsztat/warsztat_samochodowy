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
using WarsztatV2.Tables;
using WarsztatV2.Faktury;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for DoOdbioru.xaml
    /// </summary>
    public partial class HistoriaZlecen : Page
    {
        private int PracownikID { get; set; }
        private int NaprawaID { get; set; }

        public HistoriaZlecen()
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
                            if (naprawaL[i].ID_Pracownik == pracownikL[j].ID_Pracownik && naprawaL[i].Numer_rejestracyjny == pojazdL[k].Numer_rejestracyjny && naprawaL[i].Status_naprawy == "Wydany")
                            {
                                wynikL.Add(new DaneNaprawa
                                {
                                    Imie = pracownikL[j].Imie,
                                    Nazwisko = pracownikL[j].Nazwisko,
                                    Data_przyjecia = naprawaL[i].Data_przyjecia,
                                    Data_wydania = naprawaL[i].Data_wydania,
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
            public DateTime? Data_wydania { get; set; }
            public string Status_naprawy { get; set; }
            public string Opis_usterek { get; set; }
            public string Wiadomosc_zwrotna { get; set; }

        }






    }
}
