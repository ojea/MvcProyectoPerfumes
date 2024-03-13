using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProyectoPerfumes.Models
{
    [Table("Comentarios")]
    public class Comentario
    {
        [Key]
        [Column("ComentarioID")]
        public int ComentarioId { get; set; }

        [Column("@PerfumeID")]
        public int @PerfumeId { get; set; }

        [Column("@UsuarioID")]
        public int UsuarioId { get; set; }

        [Column("@Comentario")]
        public string comentario { get; set; }

        [Column("@FechaPublicacion")]
        public DateTime FechaPublicacion { get; set; }

        [Column("@Rating")]
        public int Rating { get; set; }
    }
}
