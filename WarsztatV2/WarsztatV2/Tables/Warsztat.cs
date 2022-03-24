using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("warsztat")]
    internal class Warsztat
    {
        [Column("nazwa"), Key, MaxLength(100)]
        public string Nazwa { get; set; }

        [Column("adres"), Required]
        public int Adres { get; set; }

        [Column("telefon"), Required]
        public int Telefon { get; set; }

        [Column("nip"), MaxLength(12), Required]
        public string NIP { get; set; }

        [Column("numer_konta_bankowego"), MaxLength(28), Required]
        public string Numer_konta_bankowego { get; set; }

        [Column("nazwa_banku"), Required, MaxLength(50)]
        public string Nazwa_banku { get; set; }
    }
}
