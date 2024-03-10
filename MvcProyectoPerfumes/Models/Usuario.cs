using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcProyectoPerfumes.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("UsuarioID")]
        public int IdUsuario { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Contraseña")]
        public byte[] Password { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        [Column("NombreUsuario")]
        public string NombreUsuario { get; set; }

        [Column("SALT")]
        public string Salt { get; set; }

        [Column("ACTIVO")]
        public bool Activo { get; set; }

        [Column("TOKENMAIL")]
        public string TokenMail { get; set; }

        [Column("PASS")]
        public string Pass { get; set; }

        //[Column("ROL")]
        //public string Rol { get; set; }

        public ICollection<Perfume> Perfumes { get; set; }
    }
}
