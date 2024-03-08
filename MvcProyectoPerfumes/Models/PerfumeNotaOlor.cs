using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace MvcProyectoPerfumes.Models
{
    [Table("V_NOTAS")]
    public class PerfumeNotaOlor
    {
        [Key]
        [Column("PerfumeID")]
        public int PerfumeID { get; set; }

        [Column("NotaOlorID")]
        public int NotaOlorID { get; set; }

        [Column("NOTAOLFATIVA")]
        public string NombreNota {  get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

    }
}
