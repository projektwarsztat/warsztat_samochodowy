using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WarsztatV2.Tables
{
    internal class Czesc
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