using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę klient, która organizuje informacje o klientach. Zawiera następujące informacje: imię (Imie) i nazwisko (Nazwisko), adres (ID_Adres) oraz numer telefonu (Telefon). Posiada trzy własności nawigacuje do: tabeli adres (AdresNav), oraz kolekcję do pojzadów klienta (PojazdNav) oraz faktur (FakturaNav), które zostały jemu wystawione.
    /// </summary>
    [Serializable]
    public class Klient
    {
        public Klient()
        {
            //AdresNav = new Adres();
            PojazdNav = new List<Pojazd>();
            FakturaNav = new List<Faktura>();
        }

        [Key]
        public int ID_Klient { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int ID_Adres { get; set; }
        public int Telefon { get; set; }
        public virtual Adres AdresNav { get; set; }
        public virtual ICollection<Pojazd> PojazdNav { get; set; }
        public virtual ICollection<Faktura> FakturaNav { get; set; }
    }
}
