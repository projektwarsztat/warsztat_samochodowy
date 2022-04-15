using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using WarsztatV2.Tables;

namespace WarsztatV2.Menu
{
    /// <summary>
    /// Interaction logic for Klienci.xaml
    /// </summary>
    public partial class Klienci : Page
    {
        private int KlientID { get; set; }
        private int AdresID { get; set; }
        public Klienci()
        {
            InitializeComponent();
            KlientID = -1;
            AdresID = -1;
            _ = pobierzDaneKlientow();
        }

        private async Task pobierzDaneKlientow()
        {
            List<Klient> klientL;
            List<Adres> adresL;
            ObservableCollection<DaneKlient> wynikL = new ObservableCollection<DaneKlient>();
            using (databaseConnection newConnection = new databaseConnection())
            {
                klientL = await Task.Run(() => { return newConnection.Klienci.ToList<Klient>(); });
                adresL = await Task.Run(() => { return newConnection.Adresy.ToList<Adres>(); });
                for (int i = 0; i < klientL.Count; i++)
                {
                    for (int j = 0; j < adresL.Count; j++)
                    {
                        if (klientL[i].ID_Adres == adresL[j].ID_Adres)
                        {
                            wynikL.Add(new DaneKlient { ID_Klient = klientL[i].ID_Klient, Imie = klientL[i].Imie, Nazwisko = klientL[i].Nazwisko, Telefon = klientL[i].Telefon, ID_Adres = klientL[i].ID_Adres, Ulica = adresL[j].Ulica, Numer = adresL[j].Numer, Kod_pocztowy = adresL[j].Kod_pocztowy, Miejscowosc = adresL[j].Miejscowosc });
                        }
                    }
                }
                Thread updateInfoLAndSearchBT = new Thread(() => this.Dispatcher.Invoke(() => { informationLabel.Visibility = Visibility.Hidden; searchTextBox.Visibility = Visibility.Visible; }));
                updateInfoLAndSearchBT.Start();
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

        private void searchTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
        }

        private async void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem element = sender as ListViewItem;
            if (element != null && !element.IsSelected)
            {
                DaneKlient klient = (DaneKlient)element.Content;
                await Task.Run(() =>
                {
                    this.Dispatcher
                        .Invoke(() => {
                            imie.Text = klient.Imie;
                            nazwisko.Text = klient.Nazwisko;
                            miejscowosc.Text = klient.Miejscowosc;
                            ulica.Text = klient.Ulica;
                            numer.Text = klient.Numer;
                            kod_pocztowy.Text = klient.Kod_pocztowy;
                            telefon.Text = klient.Telefon.ToString();
                        }
                    );
                    KlientID = klient.ID_Klient;
                    AdresID = klient.ID_Adres;
                });
            }
        }

        private async void modyfikujClick(object sender, RoutedEventArgs e)
        {
            if (KlientID != -1 && AdresID != -1)
            {
                await Task.Run(() =>
                {
                    using (databaseConnection newConnection = new databaseConnection())
                    {
                        Klient klientModyfikacja = newConnection.Klienci.Single<Klient>(k => k.ID_Klient == KlientID);
                        Adres adresModyfikacja = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == AdresID);
                        this.Dispatcher.Invoke(() =>
                        {
                            klientModyfikacja.Imie = imie.Text;
                            klientModyfikacja.Nazwisko = nazwisko.Text;
                            klientModyfikacja.Telefon = Convert.ToInt32(telefon.Text);
                            adresModyfikacja.Miejscowosc = miejscowosc.Text;
                            adresModyfikacja.Ulica = ulica.Text;
                            adresModyfikacja.Numer = numer.Text;
                            adresModyfikacja.Kod_pocztowy = kod_pocztowy.Text;
                        });
                        newConnection.SaveChanges();
                    }
                });
            }
        }
    }

    //Struktura pojedynczego wiersza w ListView
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
    }
}
