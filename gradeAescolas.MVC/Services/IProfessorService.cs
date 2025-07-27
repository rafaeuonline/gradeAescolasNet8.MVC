using gradeAescolas.MVC.Models;

namespace gradeAescolas.MVC.Services;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorViewModel>> GetProfessoresAsync(string token);
    Task<(IEnumerable<ProfessorViewModel> Result, int TotalCount)> GetProfessoresAsyncPagFiltro(int page, int pageSize, string? registro, string token);
    Task<IEnumerable<ProfessorViewModel>> GetProfessoresByEmpresaIdAsync(int empresaId, string token);
    Task<(IEnumerable<ProfessorViewModel> Result, int TotalCount)> GetProfessoresByEmpresaIdAsyncPagFiltro(int empresaId, int page, int pageSize, string? registro, string token);
    Task<ProfessorViewModel?> GetProfessorByEmpresaIdAndProfessorId(int empresaId, int professorId, string token);
    Task<ProfessorViewModel?> GetProfessorByIdAsync(int id, string token);
    Task<ProfessorViewModel> CreateProfessorAsync(ProfessorViewModel professorVM, string token);
    Task<bool> UpdateProfessorAsync(int id, ProfessorViewModel professorVM, string token);
    Task<bool> DeleteProfessorAsync(int id, string token);
}
