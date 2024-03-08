using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Models;
using MvcProyectoPerfumes.Repositories;

namespace MvcProyectoPerfumes.Controllers
{
    public class PerfumesController : Controller
    {

        private RepositoryPerfumes repo;

        public PerfumesController(RepositoryPerfumes repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Perfume> perfumes = this.repo.GetPerfumes();
            return View(perfumes);
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
