using BibliotekaKlas;
using PasswordCryptography;
using Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WarsztatV2.SMS;

namespace WarsztatV2
{
    /// <summary>
    /// Klasa zajmująca się obsługą połączenia sieciowego
    /// </summary>
    internal class socketConnection
    {
        private Socket serverSocketConnection;
        private EndPoint endPoint;

        public socketConnection()
        {
            serverSocketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Utworzenie gniazda
            endPoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(ConfigurationManager.AppSettings["Port"])); //Utworzenie adresu
        }

        /// <summary>
        /// Metoda zajmująca się połączeniem i wymianą danych z klientem
        /// </summary>
        public void sCConnection()
        {
            serverSocketConnection.Bind(endPoint); //Dowiązanie adresu do gniazda
            serverSocketConnection.Listen(50); //Ustalenie długości kolejki

            Task.Run(async () => {
                while (true)
                {
                    Socket socketAccepted = serverSocketConnection.Accept();
                    await Task.Run(async () => {
                        if (socketAccepted.Connected)
                        {
                            NetworkStream networkStream = new NetworkStream(socketAccepted); //Utworzenie strumienia do komunikacji
                            BinaryFormatterAsync bF = new BinaryFormatterAsync(); //Umożliwia serializację danych
                            while (true)
                            {
                                string data = await bF.DeserializeAsync<string>(networkStream);

                                //Logowanie do aplikacji - pobranie swojego ID, aby dalej pracować
                                if (data == "LOG")
                                {
                                    string login = await bF.DeserializeAsync<string>(networkStream); //Pozyskanie danych logowania od klienta
                                    string haslo = await bF.DeserializeAsync<string>(networkStream);

                                    string zaszyfrowaneHaslo = passwordCryptography.Encrypt(haslo);
                                    using (databaseConnection newConnection = new databaseConnection()) //Sprawdzenie danych w bazie danych (póki co nie ma jeszcze szyfrowania danych)
                                    {
                                        Pracownik pracownik = newConnection.Pracownicy.SingleOrDefault(dL => dL.Dane_logowaniaNav.Login == login && dL.Dane_logowaniaNav.Haslo == zaszyfrowaneHaslo);
                                        if (pracownik == null)
                                        {
                                            await bF.SerializeAsync<string>(networkStream, "NON_EXIST"); //Brak takich danych
                                            break; //Kończymy i zamykany strumień
                                        }
                                        else
                                        {
                                            await bF.SerializeAsync<string>(networkStream, pracownik.ID_Pracownik.ToString());
                                        }
                                    }
                                }

                                //Pobieranie danych o naprawach
                                else if (data.Contains("NDATA"))
                                {
                                    int ID = Convert.ToInt32(data.Substring(5));
                                    using (databaseConnection newConnection = new databaseConnection()) //Połączenie do bazy
                                    {
                                        List<Naprawa> naprawaL = newConnection.Naprawy.Where(n => n.ID_Pracownik == ID && n.Status_naprawy == "Przyjety").ToList<Naprawa>();
                                        Pracownik pracownik = newConnection.Pracownicy.Single(p => p.ID_Pracownik == ID);
                                        List<Pojazd> pojazdL = newConnection.Pojazdy.ToList<Pojazd>();
                                        await bF.SerializeAsync<string>(networkStream, pracownik.Imie + " " + pracownik.Nazwisko);
                                        await bF.SerializeAsync<List<Naprawa>>(networkStream, naprawaL);
                                        await bF.SerializeAsync<List<Pojazd>>(networkStream, pojazdL);
                                    }
                                }

                                //Pobranie danych o częściach
                                else if (data == "CDATA")
                                {
                                    using (databaseConnection newConnection = new databaseConnection()) //Połączenie do bazy
                                    {
                                        List<Czesc> czesciL = newConnection.Czesci.ToList<Czesc>(); //Pobranie tablicy części
                                        await bF.SerializeAsync<List<Czesc>>(networkStream, czesciL);
                                    }
                                }

                                //Pobranie danych od klienta, wysłanie SMS oraz aktualizacja bazy danych
                                else if (data == "SNDT")
                                {
                                    int naprawaID = await bF.DeserializeAsync<int>(networkStream);
                                    string komentarz = await bF.DeserializeAsync<string>(networkStream);
                                    List<Uzyte_czesci> uzyteCzesciL = await bF.DeserializeAsync<List<Uzyte_czesci>>(networkStream);
                                    using (databaseConnection newConnection = new databaseConnection()) //Połączenie do bazy
                                    {
                                        Naprawa naprawa = newConnection.Naprawy.SingleOrDefault<Naprawa>(n => n.ID_Naprawa == naprawaID);
                                        naprawa.Status_naprawy = "DoOdbioru"; //Zaznaczenie stanu jako naprawniony i jest już do odbioru
                                        naprawa.Wiadomosc_zwrotna = komentarz;
                                        //SendSMS.SendToClient(naprawaID);
                                        for (int i = 0; i < uzyteCzesciL.Count; i++)
                                        {
                                            newConnection.Uzyte_czesci.Add(new Uzyte_czesci { ID_Czesci = uzyteCzesciL[i].CzescNav.ID_Czesci, ID_Naprawa = uzyteCzesciL[i].NaprawaNav.ID_Naprawa, Ilosc = uzyteCzesciL[i].Ilosc });
                                        }
                                        newConnection.SaveChanges(); //Zapisanie zmian w bazie danych
                                    }
                                }
                                //Zakończenie połączenia
                                else if (data == "END_CONN") { break; }
                            }
                            networkStream.Close(); //Zamknięcie strumienia połączenia po zakończeniu pracy
                            socketAccepted.Close(); //Zamknięcie socketu po zakończeniu pracy
                        }
                    });
                }
            });
        }

    }
}
