using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IPessoaService
{
    Task<IEnumerable<PessoaViewModel>> GetPessoasAsync(string token);
    Task<(IEnumerable<PessoaViewModel> Result, int TotalCount)> GetPessoasAsyncPagFiltro(int page, int pageSize, string? nome, string token);
    Task<IEnumerable<PessoaViewModel>> GetPessoasByEmpresaIdAsync(int empresaId, string token);
    Task<(IEnumerable<PessoaViewModel> Result, int TotalCount)> GetPessoasByEmpresaIdAsyncPagFiltro(int empresaId, int page, int pageSize, string? nome, string token);
    Task<PessoaViewModel> GetPessoaByEmpresaIdAndPessoaId(int empresaId, int pessoaId, string token);
    Task<PessoaViewModel> GetPessoaByIdAsync(int id, string token);
    Task<PessoaUsuarioViewModel> CreatePessoaAsync(PessoaUsuarioViewModel pessoaUsuarioVM, string token);
    Task<PessoaViewModel> CreatePessoaProfAsync(PessoaViewModel pessoaVM, string token);
    Task<bool> UpdatePessoaAsync(int id, PessoaViewModel pessoaVM, string token);
    Task<bool> DeletePessoaAsync(int id, string token);
}