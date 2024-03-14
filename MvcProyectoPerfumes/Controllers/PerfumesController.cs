using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Models;
using MvcProyectoPerfumes.Repositories;
using RedSocialNetCore.Extensions;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        //MENU PRINCIPAL (INDEX)

        public async Task<IActionResult> Index(string searchString, string marcaSeleccionada, int page = 1)
        {
            int pageSize = 12; // Número de elementos por página

            // Obtener todos los perfumes sin filtrar
            List<Perfume> allPerfumes = repo.GetPerfumes();

            // Obtener lista de marcas disponibles antes de aplicar cualquier filtro
            List<string> marcasDisponibles = allPerfumes.Select(p => p.Marca).Distinct().ToList();

            // Aplicar búsqueda por nombre si se proporciona
            if (!string.IsNullOrEmpty(searchString))
            {
                allPerfumes = allPerfumes.Where(p => p.Nombre.Contains(searchString)).ToList();
            }

            // Filtrar por marca si se selecciona una en el filtro
            if (!string.IsNullOrEmpty(marcaSeleccionada))
            {
                allPerfumes = allPerfumes.Where(p => p.Marca == marcaSeleccionada).ToList();
            }

            // Calcular el total de elementos y páginas después de aplicar la búsqueda y los filtros
            int totalItems = allPerfumes.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Obtener la página actual de perfumes
            List<Perfume> perfumes = allPerfumes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ModelIndex model = new ModelIndex
            {
                Perfumes = perfumes,
                MarcasDisponibles = marcasDisponibles,
                MarcaSeleccionada = marcaSeleccionada,
                SearchString = searchString,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = page,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                }
            };

            model.Perfumes = perfumes;

            if (HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
            {
                model.Usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            }

            return View(model);
        }
        //DETALLES

        [ResponseCache (Duration = 3600, Location = ResponseCacheLocation.Client)]
        public IActionResult Detalles(int id, int? idfav)
        {

            if (HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
            {
                ViewData["USUARIO"] = true;
            } else
            {
                ViewData["USUARIO"] = false;
            }

            if (idfav != null)
            {
                List<Perfume> perfumesFav;
                if(this.memoryCache.Get("FAV") == null)
                {
                    perfumesFav = new List<Perfume>();
                } else
                {
                    perfumesFav = this.memoryCache.Get<List<Perfume>>("FAV");
                }
                Perfume perfume = repo.ObtenerPorId(idfav.Value);
                perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(idfav.Value);
                perfumesFav.Add(perfume);

                this.memoryCache.Set("FAV", perfumesFav);
                return View(perfume);

            }  else
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

        //PAGINACION

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

        [HttpPost]
        public ActionResult AgregarComentario(int perfumeID, int usuarioID, string comentario, int rating, DateTime fechaPublicacion)
        {
            repo.InsertarComentario(perfumeID, usuarioID, comentario, rating, fechaPublicacion);
            return RedirectToAction("PaginarGrupoPerfumes", "Perfumes");
        }


    }
}
