using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WarsztatV2.Tables
{
    [Table("pojazd")]
    internal class Pojazd
    {
        [Column("numer_rejestracyjny"), Key, MaxLength(8)]
        public string Numer_rejestracyjny { get; set; }

        [Column("wlasciciel"), Required]
        public int Wlasciciel { get; set; }

        [Column("marka"), Required, MaxLength(20)]
        public string Marka { get; set; }

        [Column("model"), Required, MaxLength(20)]
        public string Model { get; set; }

        [Column("numer_vin"), Required, MaxLength(17)]
        public string Numer_VIN { get; set; }

        [Column("rok_produkcji"), Required]
        public int Rok_produkcji { get; set; }

        [Column("typ_paliwa"), Required, MaxLength(15)]
        public string Typ_paliwa { get; set; }
    }
}
