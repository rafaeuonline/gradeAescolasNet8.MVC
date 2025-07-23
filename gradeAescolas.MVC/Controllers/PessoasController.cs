using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gradeAescolas.MVC.Controllers;

[Authorize]
public class PessoasController : Controller
{
    private readonly IPessoaService _pessoaService;
    private readonly IEmpresaService _empresaService;

    public PessoasController(IPessoaService pessoaService, IEmpresaService empresaService)
    {
        _pessoaService = pessoaService;
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaViewModel>>> Index()
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var result = await _pessoaService.GetPessoasAsync(token);

            if (result == null || !result.Any())
            {
                ViewBag.Message = "Nenhuma pessoa encontrada.";
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

    //Criar nova pessoa, Get para a view, e Post para atualizar Início
    [HttpGet]
    public async Task<ActionResult> CriarNovaPessoa()
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            ViewBag.EmpresaId = 
                new SelectList( await
                    _empresaService.GetEmpresasAsync(token),
                    "EmpresaId",
                    "NomeFantasia");

            return View();
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
    public async Task<ActionResult<PessoaViewModel>> CriarNovaPessoa(PessoaViewModel pessoaVM)
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
                var result = await _pessoaService.CreatePessoaAsync(pessoaVM, token);

                if (result != null)
                    return RedirectToAction(nameof(Index));
            } 
            else
            {
                ViewBag.EmpresaId = 
                    new SelectList(await 
                        _empresaService.GetEmpresasAsync(token),
                        "EmpresaId",
                        "NomeFantasia");
            }
            ViewBag.Erro = "Erro ao criar Pessoa";
            return View(pessoaVM);
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
    //Criar nova pessoa, Get para a view, e Post para atualizar Fim
    private string? ObterTokenJwt()
    {
        var token = HttpContext.Request.Cookies["X-Access-Token"];
        return string.IsNullOrEmpty(token) ? null : token;
    }
}
