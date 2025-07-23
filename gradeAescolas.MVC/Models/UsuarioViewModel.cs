using System.ComponentModel.DataAnnotations;

namespace gradeAescolas.MVC.Models;

public class UsuarioViewModel
{
    [Required(ErrorMessage = "O UserName é obrigatório!")]
    [StringLength(50, ErrorMessage = "O User Name deve ter pelo menos {2} caracteres.", MinimumLength = 4)]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "A Senha é obrigatória!")]
    [DataType(DataType.Password)]
    [StringLength(50, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%&*])[a-zA-Z\d!@#$%&*]{6,}$",
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial (!@#$%&*).")]
    public string? Password { get; set; }
}
