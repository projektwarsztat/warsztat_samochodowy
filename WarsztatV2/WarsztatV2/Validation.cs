using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarsztatV2
{
    internal class Validation
    {
        public Validation() { }

        /// <summary>
        /// Metoda ustawiający kolor ramki w zależności od poprawności danych
        /// </summary>
        /// <param name="tB">Pole tekstowe</param>
        public void checkData(TextBox tB)
        {
            string pattern;
            switch (tB.Name)
            {
                case "imie":
                case "nazwisko":
                case "miejscowosc":
                case "ulica":
                    {
                        pattern = @"^[.\p{L} ]+$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "telefon":
                    {
                        pattern = @"^[0-9]{9}$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "numer":
                    {
                        pattern = @"^[0-9]+([a-zA-Z]+)?(/[0-9a-zA-Z]+)?$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "kod_pocztowy":
                    {
                        pattern = @"^[0-9]{2}-[0-9]{3}$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "login":
                case "haslo":
                    {
                        if (tB.Text.Length > 4) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "Numer_rejestracyjny":
                    {
                        if (tB.Text.Length > 0 && tB.Text.Length <= 8) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "NazwaCzesci":
                case "nazwa_banku":
                case "nazwa":
                case "Marka":
                case "Model":
                case "Typ_paliwa":
                    {
                        if (tB.Text.Length > 0) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "Rok_produkcji":
                    {
                        pattern = @"^[0-9]{4}$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "Numer_VIN":
                    {
                        if (tB.Text.Length == 17) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "Cena":
                    {
                        pattern = @"^[0-9]+(,[0-9]{2})?$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "nip":
                    {
                        pattern = @"^[0-9]{10}$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                case "numer_konta":
                    {
                        pattern = @"^[0-9]{26}$";
                        Regex regex = new Regex(pattern);
                        if (regex.IsMatch(tB.Text)) tB.BorderBrush = Brushes.DarkGreen;
                        else tB.BorderBrush = Brushes.Crimson;
                        break;
                    }
                default: { break; }
            }
        }

        /// <summary>
        /// Metoda ustawiający pierwotny kolor obramowania TextBoxa
        /// </summary>
        /// <param name="tB">Pole tekstowe</param>
        public void colorRestore(TextBox tB)
        {
            tB.BorderBrush = Brushes.Silver;
        }

        /// <summary>
        /// Metoda ustawiający pierwotny kolor obramowania PasswordBox
        /// </summary>
        /// <param name="pB">Pole tekstowe</param>
        public void colorRestore(PasswordBox pB)
        {
            pB.BorderBrush = Brushes.Silver;
        }
    }
}
