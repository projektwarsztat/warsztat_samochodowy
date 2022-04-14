using System;
using System.Text;
using System.Linq;
using System.Windows;
using WarsztatV2.Tables;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                //Dane warsztatu
                string nazwaWarsztatu = nazwa.Text;
                int numerTelefonuInt = Convert.ToInt32(telefon.Text);
                string numerNIP = nip.Text;
                string numerKonta = numer_konta.Text;
                string nazwaBanku = nazwa_banku.Text;

                //Dane adresowe
                string nazwaMiejscowosci = miejscowosc.Text;
                string nazwaUlicy = ulica.Text;
                string numerBudynku = numer.Text;
                string numerKoduPocztowego = kod_pocztowy.Text;

                await Task.Run(() => newConnection.Warsztaty.Add(
                    new Warsztat
                    {
                        Nazwa = nazwaWarsztatu,
                        Telefon = numerTelefonuInt,
                        NIP = numerNIP,
                        Numer_konta_bankowego = numerKonta,
                        Nazwa_banku = nazwaBanku,
                        AdresNav = new Adres
                        {
                            Ulica = nazwaUlicy,
                            Numer = numerBudynku,
                            Kod_pocztowy = numerKoduPocztowego,
                            Miejscowosc = nazwaMiejscowosci
                        }
                    })
                );
                await newConnection.SaveChangesAsync();
            }
        }
    }
}
