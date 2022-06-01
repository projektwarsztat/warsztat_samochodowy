using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapujaca na tabelę uzyte_czesci, która organizuje informacje na temat użytych części w danej naprawie. Zawiera właśności: ID_Naprawa (naprawa), ID_Czesci (części) oraz Ilość (ilość danej części użytej w danej naprawie). Posiada również dwie własności nawigacyjne tj. NaprawaNav (do tabeli naprawa) oraz CzescNav (do tabeli czesc). 
    /// </summary>
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