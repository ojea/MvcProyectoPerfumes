using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;
using MvcProyectoPerfumes.Repositories;
using RedSocialNetCore.Extensions;

namespace MvcProyectoPerfumes.Controllers
{
    public class PerfumesController : Controller
    {

        private RepositoryPerfumes repo;

        public PerfumesController(RepositoryPerfumes repo)
        {
            this.repo = repo;
        }


        public IActionResult Index(string searchString, int pg=1)
        {
            List<Perfume> perfumes = this.repo.GetPerfumes();

            if(!String.IsNullOrEmpty(searchString))
            {
                perfumes = perfumes.Where(n => n.Nombre.Contains(searchString) || n.Marca.Contains(searchString)).ToList();
            }

            ModelIndex model = new ModelIndex();
            model.Perfumes = perfumes;

           if( HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
            {
                model.Usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            }

            return View(model);
        }

        public IActionResult Detalles(int id)
        {
            Perfume perfume = repo.ObtenerPorId(id);

            if (perfume == null)
            {
                return NotFound();
            }

            perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(id);

            return View(perfume);
        }



    } 
}
