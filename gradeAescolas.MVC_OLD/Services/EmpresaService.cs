using gradeAescolas.MVC.Models;
using System.Net.Http.Headers;
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

    public async Task<IEnumerable<EmpresaViewModel>> GetEmpresasAsync(string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

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

    //x-page: 1 
    //x-pagesize: 10 
    //x-powered-by: ASP.NET
    //x-total-count: 4 
    //x-total-pages: 1 
    public async Task<(IEnumerable<EmpresaViewModel> empresas, int totalCount, int totalPages)> GetEmpresasPaginacaoAsync(int page, int pageSize, string? search, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        //localhost:8080/v1/empresas/pag?page=1&pageSize=10&search=teste
        var url = $"{apiEndpoint}pag?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        using var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var empresasVM = await JsonSerializer.DeserializeAsync<IEnumerable<EmpresaViewModel>>(stream, _options);

            // Cuidado: se header não existir, evita exceção
            var totalCount = response.Headers.TryGetValues("x-total-count", out var countValues)
                             ? int.Parse(countValues.FirstOrDefault() ?? "0")
                             : 0;

            var totalPages = response.Headers.TryGetValues("x-total-pages", out var pageValues)
                             ? int.Parse(pageValues.FirstOrDefault() ?? "0")
                             : 0;

            return (empresasVM ?? Enumerable.Empty<EmpresaViewModel>(), totalCount, totalPages);
        }

        // Erro (ex: Unauthorized)
        return (Enumerable.Empty<EmpresaViewModel>(), 0, 0);
    }


    public async Task<EmpresaViewModel> GetEmpresaByIdAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

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

    public async Task<EmpresaViewModel> CreateEmpresaAsync(EmpresaViewModel empresaVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

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

    public async Task<bool> UpdateEmpresaAsync(int id, EmpresaViewModel empresaVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

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

    public async Task<bool> DeleteEmpresaAsync(int id, string token)
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
                return false;
            }
        }
    }

    private static void PutTokenInHeaderAuthorization(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
