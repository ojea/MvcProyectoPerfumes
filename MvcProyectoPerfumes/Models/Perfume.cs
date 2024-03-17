using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcProyectoPerfumes.Models
{
    [Table("Perfumes")]
    public class Perfume
    {
        [Key]
        [Column("PerfumeID")]
        public int IdPerfume { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Marca")]
        public string Marca { get; set; }

        [Column("Modelo")]
        public string Modelo { get; set; }

        [Column("Valoracion")]
        public decimal Valoracion { get; set; }

        [Column("PrecioMedio")]
        public decimal PrecioMedio { get; set; }

        [Column("Favorito")]
        public bool Favorito { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        public List<PerfumeNotaOlor> NotasOlfativas { get; set; }
        public List<Comentario> Comentarios { get; set; }


    }
}
