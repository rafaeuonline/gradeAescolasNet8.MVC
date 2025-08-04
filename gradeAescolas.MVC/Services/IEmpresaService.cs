using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaViewModel>> GetEmpresasAsync(string token);

    //x-page: 1 
    //x-pagesize: 10 
    //x-powered-by: ASP.NET
    //x-total-count: 4 
    //x-total-pages: 1 
    Task<(IEnumerable<EmpresaViewModel> empresas, int totalCount, int totalPages)>
        GetEmpresasPaginacaoAsync(int page, int pageSize, string? search, string token);

    Task<EmpresaViewModel> GetEmpresaByIdAsync(int id, string token);
    Task<EmpresaViewModel> CreateEmpresaAsync(EmpresaViewModel empresaVM, string token);
    Task<bool> UpdateEmpresaAsync(int id, EmpresaViewModel empresaVM, string token);
    Task<bool> DeleteEmpresaAsync(int id, string token);
}
