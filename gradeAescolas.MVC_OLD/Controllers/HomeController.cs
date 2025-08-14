using gradeAescolas.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gradeAescolas.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (!string.IsNullOrEmpty(usuarioId))
            {
                // Sessão ativa  usuário logado
                return View(); // redireciona para a view principal
            }

            // Sessão não ativa  redireciona para login
            return RedirectToAction("Login", "Account");
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
