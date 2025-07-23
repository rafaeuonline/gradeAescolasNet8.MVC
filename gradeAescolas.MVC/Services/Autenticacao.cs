using gradeAescolas.MVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace gradeAescolas.MVC.Services;

public class Autenticacao : IAutenticacao
{
    private readonly IHttpClientFactory _clientFactory;
    //const string apiEndpointAutentica = "/auth/login";
    private readonly JsonSerializerOptions _options;
    private TokenViewModel tokenUsuario;

    public Autenticacao(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel usuarioVM)
    {
        var client = _clientFactory.CreateClient("AutenticaApi");
        var usuario = JsonSerializer.Serialize(usuarioVM);
        StringContent content = new StringContent(usuario, Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync("login", content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                tokenUsuario = await JsonSerializer
                                .DeserializeAsync<TokenViewModel>
                                (apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return tokenUsuario;
    }

    public async Task<bool> RegistrarUsuarioAsync(PessoaUsuarioViewModel pessoaUsuarioVM, string token)
    {
        var client = _clientFactory.CreateClient("AutenticaApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var novoUsuario = new
        {
            PessoaId = pessoaUsuarioVM.Pessoa.PessoaId,
            EmpresaId = pessoaUsuarioVM.Pessoa.EmpresaId,
            //Email = pessoaUsuarioVM.Pessoa.Email,
            UserName = pessoaUsuarioVM.Usuario.UserName, // ou CPF
            Password = pessoaUsuarioVM.Usuario.Password // pode ser randomizada se quiser
        };

        var json = JsonSerializer.Serialize(novoUsuario);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("register", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AdicionarUsuarioRoleAsync(string userName, string roleName, string token)
    {
        var client = _clientFactory.CreateClient("AutenticaApi");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            userName = userName,
            roleName = roleName
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("add-user-to-role", content);

        return response.IsSuccessStatusCode;
    }
}