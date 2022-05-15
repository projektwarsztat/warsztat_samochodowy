using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlas
{
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
