using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;

#region
//BUSCAR POR NOMBRE

    //CREATE PROCEDURE BuscarPorNombre
    //@consulta NVARCHAR(50),
    //@cantidad INT

    //AS 
    //BEGIN
    //SET NOCOUNT ON;

    //select top(@cantidad)

    //        PerfumeID, Nombre, Imagen
    //		from Perfumes
    //		WHERE Nombre LIKE '%' + @consulta + '%'
    //END
    //GO 

//======================
//BUSCAR LETRAS CON ACENTO

    //ALTER PROCEDURE BuscarPorNombre
    //@consulta NVARCHAR(50),
    //@cantidad INT

    //AS 
    //BEGIN
    //SET NOCOUNT ON;
		
    //		select top (@cantidad)
    //		PerfumeID, Nombre, Imagen
    //		from Perfumes
    //		WHERE  dbo.fnEliminarAcentos(Nombre) LIKE '%' + dbo.fnEliminarAcentos(@consulta) + '%'
    //END
    //GO

#endregion

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
