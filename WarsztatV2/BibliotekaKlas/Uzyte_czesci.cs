using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    [Serializable]
    public class Uzyte_czesci
    {
        [Key]
        public int ID { get; set; }
        public int ID_Naprawa { get; set; }
        public int ID_Czesci { get; set; }
        public int Ilosc { get; set; }
        public virtual Naprawa NaprawaNav { get; set; }
        public virtual Czesc CzescNav { get; set; }
    }
}