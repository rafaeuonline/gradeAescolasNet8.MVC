namespace gradeAescolas.MVC.Models;

public class ProfessorViewModel
{
    public int ProfessorId { get; set; }
    public string? Graduacao { get; set; }
    public string? Especializacao { get; set; }
    public string? Registro { get; set; }

    public PessoaViewModel Pessoa { get; set; } = new PessoaViewModel();
    public int PessoaId { get; set; }

    //public EmpresaViewModel Empresa { get; set; } = new EmpresaViewModel();
    public int EmpresaId { get; set; }

    //não temos o endpoint que retorna uma lista de usuários na API - será necessário implementar
    public UsuarioViewModel Usuario { get; set; } = new UsuarioViewModel();
}
