using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotekaKlas;

namespace WarsztatV2.SMS
{
    internal class SendSMS
    {
        private static string log;
        private static string pas;

        public static async void GetDataFromFile()
        {
            string[] lines = System.IO.File.ReadAllLines("config.txt");

            if (lines.Length >= 2)
            {
                log = lines[2];
                pas = lines[3];
            }
            else
            {
                log = "unknown";
                pas = "unknown";
            }
        }

        public static async void SendToClient(int IdNaprawy)
        {

            GetDataFromFile();

            Warsztat WarsztatFav = new Warsztat();
            Naprawa NaprawaFav = new Naprawa();
            Pojazd PojazdFav = new Pojazd();
            Klient KlientFav = new Klient();

            // pobranie danych z bazy potrzebnych do wyslania sms
            using (databaseConnection newConnection = new databaseConnection())
            {

                WarsztatFav = await Task.Run(() => { return newConnection.Warsztaty.Single<Warsztat>(a => a.ID_Warsztat == 1); });
                NaprawaFav = await Task.Run(() => { return newConnection.Naprawy.Single<Naprawa>(a => a.ID_Naprawa == IdNaprawy); });
                PojazdFav = await Task.Run(() => { return newConnection.Pojazdy.Single<Pojazd>(p => p.Numer_rejestracyjny == NaprawaFav.Numer_rejestracyjny); });
                KlientFav = await Task.Run(() => { return newConnection.Klienci.Single<Klient>(a => a.ID_Klient == PojazdFav.ID_Klient); });
            }

            //numer telefonu na ktory zostanie wyslana wiadomosc sms
            string NumerTelKlienta = "48" + KlientFav.Telefon.ToString();


            // Utworzenie obiektu Web Services
            GSMServiceAPI.GSMServicePortTypeClient ws = new GSMServiceAPI.GSMServicePortTypeClient();

            // Obiekt Account - zawiera login oraz haslo do konta API
            GSMServiceAPI.Account account = new GSMServiceAPI.Account();
            account.login = log; // Login subkonta API
            account.pass = pas; // Hasło subkonta API

            //   account.login = "projektwarsztat"; // Login subkonta API
            //  account.pass = "Q@wertyuiop!1"; // Hasło subkonta API


            // Wysyłanie SMS:

            // Tablica obiektów Message - każdy element zawiera jedną wiadomość SMS, ktora ma zostać wyslana.
            GSMServiceAPI.Message[] messages = new GSMServiceAPI.Message[1];
            messages[0] = new GSMServiceAPI.Message();
            messages[0].recipients = new string[] { "48" + KlientFav.Telefon.ToString() }; // Numery telefonów odbiorców danej wiadomości
            messages[0].message = WarsztatFav.Nazwa + " zaprasza po odbior samochodu o numerze rej. " + NaprawaFav.Numer_rejestracyjny + "\nDo zobaczenia."; // Tresc wiadomosci
            messages[0].sender = "BRAMKA SMS"; // Pole nadawcy, z ktorym ma zostac wyslany SMS (uprzednio zdefiniowane na koncie Uzytkownika)
            messages[0].msgType = 1; // Typ wiadomosci SMS: 1 - Tradycyjny SMS, 2 - Flash SMS, 3 - SMS ekonomiczny
            messages[0].unicode = false; // Czy wiadomosc ma byc wyslana w kodowaniu UNICODE?
            messages[0].sandbox = false; // Czy tryb testowy?

            // Wysyłanie SMS - wywolanie metody SendSMS
            GSMServiceAPI.SendSMSReturn[] sendResult = ws.SendSMS(account, messages);

        }

    }
}
