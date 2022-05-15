using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlas
{
    [Serializable]
    public class Dane_logowania
    {
        [Key]
        public int ID_Dane_logowania { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }

    }
}
