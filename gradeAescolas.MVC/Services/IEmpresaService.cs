using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaViewModel>> GetEmpresasAsync(string token);
    Task<(IEnumerable<EmpresaViewModel> empresas, int totalCount)> GetEmpresasPaginacaoAsync(int page, int pageSize, string? search, string token);
    Task<EmpresaViewModel> GetEmpresaByIdAsync(int id, string token);
    Task<EmpresaViewModel> CreateEmpresaAsync(EmpresaViewModel empresaVM, string token);
    Task<bool> UpdateEmpresaAsync(int id, EmpresaViewModel empresaVM, string token);
    Task<bool> DeleteEmpresaAsync(int id, string token);
}
