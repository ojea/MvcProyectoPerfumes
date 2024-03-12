using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProyectoPerfumes.Models
{
    [Table("V_PERFUMES")]
    public class VistaPerfumes
    {
        [Key]
        [Column("PerfumeID")]
        public int IdPerfume { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Marca")]
        public string Marca { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        [Column("posicion")]
        public int Posicion { get; set; }
    }
}
