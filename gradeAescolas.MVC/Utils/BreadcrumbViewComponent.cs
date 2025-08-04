using Microsoft.AspNetCore.Mvc;

namespace gradeAescolas.MVC.Utils;

public class BreadcrumbViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BreadcrumbViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        var routeData = _httpContextAccessor.HttpContext?.GetRouteData();

        var controller = routeData?.Values["controller"]?.ToString() ?? "";
        var action = routeData?.Values["action"]?.ToString() ?? "";

        var items = new List<(string Label, string? Controller, string? Action)>();

        // Caso especial para Home/Index
        if (controller == "Home" && action == "Index")
        {
            //items.Add(("Home", null, null));
            items.Add(("Bem-vindo", null, null));
        }
        else
        {
            if (!string.IsNullOrEmpty(controller))
            {
                var labelController = _breadcrumbNames.GetValueOrDefault(controller, controller);
                items.Add((labelController, controller, "Index"));
            }

            if (!string.IsNullOrEmpty(action))
            {
                var fullKey = $"{controller}/{action}";
                var labelAction = _breadcrumbNames.GetValueOrDefault(fullKey, _breadcrumbNames.GetValueOrDefault(action, action));
                items.Add((labelAction, controller, action));
            }
        }

        return View("~/Views/Shared/Breadcrumb/Default.cshtml", items);
    }

    private readonly Dictionary<string, string> _breadcrumbNames = new()
    {
        { "Home/Index", "Bem-vindo" },
        { "Empresas", "Empresas" },
        { "Empresas/Index", "Listagem de Empresas" },
        { "Empresas/CriarNovaEmpresa", "Criar Nova Empresa" },
        { "Empresas/ListaEmpresasPag", "Empresas Paginadas" },
        { "Empresas/Edit", "Editar" },
        { "Empresas/Details", "Detalhes" },
        { "Pessoas/DetalhesPessoa", "Detalhes da Pessoa" },
        { "Create", "Novo Cadastro" },
        { "Edit", "Editar" },
        { "Details", "Detalhes" },
        { "Index", "Listagem" }
    };
}
