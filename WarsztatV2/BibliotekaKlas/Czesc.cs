using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę czesc, która organizuje informacje na temat części dostępnych w warsztacie. Zawiera dane o nazwie (Nazwa) i cenie (Cena)  danej części oraz własność nawigacyjną (kolekcję) do klasy (tabeli) uzyte_czesci (Uzyte_czesc)
    /// </summary>
    [Serializable]
    public class Czesc
    {
        public Czesc()
        {
            Uzyte_czesciNav = new List<Uzyte_czesci>();
        }

        [Key]
        public int ID_Czesci { get; set; }
        public string Nazwa { get; set; }
        public double Cena { get; set; }
        public virtual ICollection<Uzyte_czesci> Uzyte_czesciNav { get; set; }
    }
}