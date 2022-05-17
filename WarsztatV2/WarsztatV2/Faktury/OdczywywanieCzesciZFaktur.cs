using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotekaKlas;

namespace WarsztatV2.Faktury
{
    internal class OdczywywanieCzesciZFaktur
    {
        static string text = string.Empty;
        static int start = 0;
        static int koniec = 0;

        //metoda sprawdzajaca czy znak jest liczba
        private static bool czyl(char c)
        {
            if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0')

                return true;
            else return false;
        }

        // metoda odczytujaca plik PDF (fakturę) i zapisujaca do bazy danych czesci jesli nie istnieją juz w bazie
        public static async void ReadPDF()
        {
            //okno dialogowe do wyboru pliku
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            PdfReader reader = new PdfReader(openFileDialog.FileName);

            //odczzytanie zawartosci pliku pdf i umieszczenie go w zmiennej text
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();

            // listy do przechowywania nazw i cen czesci
            List<string> nazwy = new List<string>();
            List<string> ceny = new List<string>();

            string temp = String.Empty;
            string tempCena = String.Empty;

            // wyszukanie nazw części i ich cen w zmiennej text
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    i++;
                    if (czyl(text[i]))
                    {
                        start = i + 1;
                        koniec = i + 1;
                        i++;
                        while (text[i] != '\n')
                        {
                            temp = String.Empty;
                            tempCena = String.Empty;

                            koniec++;

                            if (czyl(text[i]))
                                if (((czyl(text[i - 1]) && czyl(text[i + 1])) || (text[i - 1] == ' ' && text[i + 1] == ' ')) && (text[i - 2] != '-'))
                                {
                                    for (int z = start + 1; z <= koniec - 3; z++)
                                    {
                                        temp += text[z];
                                    }

                                    nazwy.Add(temp);

                                    if (czyl(text[i]))
                                    {
                                        while (czyl(text[i]))
                                        {
                                            i++;
                                        }

                                        while (czyl(text[i + 1]))
                                        {
                                            tempCena += text[i];
                                            i++;
                                        }

                                        tempCena += text[i];
                                        ceny.Add(tempCena);
                                    }
                                    break;
                                }
                            i++;
                        }
                    }
                }
            }

            // dodanie do bazy danych odczytanych części
            await Task.Run(
                () =>
                {
                    using (databaseConnection newConnection = new databaseConnection())
                    {
                        for (int i = 0; i < nazwy.Count; i++)
                        {
                            if (!CzyWUzyciu(nazwy[i]))
                            {
                                newConnection.Czesci.Add(
                                new Czesc
                                {
                                    Nazwa = nazwy[i],
                                    Cena = Convert.ToDouble(ceny[i]),

                                });

                                newConnection.SaveChanges();
                            }

                        }
                    }
                }
            );

            nazwy.Clear();
            ceny.Clear();
        }

        // metoda sprawdzacjaca czy część o danej nazwie juz istnieje w bazie
        private static bool CzyWUzyciu(string nazwaCzesciDoWstawienia)
        {
            List<Czesc> czescAll;

            using (databaseConnection newConnection = new databaseConnection())
            {

                czescAll = newConnection.Czesci.ToList<Czesc>();

                for (int i = 0; i < czescAll.Count; i++)
                {
                    if (czescAll[i].Nazwa == nazwaCzesciDoWstawienia) return true;
                }
                return false;

            }
        }

    }
}
