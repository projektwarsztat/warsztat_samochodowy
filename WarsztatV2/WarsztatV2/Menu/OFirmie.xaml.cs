using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarsztatV2.Tables;

namespace WarsztatV2
{
    /// <summary>
    /// Interaction logic for OFirmie.xaml
    /// </summary>
    public partial class OFirmie : Page
    {
        public OFirmie()
        {
            InitializeComponent();

            //Dodać wpisywanie informacji o warsztacie z bazy danych do pól formularza, jeśli takie informacje znajdują się w bazie danych  
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                //Obiekt przechowywujący rekord, który przed chwilą dodaliśmy do bazy 
                Adres ostatnioDodanyAdres;

                //Zmienna przechowywująca numer indentyfikatora ostatnio dodanego adresu
                int indexODA;

                //Dodanie do bazy danych nowego adresu
                newConnection.Adresy.Add(new Adres { Miejscowosc = miejscowosc.Text, Ulica = ulica.Text, Numer = numer.Text, Kod_pocztowy = kod_pocztowy.Text });

                //Zapisanie zmian 
                newConnection.SaveChanges();

                //Zapytanie SQL, które zwraca ostatnio wstawiony rekord do bazy
                ostatnioDodanyAdres = newConnection.Adresy.SqlQuery("select * from adres where miejscowosc = \'" + miejscowosc.Text + "\' and ulica = \'" + ulica.Text + "\' and numer = \'" + numer.Text + "\' and kod_pocztowy = \'" + kod_pocztowy.Text + "\'").FirstOrDefault<Adres>();

                //Wydobycie numeru identyfikatora ostatnio dodanego adresu
                indexODA = ostatnioDodanyAdres.ID_Adres;

                //Dodanie do bazy danych nowego rekordu z informacjami o warsztacie                
                newConnection.Warsztat.Add(new Warsztat { Nazwa = nazwa.Text, Adres = indexODA, Telefon = Convert.ToInt32(telefon.Text), NIP = nip.Text, Numer_konta_bankowego = numer_konta.Text, Nazwa_banku = nazwa_banku.Text });

                //Zapisanie zmian 
                newConnection.SaveChanges();
            }
        }
    }
}
