using gradeAescolas.MVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace gradeAescolas.MVC.Services;

public class PessoaService : IPessoaService
{
    private const string apiEndpoint = "/v1/pessoas/";

    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    private PessoaViewModel pessoaVM;
    private IEnumerable<PessoaViewModel> pessoasVM;

    private PessoaUsuarioViewModel pessoaUsuarioVM;
    private IEnumerable<PessoaUsuarioViewModel> pessoasUsuarioVM;

    public PessoaService(IHttpClientFactory clientFactory)
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _clientFactory = clientFactory;
    }

    public async Task<IEnumerable<PessoaViewModel>> GetPessoasAsync(string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                pessoasVM = await JsonSerializer
                                .DeserializeAsync<IEnumerable<PessoaViewModel>>
                                (apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return pessoasVM;
    }

    public async Task<PessoaViewModel> GetPessoaByIdAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                pessoaVM = await JsonSerializer
                                .DeserializeAsync<PessoaViewModel>(apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return pessoaVM;
    }

    public async Task<PessoaUsuarioViewModel> CreatePessoaAsync(PessoaUsuarioViewModel pessoaUsuarioVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        var pessoa = JsonSerializer.Serialize(pessoaUsuarioVM.Pessoa);
        StringContent content = new StringContent(pessoa, Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                pessoaUsuarioVM.Pessoa = await JsonSerializer
                                .DeserializeAsync<PessoaViewModel>
                                (apiResponse, _options);
            }
            else
            {
                return null; // Handle error response
            }
        }
        return pessoaUsuarioVM;
    }

    public async Task<bool> UpdatePessoaAsync(int id, PessoaViewModel pessoaVM, string token)
    {
        var client = _clientFactory.CreateClient("GradeAescolasApi");
        PutTokenInHeaderAuthorization(client, token);

        using (var response = await client.PutAsJsonAsync(apiEndpoint + id, pessoaVM))
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

    public async Task<bool> DeletePessoaAsync(int id, string token)
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

    public async Task<PessoaViewModel> GetPessoaByEmpresaIdAndPessoaId(int empresaId, int pessoaId, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<PessoaViewModel> Result, int TotalCount)> GetPessoasAsyncPagFiltro(int page, int pageSize, string? nome, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PessoaViewModel>> GetPessoasByEmpresaIdAsync(int empresaId, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<PessoaViewModel> Result, int TotalCount)> GetPessoasByEmpresaIdAsyncPagFiltro(int empresaId, int page, int pageSize, string? nome, string token)
    {
        throw new NotImplementedException();
    }

    private static void PutTokenInHeaderAuthorization(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
