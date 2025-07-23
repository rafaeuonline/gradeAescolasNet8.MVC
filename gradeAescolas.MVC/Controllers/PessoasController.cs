using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Common;

namespace gradeAescolas.MVC.Controllers;

[Authorize]
public class PessoasController : Controller
{
    private readonly IPessoaService _pessoaService;
    private readonly IEmpresaService _empresaService;
    private readonly IAutenticacao _autenticacao;

    public PessoasController(IPessoaService pessoaService, IEmpresaService empresaService, IAutenticacao autenticacao)
    {
        _pessoaService = pessoaService;
        _empresaService = empresaService;
        _autenticacao  = autenticacao;       
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
    //public async Task<ActionResult<PessoaViewModel>> CriarNovaPessoa(PessoaViewModel pessoaVM)
    public async Task<ActionResult<PessoaUsuarioViewModel>> CriarNovaPessoa(PessoaUsuarioViewModel pessoaUsuarioVM)
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
                var result = await _pessoaService.CreatePessoaAsync(pessoaUsuarioVM, token);

                if (result != null)
                {
                    // Assumindo que o CreatePessoaAsync retorna o objeto com o PessoaId preenchido
                    pessoaUsuarioVM.Pessoa.PessoaId = result.Pessoa.PessoaId;

                    // Registra o usuário com base na nova Pessoa criada
                    var usuarioCriado = await _autenticacao.RegistrarUsuarioAsync(pessoaUsuarioVM, token);

                    if (!usuarioCriado)
                    {
                        TempData["MensagemErro"] = "Pessoa criada com sucesso, mas falha ao registrar o usuário.";

                        return RedirectToAction(nameof(Index));
                    }

                    // Adiciona uma Role (pode ser "Admin", "Secretaria", etc)
                    var roleAdicionada = await _autenticacao.AdicionarUsuarioRoleAsync(
                        pessoaUsuarioVM.Usuario.UserName,  // se Email for usado como UserName
                        "Admin",  // a role que você quiser atribuir
                        token);

                    if (!roleAdicionada)
                        TempData["MensagemErro"] = "Usuário criado, mas falha ao atribuir a role.";

                    return RedirectToAction(nameof(Index));

                }
            }

            // Se ModelState inválido, recarrega dados do dropdown
            ViewBag.EmpresaId = new SelectList(
                await _empresaService.GetEmpresasAsync(token),
                "EmpresaId",
                "NomeFantasia");

            ViewBag.Erro = "Erro ao criar Pessoa";
            return View(pessoaUsuarioVM);
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

    // detalhes da pessoa INÍCIO
    [HttpGet]
    public async Task<ActionResult> DetalhesPessoa(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var pessoa = await _pessoaService.GetPessoaByIdAsync(id, token);
            if (pessoa == null)
            {
                ViewBag.Message = "Pessoa não encontrada.";
                return View("Error");
            }
            return View(pessoa);
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
    // detalhes da pessoa FIM

    //Atualizar pessoa, Get para a view, e Post para atualizar Início
    [HttpGet]
    public async Task<ActionResult> AtualizarPessoa(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var pessoa = await _pessoaService.GetPessoaByIdAsync(id, token);
            if (pessoa == null)
            {
                ViewBag.Message = "Pessoa não encontrada.";
                return View("Error");
            }
            ViewBag.EmpresaId = 
                new SelectList(await 
                    _empresaService.GetEmpresasAsync(token),
                    "EmpresaId",
                    "NomeFantasia");
            return View(pessoa);
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
    public async Task<ActionResult<PessoaViewModel>> AtualizarPessoa(int id, PessoaViewModel pessoaVM)
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
                var result = await _pessoaService.UpdatePessoaAsync(id, pessoaVM, token);

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
            ViewBag.Erro = "Erro ao editar Pessoa";
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
    //Atualizar pessoa, Get para a view, e Post para atualizar Fim

    //Deletar pessoa, Get para a view, e Post para delegar INICIO
    [HttpGet]
    public async Task<ActionResult> DeletarPessoa(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var pessoa = await _pessoaService.GetPessoaByIdAsync(id, token);
            if (pessoa == null)
            {
                ViewBag.Message = "Pessoa não encontrada.";
                return View("Error");
            }
            return View(pessoa);
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
    [HttpPost(), ActionName("DeletarPessoa")]
    public async Task<ActionResult> DeletarPessoaConfirmado(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var result = await _pessoaService.DeletePessoaAsync(id, token);
            if (result)
                return RedirectToAction(nameof(Index));
            ViewBag.Erro = "Erro ao deletar Pessoa";
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
    //Deletar pessoa, Get para a view, e Post para delegar Fim

    // metodo auxiliar para obter o token JWT do cookie
    private string? ObterTokenJwt()
    {
        var token = HttpContext.Request.Cookies["X-Access-Token"];
        return string.IsNullOrEmpty(token) ? null : token;
    }

}
