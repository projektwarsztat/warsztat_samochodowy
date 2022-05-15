using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlas
{
    [Serializable]
    public class Warsztat
    {
        public Warsztat()
        {
            //AdresNav = new Adres();
            FakturaNav = new List<Faktura>();
        }

        [Key]
        public int ID_Warsztat { get; set; }
        public string Nazwa { get; set; }
        public int ID_Adres { get; set; }
        public int Telefon { get; set; }
        public string NIP { get; set; }
        public string Numer_konta_bankowego { get; set; }
        public string Nazwa_banku { get; set; }
        public virtual Adres AdresNav { get; set; }
        public virtual ICollection<Faktura> FakturaNav { get; set; }
    }
}
