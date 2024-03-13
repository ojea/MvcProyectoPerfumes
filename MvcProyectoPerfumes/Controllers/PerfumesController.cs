using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;
using MvcProyectoPerfumes.Repositories;
using RedSocialNetCore.Extensions;
using System.Collections.Generic;

namespace MvcProyectoPerfumes.Controllers
{
    public class PerfumesController : Controller
    {

        private RepositoryPerfumes repo;
        private IMemoryCache memoryCache;

        public PerfumesController(RepositoryPerfumes repo, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.repo = repo;
        }


        public IActionResult Index(string searchString, int pg = 1)
        {
            List<Perfume> perfumes = this.repo.GetPerfumes();

            if (!String.IsNullOrEmpty(searchString))
            {
                perfumes = perfumes.Where(n => n.Nombre.Contains(searchString) || n.Marca.Contains(searchString)).ToList();
            }

            ModelIndex model = new ModelIndex();
            model.Perfumes = perfumes;

            if (HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
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

        public async Task<IActionResult>
            PaginarGrupoPerfumes(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int registros = await this.repo.GetNumeroPerfumesAsync();

            List<VistaPerfumes> vistaPerfumes =

                await this.repo.GetGrupoPerfumesAsync(posicion.Value);
            List<Perfume> perfumes =  this.repo.GetPerfumes();
            ModelPrueba modelPrueba = new ModelPrueba
            {
                VistaPerfumes = vistaPerfumes
                , Perfumes = perfumes
                
            };

            ViewData["REGISTROS"] = registros;
            ViewData["ULTIMO"] = registros;
            ViewData["SIGUIENTE"] = posicion + 9;
            ViewData["ANTERIOR"] = (posicion - 9) < 1 ? 1 : (posicion - 9);
            return View(modelPrueba);
        }
    }
}
