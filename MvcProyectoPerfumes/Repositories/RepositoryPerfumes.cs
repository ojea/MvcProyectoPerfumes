using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;
using System.Diagnostics.Metrics;

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

//======================
//PAGINACION

//alter procedure SP_GRUPO_PERFUMES_PAGINACION
//(@posicion int) 
//as 
//    select PerfumeID, Nombre, Marca, Imagen, posicion
//	from V_GRUPO_PERFUMES 
//    where posicion >= @posicion and posicion < (@posicion + 3) 
//go



//alter view V_GRUPO_PERFUMES
//as 
//    select cast( 
//       row_number() over (order by perfumeID) as int) as posicion,
//        ISNULL(PerfumeID, 0) AS PerfumeID, Nombre, Marca, Imagen FROM Perfumes 
//go 

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

        //========GET PERFUMES========

        public List<Perfume> GetPerfumes()
        {
            string sql = "PERFUMES_ALL";
            var consulta = this.perfumesContext.Perfumes.FromSqlRaw(sql);
            return consulta.ToList();


        }

        //========DETALLES PERFUMES========

        public Perfume ObtenerDetallesPerfume(int perfumeID)
        {
            var perfume = perfumesContext.Perfumes
                .FirstOrDefault(p => p.IdPerfume == perfumeID);

            return perfume;
        }

        //========NOTAS POR PERFUMES========

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

        //PAGINACION
        public async Task<int> GetNumeroPerfumesAsync()
        {
            return await this.perfumesContext.VistaPerfumes.CountAsync();
        }


        public async Task<List<VistaPerfumes>>

            GetGrupoPerfumesAsync(int posicion)

        {

            string sql = "SP_GRUPO_PERFUMES_PAGINACION @posicion";

            SqlParameter pamPosicion =
                new SqlParameter("@posicion", posicion);
            var consulta = this.perfumesContext.VistaPerfumes.FromSqlRaw(sql, pamPosicion);
            VistaPerfumes vistaPerfumes = new VistaPerfumes(); 
            return await consulta.ToListAsync();
        }
    }
}
