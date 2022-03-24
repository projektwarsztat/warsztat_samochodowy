using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("uzyte_czeci")]
    internal class Uzyte_czesci
    {
        [Column("naprawa"), Required]
        public int Naprawa { get; set; }

        [Column("czesci"), Required]
        public int Czesci { get; set; }

        [Column("ilosc"), Required]
        public int Ilosc { get; set; }
    }
}
