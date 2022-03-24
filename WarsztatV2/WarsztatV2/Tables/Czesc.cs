using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("czesc")]
    internal class Czesc
    {
        [Column("id_czesci"), Key, Required]
        public int ID_Czesci { get; set; }

        [Column("nazwa"), Required, MaxLength(60)]
        public string Nazwa { get; set; }

        [Column("cena"), Required]
        public float Cena { get; set; }
    }
}
