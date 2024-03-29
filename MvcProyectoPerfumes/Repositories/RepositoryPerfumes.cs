﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
//    where posicion >= @posicion and posicion < (@posicion + 12) 
//go



//alter view V_GRUPO_PERFUMES
//as 
//    select cast( 
//       row_number() over (order by perfumeID) as int) as posicion,
//        ISNULL(PerfumeID, 0) AS PerfumeID, Nombre, Marca, Imagen FROM Perfumes 
//go 


//COMENTARIOS

//CREATE PROCEDURE INSERTAR_COMENTARIO (
//	@PerfumeID INT,
//    @UsuarioID INT,
//    @Comentario TEXT,
//    @FechaPublicacion DATE
//)
//AS
//	DECLARE @ID INT
//	SELECT @ID = MAX(ComentarioID) + 1 FROM Comentarios
//    INSERT INTO Comentarios
//    VALUES (@ID, @PerfumeID, @UsuarioID, @Comentario, @FechaPublicacion);
//GO

//ALTER PROCEDURE INSERTAR_COMENTARIO (
//    @PerfumeID INT,
//    @UsuarioID INT,
//    @Comentario TEXT,
//    @Rating INT,
//    @FechaPublicacion DATE
//)
//AS
//    DECLARE @ID INT
//    SELECT @ID = MAX(ComentarioID) + 1 FROM Comentarios

//    INSERT INTO Comentarios (ComentarioID, PerfumeID, UsuarioID, Comentario, Rating, FechaPublicacion)
//    VALUES (@ID, @PerfumeID, @UsuarioID, @Comentario, @Rating, @FechaPublicacion);
//GO


//CREATE PROCEDURE InsertarPerfume
//    @Nombre NVARCHAR(255),
//    @Marca NVARCHAR(255),
//    @Modelo NVARCHAR(255),
//    @PrecioMedio INT,
//    @Imagen NVARCHAR(255)
//AS
//    DECLARE @ID INT 
//	SELECT @ID = MAX(PerfumeID) + 1 FROM Perfumes
//	INSERT INTO Perfumes 
//    VALUES (@ID, @Nombre, @Marca, @Modelo, 0, @PrecioMedio, 0, @Imagen);
//GO




//ALTER PROCEDURE SP_ELIMINARPERFUME
//(@ID INT)
//AS
//	DELETE FROM PerfumeNotaOlor WHERE PerfumeID = @ID;
//DELETE FROM Perfumes WHERE PerfumeID = @ID;
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

        public List<Comentario> ObtenerComentariosPerfume(int idPerfume)
        {
            var consulta = from datos in perfumesContext.Comentarios 
                           where datos.PerfumeId == idPerfume
                           select datos;
           
            return consulta.ToList();

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

        //COMENTARIOS

        public void InsertarComentario(int perfumeId, int usuarioId, string comentario, int rating, DateTime fechaPublicacion)
        {
            string sql = "INSERTAR_COMENTARIO @PerfumeID, @UsuarioID, @Comentario, @Rating, @FechaPublicacion";

            var perfumeIdParam = new SqlParameter("@PerfumeID", perfumeId);
            var usuarioIdParam = new SqlParameter("@UsuarioID", usuarioId);
            var comentarioParam = new SqlParameter("@Comentario", comentario);
            var ratingParam = new SqlParameter("@Rating", rating);
            var fechaPublicacionParam = new SqlParameter("@FechaPublicacion", fechaPublicacion);

            var consulta = this.perfumesContext.Database.ExecuteSqlRaw(sql, perfumeIdParam, usuarioIdParam, comentarioParam, ratingParam, fechaPublicacionParam);
        }

        public void InsertarPerfume (string nombre, string marca, string modelo, int precioMedio, string imagen)
        {
            string sql = "InsertarPerfume @Nombre, @Marca, @Modelo, @PrecioMedio, @Imagen";

            var paramNombre = new SqlParameter("@Nombre", nombre);
            var paramMarca = new SqlParameter("@Marca", marca);
            var paramModelo = new SqlParameter("@Modelo", modelo);
            var paramPrecioMedio = new SqlParameter("@PrecioMedio", precioMedio);
            var paramImagen = new SqlParameter("@Imagen", imagen);

            var consulta = this.perfumesContext.Database.ExecuteSqlRaw(sql, paramNombre, paramMarca, paramModelo, paramPrecioMedio, paramImagen);
        }

        public void EliminarPerfume(int idPerfume)
        {
            string sql = "SP_ELIMINARPERFUME @ID";

            SqlParameter paramId = new SqlParameter("@ID", idPerfume);

            var consulta = this.perfumesContext.Database.ExecuteSqlRaw(sql, paramId);
        }

    }
}
