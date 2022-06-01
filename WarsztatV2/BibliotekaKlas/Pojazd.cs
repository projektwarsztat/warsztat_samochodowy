using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabele Pojazd, która organizuje dane na temat pojazdu. Zawiera: Numer_rejestracyjny (przechowuje numer rejestracyjny), Marka (marka pojazdu), Model (model pojazdu), Numer_VIN (numer VIN pojazdu), Rok_produkcji oraz Typ_paliwa. Posiada również dwie własności nawigacyjne do tabeli klient (KlientNav) oraz do kolekcji napraw danego pojazdu (NaprawaNav).
    /// </summary>
    [Serializable]
    public class Pojazd
    {
        public Pojazd()
        {
            //KlientNav = new Klient();
            NaprawaNav = new List<Naprawa>();
        }

        [Key]
        public string Numer_rejestracyjny { get; set; }
        public int ID_Klient { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Numer_VIN { get; set; }
        public int Rok_produkcji { get; set; }
        public string Typ_paliwa { get; set; }
        public virtual Klient KlientNav { get; set; }
        public virtual ICollection<Naprawa> NaprawaNav { get; set; }
    }
}
