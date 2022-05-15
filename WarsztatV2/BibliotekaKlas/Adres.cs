using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlas
{
    [Serializable]
    public class Adres
    {
        [Key]
        public int ID_Adres { get; set; }
        public string Ulica { get; set; }
        public string Numer { get; set; }
        public string Kod_pocztowy { get; set; }
        public string Miejscowosc { get; set; }
    }
}