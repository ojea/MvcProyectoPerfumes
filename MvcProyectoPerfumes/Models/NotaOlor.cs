using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProyectoPerfumes.Models
{
    public class NotaOlor
    {
        public int NotaOlorID { get; set; }
        public string NombreNota { get; set; }
        public List<PerfumeNotaOlor> PerfumeNotaOlor { get; set; }


    }
}
