using MediatR;

namespace FastFood.Application.Queries;

/// <summary>
/// Query para listar todos os clientes com suporte a paginação
/// </summary>
public class GetAllCustomersQuery : IRequest<GetAllCustomersQueryResult>
{
    /// <summary>
    /// Número da página (começando em 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Resultado da query de listagem de clientes
/// </summary>
public class GetAllCustomersQueryResult
{
    /// <summary>
    /// Lista de clientes encontrados
    /// </summary>
    public IEnumerable<CustomerResult> Customers { get; set; } = new List<CustomerResult>();

    /// <summary>
    /// Informações de um cliente no resultado da query
    /// </summary>
    public class CustomerResult
    {
        /// <summary>
        /// Identificador único do cliente
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de e-mail do cliente
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CPF do cliente (apenas números)
        /// </summary>
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de criação do registro
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
