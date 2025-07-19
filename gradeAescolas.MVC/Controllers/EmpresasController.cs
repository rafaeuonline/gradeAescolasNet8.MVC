using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace gradeAescolas.MVC.Controllers;

public class EmpresasController : Controller
{
    private readonly IEmpresaService _empresaService;

    public EmpresasController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpresaViewModel>>> Index()
    {
        var result = await _empresaService.GetEmpresasAsync();

        if (result == null || !result.Any())
        {
            return View("Error");
        }
        return View(result);
    }

    //Criar nova empresa, Get para a view, e Post para atualizar Início
    [HttpGet]
    public ActionResult CriarNovaEmpresa()
    {
        return View();
    }
    [HttpPost]
    public async Task<ActionResult<EmpresaViewModel>> CriarNovaEmpresa(EmpresaViewModel empresaVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _empresaService.CreateEmpresaAsync(empresaVM);

            if (result != null)
                return RedirectToAction(nameof(Index));
        }
        ViewBag.Erro = "Erro ao criar Empresa";
        return View(empresaVM);
    }
    //Criar nova empresa, Get para a view, e Post para atualizar Fim
}