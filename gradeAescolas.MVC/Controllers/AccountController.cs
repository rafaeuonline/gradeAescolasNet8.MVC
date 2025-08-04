using gradeAescolas.MVC.Models;
using gradeAescolas.MVC.Services;
using gradeAescolas.MVC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;
using System.Text.Json;

namespace gradeAescolas.MVC.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAutenticacao _autenticacaoService;

    public AccountController(IAutenticacao autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    // Login - primeiro Get para renderizar a página de login, depois post para postar os dados INICIO
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Remove o cookie com o token
            Response.Cookies.Delete("X-Access-Token");

            // (Opcional) Limpa a sessão
            HttpContext.Session.Clear();

            //TempData["Mensagem"] = "Você já está logado no sistema.";
            //return RedirectToAction("Index", "Home"); // ou o controller/action do seu sistema principal


        }

        return View();

    }   

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View(model);
        }

        //verifica as credenciais do usuário e retorna um valor de token
        var result = await _autenticacaoService.AutenticaUsuario(model);

        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View(model);
        }

        // Decodifica o JWT para extrair claims
        var jwtToken = TokenHelper.ReadToken(result.AccessToken);

        // DEBUG: Mostra todas as claims no console (ou no Output do Visual Studio)
        //foreach (var claim in jwtToken.Claims)
        //{
        //    Console.WriteLine($"CLAIM TYPE: {claim.Type} - VALUE: {claim.Value}");
        //}

        // Armazena token e claims na sessão
        HttpContext.Session.SetString("AccessToken", result.AccessToken);
        HttpContext.Session.SetString("UserName", TokenHelper.GetClaim(jwtToken, ClaimTypes.Name) ?? "");
        HttpContext.Session.SetString("UserEmail", TokenHelper.GetClaim(jwtToken, ClaimTypes.Email) ?? "");
        HttpContext.Session.SetString("UsuarioId", TokenHelper.GetClaim(jwtToken, ClaimTypes.NameIdentifier) ?? "");
        HttpContext.Session.SetString("EmpresaId", TokenHelper.GetClaim(jwtToken, "EmpresaId") ?? "");
        HttpContext.Session.SetString("PessoaId", TokenHelper.GetClaim(jwtToken, "PessoaId") ?? "");
        HttpContext.Session.SetString("Role", TokenHelper.GetClaim(jwtToken, ClaimTypes.Role) ?? "");
        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(TokenHelper.GetClaims(jwtToken, ClaimTypes.Role)) ?? "");
        HttpContext.Session.SetString("TokenExpiration", result.Expiration.ToString("o"));

        // Armazena o token no cookie (HttpOnly, Secure)
        Response.Cookies.Append("X-Access-Token", result.AccessToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true, // Use true if using HTTPS
            SameSite = SameSiteMode.None, // Adjust as necessary
            Expires = result.Expiration
        });

        // Aqui você pode armazenar o token em um cookie ou sessão, conforme necessário
        // Exemplo: HttpContext.Session.SetString("Token", token.AccessToken);
        //return Redirect("/");
        return RedirectToAction("Index", "Home");
    }
    // Login - primeiro Get para renderizara a página de login, depois post para postar os dados FIM

    // metodo de logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        // Remove o cookie com o token
        Response.Cookies.Delete("X-Access-Token");

        // (Opcional) Limpa a sessão
        HttpContext.Session.Clear();

        // Redireciona para a tela de login
        return RedirectToAction("Login", "Account", new { mensagem = "204" });
    }
}
