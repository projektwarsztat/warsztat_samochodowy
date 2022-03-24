using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("naprawa")]
    internal class Naprawa
    {
        [Column("id_naprawa"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Naprawy { get; set; }

        [Column("numer_rejestracyjny"), Required, MaxLength(8)]
        public string Numer_rejestracyjny { get; set; }

        [Column("mechanik"), Required]
        public int Mechanik { get; set; }

        [Column("data_przyjecia"), Required]
        public DateTime Data_przyjecia { get; set; }

        [Column("data_wydania"), Required]
        public DataType Data_wydania { get; set; }

        [Column("status_naprawy"), Required, MaxLength(15)]
        public string Status_naprawy { get; set; }

        [Column("opis_usterek"), Required, MaxLength(500)]
        public string Opis_usterek { get; set; }
    }
}
