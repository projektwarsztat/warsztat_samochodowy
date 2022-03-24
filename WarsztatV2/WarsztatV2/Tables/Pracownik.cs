using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("pracownik")]
    internal class Pracownik
    {
        [Column("id_pracownik"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Pracownik { get; set; }

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
