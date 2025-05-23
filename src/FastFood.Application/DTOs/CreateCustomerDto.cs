using System.ComponentModel.DataAnnotations;

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

    [Required(ErrorMessage = "O CPF é obrigatório")]
    [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
    public string Cpf { get; set; }
}
