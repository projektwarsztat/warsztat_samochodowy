using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę adres, która przechowuje informacje adresowe wszystkich podmiotów. Zawiera: Ulica (własność przechowywująca ulicę), Numer (przechowuje numer mieszkania/domu), Kod_pocztowy (przechowuje kod pocztowy) oraz Miejscowość (przechowuje nazwe miejscowosci)
    /// </summary>
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