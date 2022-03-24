using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("klient")]
    internal class Klient
    {
        [Column("id_klient"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Klient { get; set; }

        [Column("imie"), Required, MaxLength(50)]
        public string Imie { get; set; }

        [Column("nazwisko"), Required, MaxLength(50)]
        public string Nazwisko { get; set; }

        [Column("adres"), Required]
        public int Adres { get; set; }

        [Column("telefon"), Required]
        public int Telefon { get; set; }
    }
}
