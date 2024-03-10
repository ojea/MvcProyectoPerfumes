using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MvcCoreCryptography.Repositories;
using MvcProyectoPerfumes.Helpers;
using MvcProyectoPerfumes.Models;
using MvcProyectoPerfumes.Repositories;
using RedSocialNetCore.Extensions;

namespace MvcCoreCryptography.Controllers
{
    public class UsuariosController : Controller
    {
        private RepositoryUsuarios repo;
        private HelperMails helperMails;
        private HelperPathProvider helperPathProvider;

        public UsuariosController(RepositoryUsuarios repo, HelperMails helperMails, HelperPathProvider helperPathProvider)
        {
            this.helperMails = helperMails;
            this.helperPathProvider = helperPathProvider;
            this.repo = repo;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register
            (string nombre, string email, string password, string imagen)
        {
            Usuario user = await this.repo.RegisterUserAsync(nombre, email, password, imagen);
            string serverUrl = this.helperPathProvider.MapUrlServerPath();
            //https://localhost:7927/Usuarios/ActivateUser/TOKEN???
            serverUrl = serverUrl + "/Usuarios/ActivateUser?token=" + user.TokenMail;
            string mensaje = "<h3>Usuario registrado</h3>";
            mensaje += "<p>Debe activar su cuenta con nosotros pulsando el siguiente enlace</p>";
            mensaje += "<a href='" + serverUrl + "'>" + serverUrl + "</a>";
            mensaje += "<p>Muchas gracias</p>";
            //await this.helperMails.SendMailAsync(email, "Registro Usuario", mensaje);
            ViewData["MENSAJE"] = "Usuario registrado correctamente. " +
                " Hemos enviado un mail para activar su cuenta";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            Usuario usuario = await this.repo.LogInUserAsync(email, password);
            if (usuario == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                HttpContext.Session.SetObject("USUARIO", usuario);
                return RedirectToAction("Index", "Perfumes");
            }

        }
        public async Task<IActionResult> ActivateUser(string token)
        {
            // BUSCAMOS EL USUARIO POR SU TOKEN
            Usuario usuario = await this.repo.GetUserByTokenAsync(token);

            if (usuario != null)
            {
                // Si encontramos al usuario, lo activamos y eliminamos el token
                await this.repo.ActivateUserAsync(token);

                ViewData["MENSAJE"] = "Cuenta activada correctamente";
                return View();
            }
            else
            {
                ViewData["MENSAJE"] = "Esta cuenta ya está activa o no existe en la DB";
                return View();
            }
        }

        //edición de perfil
        public async Task<IActionResult> EditarPerfil()
        {
            Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            int id = usuario.IdUsuario;
            Usuario user = await this.repo.FindUserAsync(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditarPerfil(string nombre, string email, string imagen)
        {
            Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            int id = usuario.IdUsuario;
            Usuario user = this.repo.ActualizarInfoUsuario(id, nombre, email, imagen);
            return View(user);
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Remove("USUARIO");
            return RedirectToAction("Index", "Perfumes");
        }

        public IActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CambiarContrasena(string contrasena)
        {

            Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            int id = usuario.IdUsuario;
            this.repo.UpdatePassw(id, contrasena);

            return RedirectToAction("EditarPerfil");
        }

    }
}