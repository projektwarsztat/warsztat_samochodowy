using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatV2.Tables
{
    [Table("adres")]
    internal class Adres
    {
        [Column("id_adres"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Adres { get; set; }

        [Column("ulica"), MaxLength(50), Required]
        public string Ulica { get; set; }

        [Column("numer"), MaxLength(10), Required]
        public string Numer { get; set; }

        [Column("kod_pocztowy"), MaxLength(6), Required]
        public string Kod_pocztowy { get; set; }

        [Column("miejscowosc"), MaxLength(50), Required]
        public string Miejscowosc { get; set; }
    }
}
