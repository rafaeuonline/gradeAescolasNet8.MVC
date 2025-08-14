using gradeAescolas.MVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace gradeAescolas.MVC.Services;

public class ProfessorService : IProfessorService
{
    private const string apiEndpoint = "/v1/professores/";

    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    private ProfessorViewModel professorVM;
    private IEnumerable<ProfessorViewModel> professoresVM;

    public ProfessorService(IHttpClientFactory clientFactory)
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _clientFactory = clientFactory;
    }

    public async Task<IEnumerable<ProfessorViewModel>> GetProfessoresAsync(string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                professoresVM = await JsonSerializer
                                .DeserializeAsync<IEnumerable<ProfessorViewModel>>
                                (apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return professoresVM;
    }

    public async Task<ProfessorViewModel?> GetProfessorByIdAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                professorVM = await JsonSerializer
                                .DeserializeAsync<ProfessorViewModel>(apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return professorVM;
    }

    public async Task<(IEnumerable<ProfessorViewModel> Result, int TotalCount)> GetProfessoresAsyncPagFiltro(int page, int pageSize, string? registro, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<ProfessorViewModel> CreateProfessorAsync(ProfessorViewModel professorVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        var professor = JsonSerializer.Serialize(professorVM);
        StringContent content = new StringContent(professor, Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                professorVM = await JsonSerializer
                                .DeserializeAsync<ProfessorViewModel>
                                (apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return professorVM;
    }

    public async Task<bool> UpdateProfessorAsync(int id, ProfessorViewModel professorVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.PutAsJsonAsync(apiEndpoint + id, professorVM))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false; // Handle error response
            }
        }
    }

    public async Task<bool> DeleteProfessorAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.DeleteAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false; // Handle error response
            }
        }
    }



    public async Task<ProfessorViewModel?> GetProfessorByEmpresaIdAndProfessorId(int empresaId, int professorId, string token)
    {
        throw new NotImplementedException();
    }  

    public async Task<IEnumerable<ProfessorViewModel>> GetProfessoresByEmpresaIdAsync(int empresaId, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<ProfessorViewModel> Result, int TotalCount)> GetProfessoresByEmpresaIdAsyncPagFiltro(int empresaId, int page, int pageSize, string? registro, string token)
    {
        throw new NotImplementedException();
    }

    private static void PutTokenInHeaderAuthorization(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
