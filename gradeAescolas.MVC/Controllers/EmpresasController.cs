using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace gradeAescolas.MVC.Controllers;

[Authorize]
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
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var result = await _empresaService.GetEmpresasAsync(token);

            if (result == null || !result.Any())
            {
                return View("Error");
            }
            return View(result);
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
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
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            if (ModelState.IsValid)
            {
                var result = await _empresaService.CreateEmpresaAsync(empresaVM, token);

                if (result != null)
                    return RedirectToAction(nameof(Index));
            }
            ViewBag.Erro = "Erro ao criar Empresa";
            return View(empresaVM);
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
    }
    //Criar nova empresa, Get para a view, e Post para atualizar Fim

    //Atualizar empresa, Get para a view, e Post para atualizar Início
    [HttpGet]
    public async Task<ActionResult> AtualizarEmpresa(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var result = await _empresaService.GetEmpresaByIdAsync(id, token);

            if (result == null)
            {
                return View("Error");
            }
            return View(result);
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
    }
    [HttpPost]
    public async Task<ActionResult<EmpresaViewModel>> AtualizarEmpresa(int id, EmpresaViewModel empresaVM)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            if (ModelState.IsValid)
            {
                var result = await _empresaService.UpdateEmpresaAsync(id, empresaVM, token);
                if (result)
                    return RedirectToAction(nameof(Index));
            }
            ViewBag.Erro = "Erro ao atualizar Empresa";
            return View(empresaVM);
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
    }
    //Atualizar empresa, Get para a view, e Post para atualizar Fim

    //Deletar empresa, Get para a view, e Delete para deletar Início
    [HttpGet]
    public async Task<ActionResult> DeletarEmpresa(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var result = await _empresaService.GetEmpresaByIdAsync(id, token);

            if (result == null)
            {
                return View("Error");
            }
            return View(result);
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
    }
    [HttpPost(), ActionName("DeletarEmpresa")]
    public async Task<ActionResult> DeletaConfirmado(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var result = await _empresaService.DeleteEmpresaAsync(id, token);

            if (result)
                return RedirectToAction("Index");

            return View("Error");
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Message = "Sua sessão expirou. Por favor, faça login novamente.";
            return View("Error");
        }
        catch (Exception)
        {
            ViewBag.Message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            return View("Error");
        }
    }
    //Deletar empresa, Get para a view, e Delete para deletar Fim
    private string? ObterTokenJwt()
    {
        var token = HttpContext.Request.Cookies["X-Access-Token"];
        return string.IsNullOrEmpty(token) ? null : token;
    }
}