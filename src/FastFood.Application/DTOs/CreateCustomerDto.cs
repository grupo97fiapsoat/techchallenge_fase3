using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FastFood.Application.DTOs;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O e-mail informado não é válido")]
    [StringLength(100, ErrorMessage = "O e-mail não pode ter mais de 100 caracteres")]
    public string Email { get; set; }

    private string _cpf;
    
    [Required(ErrorMessage = "O CPF é obrigatório")]
    public string Cpf 
    { 
        get => _cpf;
        set => _cpf = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null;
    }
}
