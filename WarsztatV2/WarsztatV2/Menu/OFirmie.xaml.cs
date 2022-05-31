using System;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using BibliotekaKlas;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarsztatV2
{
    /// <summary>
    /// Klasa zawierająca implementację zakładki O Firmie: dodawanie danych firmy, danych do logowania (Bramka sms), wartości podatku VAT, ścieżki do zapisu faktur
    /// </summary>
    public partial class OFirmie : Page
    {
        public OFirmie()
        {
            InitializeComponent();

            if (IfDataExists())
                DataToForm();

            if (IfDataNotNull())
                WriteDataToForm();
        }

        /// <summary>
        /// Sprawdzenie czy dane w lewym formularzu istnieją jeśli tak to nastepuje modyfikacja, jeśli nie wstawiane są nowe dane
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IfDataExists())
                Modyfikuj();
            else
                InsertData();
        }

        /// <summary>
        /// Wstawianie danych o warsztacie do bazy
        /// </summary>
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

        /// <summary>
        /// Wypisywanie danych z bazy do formularza
        /// </summary>
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

        /// <summary>
        /// Sprawdzenie czy dane istnieją w bazie
        /// </summary>
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

        /// <summary>
        /// Modyfikacja danych warsztatu w bazie
        /// </summary>
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

        /// <summary>
        /// Zapisanie danych z prawego formularza do pliku config.txt
        /// </summary>
        private void SaveDataToFIle()
        {

            using (StreamWriter writer = new StreamWriter("config.txt"))
            {
                writer.WriteLine(sciezkafaktury.Text);
                writer.WriteLine(stawkaVat.Text);
                writer.WriteLine(loginGsmservice.Text);
                writer.WriteLine(hasloGsmservice.Password);

            }

        }

        /// <summary>
        /// Sprawdzenie czy dane w pliku config.txt nie są puste
        /// </summary>
        private bool IfDataNotNull()
        {
            string[] lines = System.IO.File.ReadAllLines("config.txt");

            if (lines.Length == 4)
                return true;
            else return false;

        }

        /// <summary>
        /// Wypisanie danych z pliku config.txt do formularza
        /// </summary>
        private void WriteDataToForm()
        {
            string[] lines = System.IO.File.ReadAllLines("config.txt");

            sciezkafaktury.Text = lines[0];
            stawkaVat.Text = lines[1];
            loginGsmservice.Text = lines[2];
            hasloGsmservice.Password = lines[3];

        }

        /// <summary>
        /// Wywołanie zapisania danych do pliku 
        /// </summary>
        private void SaveButtonFile_Click(object sender, RoutedEventArgs e)
        {
            SaveDataToFIle();
        }

        /// <summary>
        /// Wywołanie okna dialogowego do wyboru folderu (gdzie zapisywane będą faktury)
        /// </summary>
        private void sciezkafaktury_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if(openFileDialog.ShowDialog()==true)
            //if (openFileDialog.FileName != " ")
            //    sciezkafaktury.Text = openFileDialog.FileName;

            var fd = new System.Windows.Forms.FolderBrowserDialog();
            var czyWybrano = fd.ShowDialog();
            if (czyWybrano == System.Windows.Forms.DialogResult.OK)
                sciezkafaktury.Text = fd.SelectedPath;


        }
    }
}
