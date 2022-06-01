using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaKlas
{
    /// <summary>
    /// Klasa mapowana na tabelę naprawa, organizująca jednostę naprawy. Wyposażona jest w następujące własności: numer rejestracyjny, status naprawy, opis usterek, wiadomosc zwrotna daty wydania oraz przyjecia, pracownik. Zawiera trzy własności nawigacyjne tj. PojazdNav do tabeli pojazd (numer_rejestracyjny jest kluczem obcym do tabeli pojazd), PracownikNav do tabeli pracownik (ID_Pracownik jest kluczem obcym do tabeli pracownik), Uzyte_czesciNav do tabeli uzyte_czesci (Uzyte_czesciNav są kolekcją organizującą użyte części podczas naprawy pojazdu).
    /// </summary>
    [Serializable]
    public class Naprawa
    {
        public Naprawa()
        {
            //PojazdNav = new Pojazd();
            //PracownikNav = new Pracownik();
            Uzyte_czesciNav = new List<Uzyte_czesci>();
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
        public virtual ICollection<Uzyte_czesci> Uzyte_czesciNav { get; set; }
    }
}