namespace MvcProyectoPerfumes.Models
{
    public class ModelIndex
    {
        public List<Perfume> Perfumes { get; set; }
        public List<string> MarcasDisponibles { get; set; }
        public string MarcaSeleccionada { get; set; }
        public string SearchString { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public Usuario Usuario { get; set; }
    }


}
