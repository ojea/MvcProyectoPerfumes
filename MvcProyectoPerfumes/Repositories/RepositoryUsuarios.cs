using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Helpers;
using MvcProyectoPerfumes.Models;
using System.Diagnostics.Metrics;

#region

//======UPDATE USER======

    //ALTER PROCEDURE SP_UPDATE_USUARIO
    //(@UsuarioID INT,
    // @NOMBRE NVARCHAR(30),
    // @EMAIL NVARCHAR(50),
    // @Imagen NVARCHAR(500))
    //AS
    //BEGIN
    //    UPDATE Usuarios SET NombreUsuario = @NOMBRE,
    //    EMAIL = @EMAIL, imagen = @Imagen
    //    WHERE UsuarioID = @UsuarioID;
    //SELECT* FROM Usuarios 
    //    WHERE UsuarioID = @UsuarioID;
    //END;
    //GO

//======UPDATE PASS======

    //CREATE PROCEDURE SP_UPDATE_USER_PASSW
    //(@ID INT,
    //@PASW VARBINARY(MAX),
    //@SALT NVARCHAR(50))
    //AS
    //    UPDATE Usuarios SET Contraseña = @PASW,
    //    SALT = @SALT WHERE UsuarioID = @ID
    //GO

//======UPDATE FOTO PERFIL======

    //CREATE PROCEDURE SP_UPDATE_USER_FOTO
    // (@ID INT,
    // @IMAGEN NVARCHAR(200))
    // AS
    //     UPDATE Usuarios SET Imagen = @IMAGEN
    //     WHERE UsuarioID = @ID
    // GO
#endregion

namespace MvcCoreCryptography.Repositories
{
    public class RepositoryUsuarios
    {
        private UsuariosContext context;

        public RepositoryUsuarios(UsuariosContext context)
        {
            this.context = context;
        }

        public async Task<Usuario> FindUserAsync(int idusuario)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.IdUsuario == (idusuario)
                           select datos;
            Usuario user = await consulta.FirstOrDefaultAsync();
            return user;
        }

        private async Task<int> GetMaxIdUsuarioAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await
                    this.context.Usuarios.MaxAsync(z => z.IdUsuario) + 1;
            }
        }

        public async Task<Usuario>
            RegisterUserAsync(string nombre, string email
            , string password, string imagen)
        {

            Usuario user = new Usuario();
            user.Rol = 0;
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.NombreUsuario = nombre;
            user.Email = email;
            user.Imagen = "default.png";
            //TEMPORAL
            user.Activo = true;
            user.Pass = password;
            //CADA USUARIO TENDRA UN SALT DISTINTO
            user.Salt = HelperCryptography.GenerateSalt();
            user.TokenMail = HelperTools.GenerateTokenMail();
            //GUARDAMOS EL PASSWORD EN BYTE[]
            user.Password =
                HelperCryptography.EncryptPassword(password, user.Salt);

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();


            return user;
        }

        public async Task ActivateUserAsync(string token)
        {
            //BUSCAMOS EL USUARIO POR SU TOKEN
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(x => x.TokenMail == token);
            user.Activo = true;
            //user.TokenMail = "";
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> GetUserByTokenAsync(string token)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(x => x.TokenMail == token);
        }
        public async Task<Usuario> LogInUserAsync(string email, string password)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario != null)
            {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = usuario.Password;
                bool response = HelperCryptography.CompareArrays(temp, passUser);

                if (response == true)
                    return usuario;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        public Usuario GetUser(string email)
        {
            var usuario = (from u in context.Usuarios
                           where u.Email == email
                           select u).FirstOrDefault();
            return usuario;
        }

        public Usuario ActualizarInfoUsuario
            (int id, string nombre, string email, string imagen)
        {
            string sql = "SP_UPDATE_USER @UsuarioID, @NOMBRE, @EMAIL, @Imagen";

            SqlParameter pid = new SqlParameter("@UsuarioID", id);
            SqlParameter pnom = new SqlParameter("@NOMBRE", nombre);
            SqlParameter pmail = new SqlParameter("@EMAIL", email);
            SqlParameter pima = new SqlParameter("@Imagen", imagen);

            var consulta = this.context.Usuarios.FromSqlRaw(sql, pid, pnom, pmail, pima);

            Usuario user = consulta.AsEnumerable().FirstOrDefault();

            return user;
        }

        public void UpdatePassw(int id, string contrasena)
        {
            string salt = HelperCryptography.GenerateSalt();
            byte[] passw = HelperCryptography.EncryptPassword(contrasena, salt);

            string sql = "SP_UPDATE_USER_PASSW @ID, @PASW, @SALT";

            SqlParameter pid = new SqlParameter("@ID", id);
            SqlParameter ppas = new SqlParameter("@PASW", passw);
            SqlParameter psalt = new SqlParameter("@SALT", salt);

            var consulta = this.context.Database.ExecuteSqlRaw(sql, pid, ppas, psalt);

        }
        public void UpdatePicture(int id, string imagen)
        {
            string sql = "SP_UPDATE_USER_FOTO @ID, @IMAGEN";

            SqlParameter pid = new SqlParameter("@ID", id);
            SqlParameter pfoto = new SqlParameter("@IMAGEN", imagen);

            var consulta = this.context.Database.ExecuteSqlRaw(sql, pid, pfoto);
        }
    }
}