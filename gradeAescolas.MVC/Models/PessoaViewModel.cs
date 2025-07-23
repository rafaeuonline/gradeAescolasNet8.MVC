using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace gradeAescolas.MVC.Models;

public class PessoaViewModel
{
    public int PessoaId { get; set; }

    [Required(ErrorMessage = "O Nome é obrigatório!")]
    [StringLength(100, ErrorMessage = "O Nome pode ter no máximo 100 caracteres.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O Sobrenome é obrigatório!")]
    [StringLength(100, ErrorMessage = "O Sobrenome pode ter no máximo 100 caracteres.")]
    public string? Sobrenome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório!")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 dígitos numéricos.")]
    public string? CPF { get; set; }

    [NotMapped]
    [Display(Name = "CPF")]
    public string? CPFFormatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(CPF) || CPF.Length != 11)
                return CPF;

            return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
        }
        set
        {
            CPF = Regex.Replace(value ?? "", @"\D", "");
        }
    }

    [Required(ErrorMessage = "A Data de Nascimento é obrigatória!")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de Nascimento")]
    public DateTime DataNascimento { get; set; }

    [NotMapped]
    [Display(Name = "Data de Nascimento")]
    public string? DataNascimentoFormatado
    {
        get => DataNascimento == DateTime.MinValue
                ? null
                : DataNascimento.ToString("dd/MM/yyyy");

        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                // Tenta converter do formato dd/MM/yyyy para DateTime
                if (DateTime.TryParseExact(value, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),
                    System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    DataNascimento = parsedDate;
                }
                else
                {
                    DataNascimento = DateTime.MinValue; // valor padrão inválido
                }
            }
            else
            {
                DataNascimento = DateTime.MinValue;
            }
        }
    }


    [Required(ErrorMessage = "O E-mail é obrigatório!")]
    [EmailAddress(ErrorMessage = "E-mail inválido!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O Telefone principal é obrigatório!")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas números com DDD.")]
    public string? Telefone1 { get; set; }

    [NotMapped]
    [Display(Name = "Telefone 1")]
    public string? Telefone1Formatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Telefone1))
                return Telefone1;

            if (Telefone1.Length == 10)
                return Convert.ToUInt64(Telefone1).ToString(@"(00) 0000-0000");
            else if (Telefone1.Length == 11)
                return Convert.ToUInt64(Telefone1).ToString(@"(00) 00000-0000");

            return Telefone1;
        }
        set
        {
            Telefone1 = Regex.Replace(value ?? "", @"\D", "");
        }
    }

    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas números com DDD.")]
    public string? Telefone2 { get; set; }

    [NotMapped]
    [Display(Name = "Telefone 2")]
    public string? Telefone2Formatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Telefone2))
                return Telefone2;

            if (Telefone2.Length == 10)
                return Convert.ToUInt64(Telefone2).ToString(@"(00) 0000-0000");
            else if (Telefone2.Length == 11)
                return Convert.ToUInt64(Telefone2).ToString(@"(00) 00000-0000");

            return Telefone2;
        }
        set
        {
            Telefone2 = Regex.Replace(value ?? "", @"\D", "");
        }
    }

    public int EmpresaId { get; set; }

}