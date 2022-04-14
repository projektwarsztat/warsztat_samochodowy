using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WarsztatV2.Tables
{
    internal class Naprawa
    {
        public Naprawa()
        {
            //PojazdNav = new Pojazd();
            //PracownikNav = new Pracownik();
        }

        [Key]
        public int ID_Naprawa { get; set; }
        public string Numer_rejestracyjny { get; set; }
        public int ID_Pracownik { get; set; }
        public DateTime? Data_przyjecia { get; set; }
        public DateTime? Data_wydania { get; set; }
        public string Status_naprawy { get; set; }
        public string Opis_usterek { get; set; }
        public string Wiadomosc_zwrotna { get; set; }
        public virtual Pojazd PojazdNav { get; set; }
        public virtual Pracownik PracownikNav { get; set; }
    }
}
