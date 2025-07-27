using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IAutenticacao
{
    Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel usuarioVM);
    Task<bool> RegistrarUsuarioAsync(PessoaUsuarioViewModel pessoaUsuarioVM, string token);
    Task<bool> RegistrarUsuarioProfAsync(ProfessorViewModel profUsuarioVM, string token);
    Task<bool> AdicionarUsuarioRoleAsync(string userName, string roleName, string token);
    //Task<TokenViewModel> Login(UsuarioViewModel usuarioViewModel);
    //Task Logout();
    //Task<bool> EstaAutenticado();
    //Task<string> ObterToken();
    //Task<int> ObterEmpresaId();
    //Task<int> ObterUsuarioId();
    //Task<string> ObterUsuarioEmail();
}
