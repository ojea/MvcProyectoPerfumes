using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;

namespace MvcProyectoPerfumes.Repositories
{
    public class RepositoryPerfumes
    {

        private PerfumesContext perfumesContext;

        public RepositoryPerfumes(PerfumesContext perfumesContext)
        {
            this.perfumesContext = perfumesContext;
        }

        public List<Perfume> GetPerfumes()
        {
            string sql = "PERFUMES_ALL";
            var consulta = this.perfumesContext.Perfumes.FromSqlRaw(sql);
            return consulta.ToList();
        }

        public Perfume ObtenerDetallesPerfume(int perfumeID)
        {
            var perfume = perfumesContext.Perfumes
                .FirstOrDefault(p => p.IdPerfume == perfumeID);

            return perfume;
        }

        public List<PerfumeNotaOlor> ObtenerNotasOlfativasPorPerfumeId(int perfumeId)
        {
            var notasOlfativas = perfumesContext.PerfumeNotaOlor
            .Where(pno => pno.PerfumeID == perfumeId)
            .ToList();

            return notasOlfativas;
        }

        public Perfume ObtenerPorId(int perfumeId)
        {
            var perfume = perfumesContext.Perfumes
                
                .FirstOrDefault(p => p.IdPerfume == perfumeId);

            return perfume;
        }
    }
}
