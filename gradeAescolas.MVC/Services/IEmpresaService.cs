using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaViewModel>> GetEmpresasAsync();
    Task<(IEnumerable<EmpresaViewModel> empresas, int totalCount)> GetEmpresasPaginacaoAsync(int page, int pageSize, string? search);
    Task<EmpresaViewModel> GetEmpresaByIdAsync(int id);
    Task<EmpresaViewModel> CreateEmpresaAsync(EmpresaViewModel empresaVM);
    Task<bool> UpdateEmpresaAsync(int id, EmpresaViewModel empresaVM);
    Task<bool> DeleteEmpresaAsync(int id);
}
