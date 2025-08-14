namespace gradeAescolas.MVC.Models;

public class TokenViewModel
{
    //public bool Authenticated { get; set; }
    //public DateTime Expiration { get; set; }
    //public string? AccessToken { get; set; }
    //public string? Message { get; set; }
    //public string? UsuarioId { get; set; }

    //public int EmpresaId { get; set; }
    //public int PessoaId { get; set; }
    //public string? Role { get; set; }

    public string? AccessToken { get; set; }
    public DateTime Expiration { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

}
