using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    [Serializable]
    public class Pracownik
    {
        public Pracownik()
        {
            //AdresNav = new Adres();
            NaprawaNav = new List<Naprawa>();
        }

        [Key]
        public int ID_Pracownik { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int ID_Adres { get; set; }
        public int Telefon { get; set; }
        public int ID_Dane_logowania { get; set; }
        public virtual Adres AdresNav { get; set; }
        public virtual Dane_logowania Dane_logowaniaNav { get; set; }
        public virtual ICollection<Naprawa> NaprawaNav { get; set; }
    }
}
