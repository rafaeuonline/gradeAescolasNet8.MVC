using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace gradeAescolas.MVC.Models;

public class EmpresaViewModel
{
    public int EmpresaId { get; set; }

    [Required(ErrorMessage = "O Nome Fantasia é obrigatório!")]
    public string? NomeFantasia { get; set; }

    [Required(ErrorMessage = "A Razão Social é obrigatória!")]
    public string? RazaoSocial { get; set; }

    [Required(ErrorMessage = "O CNPJ é obrigatório!")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter apenas números.")]
    public string? CNPJ { get; set; }

    [NotMapped]
    [Display(Name = "CNPJ")]
    public string? CNPJFormatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(CNPJ) || CNPJ.Length != 14)
                return CNPJ;

            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }
        set
        {
            CNPJ = Regex.Replace(value ?? "", @"\D", ""); // remove tudo que não for número
        }
    }

    [Required(ErrorMessage = "O E-mail é obrigatório!")]
    [EmailAddress(ErrorMessage = "E-mail inválido!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O Telefone é obrigatório!")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas números, com DDD.")]
    public string? Telefone { get; set; }

    [NotMapped]
    [Display(Name = "Telefone")]
    public string? TelefoneFormatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Telefone))
                return Telefone;

            if (Telefone.Length == 10)
                return Convert.ToUInt64(Telefone).ToString(@"(00) 0000-0000");
            else if (Telefone.Length == 11)
                return Convert.ToUInt64(Telefone).ToString(@"(00) 00000-0000");

            return Telefone;
        }
        set
        {
            Telefone = Regex.Replace(value ?? "", @"\D", ""); // remove tudo que não for número
        }
    }
}
