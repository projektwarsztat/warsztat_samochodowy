using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę fakutra, która całkowicie domyka całe przedsięwzięcie naprawy. Organizuje następujące informacje: dane warsztatu (ID_Warsztat), dane klienta (ID_Klient) oraz dane naprawy (ID_Napraww). Posiada trzy własności nawigacyje do wymienionych wcześniej tabel (warsztat (WarsztatNav), klient (KlientNav), naprawa (NaprawaNav)).
    /// </summary>
    [Serializable]
    public class Faktura
    {
        public Faktura()
        {
            //WarsztatNav = new Warsztat();
            //KlientNav = new Klient();
            //NaprawaNav = new Naprawa();
        }

        [Key]
        public int ID_Faktura { get; set; }
        public int ID_Warsztat { get; set; }
        public int ID_Klient { get; set; }
        public int ID_Naprawa { get; set; }
        public virtual Warsztat WarsztatNav { get; set; }
        public virtual Klient KlientNav { get; set; }
        public virtual Naprawa NaprawaNav { get; set; }
    }
}
