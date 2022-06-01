using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę dane_logowania, organizująca dane logowania pracowników. Zawiera login (Login) i hasło (Haslo), które zaszyfrowane jest przy pomocy symetrycznego algorytmu szyfrującego TripleDES
    /// </summary>
    [Serializable]
    public class Dane_logowania
    {
        [Key]
        public int ID_Dane_logowania { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }

    }
}
