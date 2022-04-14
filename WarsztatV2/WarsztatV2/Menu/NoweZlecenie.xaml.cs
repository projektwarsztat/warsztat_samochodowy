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
            Numer_VIM.IsEnabled = value;
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
            KodPocztowy.IsEnabled = value;
            Telefon.IsEnabled = value;

        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (Numer_rejestracyjny.Text.Length == 7)
            {
                //sprawdz w bazie czy istnieje taki numer
                //jesli tak to wypisz dane do formularzy
                //jesli nie to umozliwij wprowadzanie danych i dodaj do tabeli samochody i klienci odpowiednie dane

                PojazdTextBoxInaccessible(true);
                WlascicielTextBoxInaccessible(true);
            }

           
        }
    }
}
