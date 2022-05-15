using System;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using BibliotekaKlas;

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

            if (IfDataExists())
             DataToForm(); 
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IfDataExists())
                Modyfikuj();
            else
                InsertData();
        }

        private async void InsertData()
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

        private async void DataToForm()
        {
            //dodac zabezpieczenie gdy nie ma danych w bazie 

            using (databaseConnection newConnection = new databaseConnection())
            {

                Warsztat DefaultWarsztat = new Warsztat();
                Adres DefaultAdresWarsztatu = new Adres();

                await Task.Run(() => DefaultWarsztat = newConnection.Warsztaty.FirstOrDefault<Warsztat>());
                await Task.Run(() => DefaultAdresWarsztatu = newConnection.Adresy.Single<Adres>(a => a.ID_Adres == DefaultWarsztat.ID_Adres));

                nazwa.Text = DefaultWarsztat.Nazwa;
                telefon.Text = Convert.ToString(DefaultWarsztat.Telefon);
                nip.Text = DefaultWarsztat.NIP;
                numer_konta.Text = DefaultWarsztat.Numer_konta_bankowego;
                nazwa_banku.Text = DefaultWarsztat.Nazwa_banku;

                miejscowosc.Text = DefaultAdresWarsztatu.Miejscowosc;
                ulica.Text = DefaultAdresWarsztatu.Ulica;
                numer.Text = DefaultAdresWarsztatu.Numer;
                kod_pocztowy.Text = DefaultAdresWarsztatu.Kod_pocztowy;

            }
        }

        private bool IfDataExists()
        {
            using (databaseConnection newConnection = new databaseConnection())
            {
                if (newConnection.Warsztaty.Any(w => w.Nazwa != ""))
                    return true;

                else
                    return false;
            }


        }

        private async void Modyfikuj()
        {
            using (databaseConnection newConnection = new databaseConnection())
            {

                Warsztat modyfikacjaWarsztat = await Task.Run(() => { return newConnection.Warsztaty.FirstOrDefault(); });
                Adres modyfikacjaAdres = await Task.Run(() => { return newConnection.Adresy.Single<Adres>(p => p.ID_Adres == modyfikacjaWarsztat.ID_Adres); });

                this.Dispatcher.Invoke(() =>
                {
                    modyfikacjaWarsztat.Nazwa = nazwa.Text;
                    modyfikacjaWarsztat.Telefon = Convert.ToInt32(telefon.Text);
                    modyfikacjaWarsztat.NIP = nip.Text;
                    modyfikacjaWarsztat.Numer_konta_bankowego = numer_konta.Text;
                    modyfikacjaWarsztat.Nazwa_banku = nazwa_banku.Text;

                    //Dane adresowe
                    modyfikacjaAdres.Miejscowosc = miejscowosc.Text;
                    modyfikacjaAdres.Ulica = ulica.Text;
                    modyfikacjaAdres.Numer = numer.Text;
                    modyfikacjaAdres.Kod_pocztowy = kod_pocztowy.Text;


                });
                newConnection.SaveChanges();
            }

        }
    }
}
