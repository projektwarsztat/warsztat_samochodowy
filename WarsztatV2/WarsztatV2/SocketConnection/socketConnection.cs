using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WarsztatV2.SocketConnection
{
    public class socketConnection
    {
        public void komunikacja()
        {
            Debug.WriteLine("Tworzenie gniazda");
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Debug.WriteLine("Tworzenie adresu - punktu końcowego");
            EndPoint serverAddress = new IPEndPoint(IPAddress.Loopback, 11111);

            Debug.WriteLine("Bindowanie gniazda - dowiązanie");
            serverSocket.Bind(serverAddress);

            Debug.WriteLine("Ustawienie kolejki oczekiwania na połączenie");
            serverSocket.Listen(25);

            Debug.WriteLine("Oczekiwanie na połączenie");
            Socket socketConnect = serverSocket.Accept();

            if (socketConnect.Connected)
            {
                Debug.WriteLine("***///*** Połączenie zostało nawiązane ***\\\\\\***");
                
                Debug.WriteLine("Otwiernie strumienia powiązanego z gniazdem");
                NetworkStream streamConnection = new NetworkStream(socketConnect);

                StreamReader readerStream = new StreamReader(streamConnection);

                StreamWriter writerStream = new StreamWriter(streamConnection);

                Debug.WriteLine(readerStream.ReadLine());

                writerStream.WriteLine("ODPOWIEDZ");
                writerStream.Flush();

                readerStream.Close();
                writerStream.Close();
                streamConnection.Close();
            }

            socketConnect.Close();
        }
    }
}
