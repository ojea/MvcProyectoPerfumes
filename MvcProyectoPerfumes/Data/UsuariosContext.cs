using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Models;

namespace MvcProyectoPerfumes.Data
{
    public class UsuariosContext : DbContext
    {
        public UsuariosContext(DbContextOptions<UsuariosContext> options): base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
