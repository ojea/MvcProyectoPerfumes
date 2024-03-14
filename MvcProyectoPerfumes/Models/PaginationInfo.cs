namespace MvcProyectoPerfumes.Models
{
    public class PaginationInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }

        // Elimina esta línea para corregir el error
        // public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

        public int CurrentPage { get; set; }

        // Añade este método para calcular TotalPages automáticamente
        public int GetTotalPages()
        {
            return (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        }
    }
}
