using gradeAescolas.MVC.Models;
using System.Text;
using System.Text.Json;

namespace gradeAescolas.MVC.Services;

public class EmpresaService : IEmpresaService
{
    private const string apiEndpoint = "/v1/empresas/";

    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    private EmpresaViewModel empresaVM;
    private IEnumerable<EmpresaViewModel> empresasVM;

    public EmpresaService(IHttpClientFactory clientFactory)
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
        _clientFactory = clientFactory;
    }

    public async Task<IEnumerable<EmpresaViewModel>> GetEmpresasAsync()
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                empresasVM = await JsonSerializer
                                .DeserializeAsync<IEnumerable<EmpresaViewModel>>
                                (apiResponse, _options);
            }
            else
            {
                // Handle error response
                //return Enumerable.Empty<EmpresaViewModel>();
                return null;
            }
        }
        return empresasVM;
    }

    public async Task<EmpresaViewModel> GetEmpresaByIdAsync(int id)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                empresaVM = await JsonSerializer
                                .DeserializeAsync<EmpresaViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return empresaVM;
    }

    public async Task<EmpresaViewModel> CreateEmpresaAsync(EmpresaViewModel empresaVM)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");

        var empresa = JsonSerializer.Serialize(empresaVM);
        StringContent content = new StringContent(empresa, Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                empresaVM = await JsonSerializer
                                .DeserializeAsync<EmpresaViewModel>(apiResponse, _options);
            }
            else
            {
                // Handle error response
                return null;
            }
        }
        return empresaVM;
    }

    public async Task<bool> UpdateEmpresaAsync(int id, EmpresaViewModel empresaVM)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");

        using (var response = await client.PutAsJsonAsync(apiEndpoint + id, empresaVM))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task<bool> DeleteEmpresaAsync(int id)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");

        using (var response = await client.DeleteAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task<(IEnumerable<EmpresaViewModel> empresas, int totalCount)> GetEmpresasPaginacaoAsync(int page, int pageSize, string? search)
    {
        throw new NotImplementedException();
    }

}
