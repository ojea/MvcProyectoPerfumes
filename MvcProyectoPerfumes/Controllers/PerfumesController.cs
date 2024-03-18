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
        private IWebHostEnvironment hostEnvironment;

        public PerfumesController(RepositoryPerfumes repo, IMemoryCache memoryCache, IWebHostEnvironment hostEnvironment)
        {
            this.memoryCache = memoryCache;
            this.repo = repo;
            this.hostEnvironment = hostEnvironment;
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


        public IActionResult Detalles(int idPerfume, int? idfav, int? idColeccion, int? quieroOler)
        {

            if (HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
            {
                Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
                int rol = usuario.Rol;
                ViewData["USER"] = usuario;
                ViewData["ROL"] = rol;
                ViewData["USUARIO"] = true;
            }
            else
            {
                ViewData["USUARIO"] = false;
            }

            List<Comentario> comentarios = this.repo.ObtenerComentariosPerfume(idPerfume);
            ViewData["COMENTARIOS"] = comentarios;

            if (idfav != null)
            {
                List<Perfume> perfumesFav;

                if (this.memoryCache.Get("FAV") == null)
                {
                    perfumesFav = new List<Perfume>();
                }
                else
                {
                    perfumesFav = this.memoryCache.Get<List<Perfume>>("FAV");
                }

                // Verificar si el perfume ya está en la lista de favoritos
                bool yaEnFavoritos = perfumesFav.Any(p => p.IdPerfume == idfav.Value);


                if (!yaEnFavoritos)
                {
                    Perfume perfume = repo.ObtenerPorId(idfav.Value);
                    perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(idfav.Value);
                    perfumesFav.Add(perfume);

                    this.memoryCache.Set("FAV", perfumesFav);
                    return View(perfume);
                } 
            }


            // Para el método con idColeccion
            else if (idColeccion != null)
            {
                List<Perfume> perfumesColec;
                if (this.memoryCache.Get("COLECCION") == null)
                {
                    perfumesColec = new List<Perfume>();
                }
                else
                {
                    perfumesColec = this.memoryCache.Get<List<Perfume>>("COLECCION");
                }

                // Verificar si el perfume ya está en la lista de colección
                bool yaEnColeccion = perfumesColec.Any(p => p.IdPerfume == idColeccion.Value);

                if (!yaEnColeccion)
                {
                    Perfume perfume = repo.ObtenerPorId(idColeccion.Value);
                    perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(idColeccion.Value);
                    perfumesColec.Add(perfume);

                    this.memoryCache.Set("COLECCION", perfumesColec);
                    return View(perfume);
                }
                
            }



            // Para el método con quieroOler
            else if (quieroOler != null)
            {
                List<Perfume> perfumesOler;
                if (this.memoryCache.Get("QUIEROPROBAR") == null)
                {
                    perfumesOler = new List<Perfume>();
                }
                else
                {
                    perfumesOler = this.memoryCache.Get<List<Perfume>>("QUIEROPROBAR");
                }

                // Verificar si el perfume ya está en la lista de "quiero oler"
                bool yaEnQuieroOler = perfumesOler.Any(p => p.IdPerfume == quieroOler.Value);

                if (!yaEnQuieroOler)
                {
                    Perfume perfume = repo.ObtenerPorId(quieroOler.Value);
                    perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(quieroOler.Value);
                    perfumesOler.Add(perfume);

                    this.memoryCache.Set("QUIEROPROBAR", perfumesOler);
                    return View(perfume);
                }

               
            }

            {
                Perfume perfume = repo.ObtenerPorId(idPerfume);

                if (perfume == null)
                {
                    return NotFound();
                }

                perfume.NotasOlfativas = repo.ObtenerNotasOlfativasPorPerfumeId(idPerfume);

                return View(perfume);
            }

        }

        public IActionResult EliminarFavoritos(int idperfume)
        {
            List<Perfume> perfumesFav;

            perfumesFav = this.memoryCache.Get<List<Perfume>>("FAV");

            int index = perfumesFav.FindIndex(e => e.IdPerfume == idperfume);
            if (index != -1)
            {
                perfumesFav.RemoveAt(index);

                if (perfumesFav.Count == 0)
                {
                    // Si la lista de favoritos está vacía después de eliminar el elemento, elimina la colección de favoritos de la memoria caché
                    this.memoryCache.Remove("FAV");
                }
                else
                {
                    // Vuelve a establecer la lista de favoritos en la memoria caché
                    this.memoryCache.Set("FAV", perfumesFav);
                }
            }
            return RedirectToAction("EditarPerfil", "Usuarios");
        }
        public IActionResult EliminarDeColeccion(int idperfume)
        {
            List<Perfume> perfumesColeccion;

            perfumesColeccion = this.memoryCache.Get<List<Perfume>>("COLECCION");

            int index = perfumesColeccion.FindIndex(e => e.IdPerfume == idperfume);
            if (index != -1)
            {
                perfumesColeccion.RemoveAt(index);

                if (perfumesColeccion.Count == 0)
                {
                    // Si la lista de favoritos está vacía después de eliminar el elemento, elimina la colección de favoritos de la memoria caché
                    this.memoryCache.Remove("COLECCION");
                }
                else
                {
                    // Vuelve a establecer la lista de favoritos en la memoria caché
                    this.memoryCache.Set("COLECCION", perfumesColeccion);
                }
            }
            return RedirectToAction("EditarPerfil", "Usuarios");
        }

        public IActionResult EliminarDeProbar(int idperfume)
        {
            List<Perfume> perfumesProbar;

            perfumesProbar = this.memoryCache.Get<List<Perfume>>("QUIEROPROBAR");

            int index = perfumesProbar.FindIndex(e => e.IdPerfume == idperfume);
            if (index != -1)
            {
                perfumesProbar.RemoveAt(index);

                if (perfumesProbar.Count == 0)
                {
                    // Si la lista de favoritos está vacía después de eliminar el elemento, elimina la colección de favoritos de la memoria caché
                    this.memoryCache.Remove("QUIEROPROBAR");
                }
                else
                {
                    // Vuelve a establecer la lista de favoritos en la memoria caché
                    this.memoryCache.Set("QUIEROPROBAR", perfumesProbar);
                }
            }
            return RedirectToAction("EditarPerfil", "Usuarios");
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
            List<Perfume> perfumes = this.repo.GetPerfumes();
            ModelPrueba modelPrueba = new ModelPrueba
            {
                VistaPerfumes = vistaPerfumes
                ,
                Perfumes = perfumes

            };

            ViewData["REGISTROS"] = registros;
            ViewData["ULTIMO"] = registros;
            ViewData["SIGUIENTE"] = posicion + 9;
            ViewData["ANTERIOR"] = (posicion - 9) < 1 ? 1 : (posicion - 9);
            return View(modelPrueba);
        }

        public IActionResult AgregarComentario(int idperfume)
        {
            Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            int idUsuario = usuario.IdUsuario;
            ViewData["IDUSUARIO"] = idUsuario;
             

            ViewData["IDPERFUME"] = idperfume;
            return View();
        }

        [HttpPost]
        public IActionResult AgregarComentario(int idperfume, int usuarioID, string comentario, int rating)
        {
            DateTime fechaPublicacion = DateTime.Now;
            repo.InsertarComentario(idperfume, usuarioID, comentario, rating, fechaPublicacion);
            return RedirectToAction("Index", "Perfumes");
        }

        public IActionResult ListaPerfumes()
        {
            List<Perfume> allPerfumes = repo.GetPerfumes();
            return View(allPerfumes);

        }

        public IActionResult InsertarPerfume()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertarPerfume(string nombre, string marca, string modelo, int precioMedio, IFormFile imagen)
        {
            string rootFolder =
               this.hostEnvironment.WebRootPath;
            string fileName = imagen.FileName;

            string path = Path.Combine(rootFolder, "images", "Perfumes", fileName);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }
            repo.InsertarPerfume(nombre, marca, modelo, precioMedio, fileName);
            return View();
        }

        public IActionResult EliminarPerfume(int idperfume)
        {
            this.repo.EliminarPerfume(idperfume);
            return RedirectToAction("ListaPerfumes", "Perfumes");
        }
    }
}
