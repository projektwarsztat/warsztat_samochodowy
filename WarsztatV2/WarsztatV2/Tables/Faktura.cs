using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("faktura")]
    internal class Faktura
    {
        [Column("id_faktura"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Faktura { get; set; }

        [Column("warsztat"), Required, MaxLength(100)]
        public string Warsztat { get; set; }

        [Column("klient"), Required]
        public int Klient { get; set; }

        [Column("naprawa"), Required]
        public int Naprawa { get; set; }
    }
}
