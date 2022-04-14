using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WarsztatV2.Tables
{
    internal class Klient
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
