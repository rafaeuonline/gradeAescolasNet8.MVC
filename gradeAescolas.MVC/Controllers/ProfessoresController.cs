using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gradeAescolas.MVC.Controllers;

[Authorize]
public class ProfessoresController : Controller
{
    private readonly IProfessorService _professorService;
    private readonly IPessoaService _pessoaService;
    private readonly IEmpresaService _empresaService;
    private readonly IAutenticacao _autenticacao;

    public ProfessoresController(IProfessorService professorService, IAutenticacao autenticacao, IPessoaService pessoaService, IEmpresaService empresaService)
    {
        _professorService = professorService;
        _autenticacao = autenticacao;
        _pessoaService = pessoaService;
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessorViewModel>>> Index()
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            var professores = await _professorService.GetProfessoresAsync(token);

            if (professores == null || !professores.Any())
            {
                ViewBag.Message = "Nenhum professor encontrado.";
                return View();
            }

            // Preencher os dados relacionados manualmente
            // Não é a melhor prática, porque aqui, além de chamar a api de professor, terá que chamar a de pessoal, empresa e usuário .. ou seja.. 4 chamadadas
            // Implementar DTOs na API será a melhor prática - pois assim faremos 1 chamada somente 
            foreach (var professor in professores)
            {
                professor.Pessoa = await _pessoaService.GetPessoaByIdAsync(professor.PessoaId, token);
                // professor.Empresa = await _empresaService.GetEmpresaByIdAsync(professor.EmpresaId, token);
                // professor.Usuario = await _autenticacao.(professor.UsuarioId, token);
            }

            return View(professores);
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

    //Criar novo professor (pessoa e usuário também), Get para a view, e Post para atualizar Início
    [HttpGet]
    public async Task<ActionResult> CriarNovoProfessor()
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            await PreencherDropdownEmpresas(token);

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
    public async Task<ActionResult> CriarNovoProfessor(ProfessorViewModel professorVM)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }

        try
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropdownEmpresas(token);
                return View(professorVM);
            }

            // 1. Cria a pessoa
            var pessoaCriada = await _pessoaService.CreatePessoaProfAsync(professorVM.Pessoa, token);

            if (pessoaCriada == null || pessoaCriada.PessoaId <= 0)
            {
                TempData["MensagemErro"] = "Erro ao criar a pessoa.";
                await PreencherDropdownEmpresas(token);
                return View(professorVM);
            }

            professorVM.PessoaId = pessoaCriada.PessoaId;
            professorVM.EmpresaId = pessoaCriada.EmpresaId;

            // 2. Cria o professor usando o PessoaId
            var professorCriado = await _professorService.CreateProfessorAsync(professorVM, token);
            if (professorCriado == null || professorCriado.ProfessorId <= 0)
            {
                TempData["MensagemErro"] = "Pessoa criada, mas falha ao criar o professor.";
                await PreencherDropdownEmpresas(token);
                return View(professorVM);
            }

            // 3. Cria o usuário com base no PessoaId
            var usuarioCriado = await _autenticacao.RegistrarUsuarioProfAsync(professorVM, token);
            if (!usuarioCriado)
            {
                TempData["MensagemErro"] = "Pessoa e professor criados, mas falha ao registrar o usuário.";
                return RedirectToAction(nameof(Index));
            }

            // 4. Atribui a Role ao usuário
            var roleAdicionada = await _autenticacao.AdicionarUsuarioRoleAsync(
                professorVM.Usuario.UserName,
                "ProfUser",
                token);

            if (!roleAdicionada)
                TempData["MensagemErro"] = "Usuário criado, mas falha ao atribuir a role.";

            TempData["MensagemSucesso"] = "Professor criado com sucesso!";
            return RedirectToAction(nameof(Index));
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
    //Criar novo professorprofessor (pessoa e usuário também), Get para a view, e Post para atualizar Fim

    // detalhes do professor INÍCIO
    [HttpGet]
    public async Task<ActionResult> DetalharProfessor(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var professor = await _professorService.GetProfessorByIdAsync(id, token);
            if (professor == null)
            {
                ViewBag.Message = "Professor não encontrado.";
                await PreencherDropdownEmpresas(token);
                return View("Error");
            }

            var profIdPessoa = professor.PessoaId;
            //professorVM.EmpresaId = professor.EmpresaId;

            // 2. Cria o professor usando o PessoaId
            var pessoa = await _pessoaService.GetPessoaByIdAsync(profIdPessoa, token);
            if (pessoa == null || pessoa.PessoaId <= 0)
            {
                TempData["MensagemErro"] = "Professor listado, mas falha ao lista a pessoa.";
                await PreencherDropdownEmpresas(token);
                //return View(pessoa);
            }
            professor.Pessoa.Nome = pessoa.Nome;
            professor.Pessoa.Sobrenome = pessoa.Sobrenome;
            professor.Pessoa.CPFFormatado = pessoa.CPFFormatado;
            professor.Pessoa.DataNascimentoFormatado = pessoa.DataNascimentoFormatado;
            professor.Pessoa.Email = pessoa.Email;
            professor.Pessoa.Telefone1Formatado = pessoa.Telefone1Formatado;
            professor.Pessoa.Telefone2Formatado = pessoa.Telefone2Formatado;

            return View(professor);
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
    // detalhes do professor FIM

    //Atualizar Professor (e pessoa também), Get para a view, e Post para atualizar Início
    [HttpGet]
    public async Task<ActionResult> AtualizarProfessor(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var professor = await _professorService.GetProfessorByIdAsync(id, token);
            if (professor == null)
            {
                ViewBag.Message = "Professor não encontrado.";
                await PreencherDropdownEmpresas(token);
                return View("Error");
            }
            
            await PreencherDropdownEmpresas(token);

            var profIdPessoa = professor.PessoaId;

            // 2. Lista a pessoa usando o PessoaId do ProfessorViewModelo
            var pessoa = await _pessoaService.GetPessoaByIdAsync(profIdPessoa, token);
            if (pessoa == null || pessoa.PessoaId <= 0)
            {
                TempData["MensagemErro"] = "Professor listado, mas falha ao lista a pessoa.";
                await PreencherDropdownEmpresas(token);
                //return View(pessoa);
            }
            professor.Pessoa.Nome = pessoa.Nome;
            professor.Pessoa.Sobrenome = pessoa.Sobrenome;
            professor.Pessoa.CPFFormatado = pessoa.CPFFormatado;
            professor.Pessoa.DataNascimentoFormatado = pessoa.DataNascimentoFormatado;
            professor.Pessoa.Email = pessoa.Email;
            professor.Pessoa.Telefone1Formatado = pessoa.Telefone1Formatado;
            professor.Pessoa.Telefone2Formatado = pessoa.Telefone2Formatado;

            professor.Usuario.UserName = "UserProvisório";
            professor.Usuario.Password = "123@Qwe";

            professor.Pessoa.PessoaId = pessoa.PessoaId;
            professor.Pessoa.EmpresaId = pessoa.EmpresaId;

            return View(professor);
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
    public async Task<ActionResult<ProfessorViewModel>> AtualizarProfessor(int id, ProfessorViewModel professorVM)
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
                var result = await _professorService.UpdateProfessorAsync(id, professorVM, token);

                if (result != null)
                {
                    var profIdPessoa = professorVM.PessoaId;
                    var pessoaVM = professorVM.Pessoa;
                    pessoaVM.PessoaId = professorVM.PessoaId;
                    pessoaVM.EmpresaId = professorVM.EmpresaId;

                    // 2. Atualiza a pessoa usando o PessoaId do ProfessorViewModelo
                    var pessoa = await _pessoaService.UpdatePessoaAsync(profIdPessoa, pessoaVM, token);

                    if (pessoa == null || pessoaVM.PessoaId <= 0)
                    {
                        TempData["MensagemErro"] = "Professor atualizado, mas falha ao atualizar a pessoa.";
                        await PreencherDropdownEmpresas(token);
                        return View(professorVM);
                    }
                    //professorVM.Pessoa.Nome = pessoa.Nome;
                    //professorVM.Pessoa.Sobrenome = pessoa.Sobrenome;
                    //professorVM.Pessoa.CPFFormatado = pessoa.CPFFormatado;
                    //professorVM.Pessoa.DataNascimentoFormatado = pessoa.DataNascimentoFormatado;
                    //professorVM.Pessoa.Email = pessoa.Email;
                    //professorVM.Pessoa.Telefone1Formatado = pessoa.Telefone1Formatado;
                    //professorVM.Pessoa.Telefone2Formatado = pessoa.Telefone2Formatado;

                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                await PreencherDropdownEmpresas(token);
            }
            ViewBag.Erro = "Erro ao editar Pessoa";
            await PreencherDropdownEmpresas(token);
            return View(professorVM);
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
    //Atualizar Professor (e pessoa também), Get para a view, e Post para atualizar Fim


    //Deletar professor, Get para a view, e Post para delegar INICIO
    [HttpGet]
    public async Task<ActionResult> DeletarProfessor(int id)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            var professor = await _professorService.GetProfessorByIdAsync(id, token);
            if (professor == null)
            {
                ViewBag.Message = "Professor não encontrada.";
                return View("Error");
            }

            await PreencherDropdownEmpresas(token);

            var profIdPessoa = professor.PessoaId;

            // 2. Lista a pessoa usando o PessoaId do ProfessorViewModelo
            var pessoa = await _pessoaService.GetPessoaByIdAsync(profIdPessoa, token);
            if (pessoa == null || pessoa.PessoaId <= 0)
            {
                TempData["MensagemErro"] = "Professor listado, mas falha ao lista a pessoa.";
                await PreencherDropdownEmpresas(token);
                //return View(pessoa);
            }
            professor.Pessoa.Nome = pessoa.Nome;
            professor.Pessoa.Sobrenome = pessoa.Sobrenome;
            professor.Pessoa.CPFFormatado = pessoa.CPFFormatado;
            professor.Pessoa.DataNascimentoFormatado = pessoa.DataNascimentoFormatado;
            professor.Pessoa.Email = pessoa.Email;
            professor.Pessoa.Telefone1Formatado = pessoa.Telefone1Formatado;
            professor.Pessoa.Telefone2Formatado = pessoa.Telefone2Formatado;

            professor.Usuario.UserName = "UserProvisório";
            professor.Usuario.Password = "123@QweProvisória";

            professor.Pessoa.PessoaId = pessoa.PessoaId;
            professor.Pessoa.EmpresaId = pessoa.EmpresaId;

            return View(professor);
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
    [HttpPost(), ActionName("DeletarProfessor")]
    public async Task<ActionResult> DeletarProfessorConfirmado(int PessoaId)
    {
        var token = ObterTokenJwt();
        if (token == null)
        {
            ViewBag.Message = "Você não está autenticado. Por favor, faça login.";
            return View("Error");
        }
        try
        {
            // ao invés de deletar o professor, vamos deletar a pessoa
            // ao deletar a pessoa, o professor e o usuário são deletados automaticamente no banco
            var result = await _pessoaService.DeletePessoaAsync(PessoaId, token);
            if (result)
                return RedirectToAction(nameof(Index));
            ViewBag.Erro = "Erro ao deletar Pessoa (Professor)";
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
    //Deletar professor, Get para a view, e Post para delegar Fim


    //metodo auxiliar para preencher o dropdown de empresas
    private async Task PreencherDropdownEmpresas(string token)
    {
        ViewBag.EmpresaId = new SelectList(
            await _empresaService.GetEmpresasAsync(token),
            "EmpresaId",
            "NomeFantasia");
    }

    private string? ObterTokenJwt()
    {
        var token = HttpContext.Request.Cookies["X-Access-Token"];
        return string.IsNullOrEmpty(token) ? null : token;
    }
}
