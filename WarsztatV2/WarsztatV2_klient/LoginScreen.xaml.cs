using BibliotekaKlas;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WarsztatV2_klient
{
    /// <summary>
    /// Klasa ekranu logowania klienta
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();      
        }

        /// <summary>
        /// Metoda zawierająca mechanizmy logowania klienta
        /// </summary>
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Socket clientSocketConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Utworzenie gniazda
            EndPoint serverSocketConnection = new IPEndPoint(IPAddress.Loopback, 19164); //Utworzenie adresu
            try
            {
                clientSocketConnection.Connect(serverSocketConnection);
                if (clientSocketConnection.Connected)
                {
                    NetworkStream networkStream = new NetworkStream(clientSocketConnection); //Utworzenie strumienia do komunikacji
                    BinaryFormatterAsync bF = new BinaryFormatterAsync(); //Umożliwia serializację danych
                    await bF.SerializeAsync(networkStream, "LOG"); //Wysłanie powiadomienia do serwera o próbie logowania
                    await bF.SerializeAsync(networkStream, login.Text); //Wysłanie danych logowania
                    await bF.SerializeAsync(networkStream, haslo.Password.ToString());

                    string data = await bF.DeserializeAsync<string>(networkStream);
                    if (data == "NON_EXIST") //Brak takich danych
                    { 
                        MessageBox.Show("Niepoprawny login lub/i hasło!\nPodaj prawidłowe dane, aby się zalogować!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        login.Clear();
                        haslo.Clear();
                    }
                    else
                    {
                        await bF.SerializeAsync<string>(networkStream, "END_CONN");
                        networkStream.Close();
                        clientSocketConnection.Close();
                        MainWindow mainWindow = new MainWindow(data);
                        mainWindow.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Napotkano błąd!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
    }
}