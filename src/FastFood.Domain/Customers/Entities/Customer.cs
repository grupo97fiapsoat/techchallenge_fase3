using FastFood.Domain.Customers.Exceptions;
using FastFood.Domain.Customers.ValueObjects;
using FastFood.Domain.Shared.Entities;

namespace FastFood.Domain.Customers.Entities;

/// <summary>
/// Representa um cliente no sistema.
/// </summary>
public class Customer : Entity
{
    /// <summary>
    /// Nome do cliente, representado como um Value Object para garantir validação e integridade.
    /// </summary>
    public Name Name { get; private set; }

    /// <summary>
    /// Email do cliente, representado como um Value Object para garantir validação e integridade.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// CPF do cliente, representado como um Value Object para garantir validação e integridade.
    /// </summary>
    public Cpf Cpf { get; private set; }

    /// <summary>
    /// Construtor privado para uso do EF Core.
    /// </summary>
    private Customer() : base() { }

    /// <summary>
    /// Construtor interno que recebe os Value Objects já validados.
    /// </summary>
    /// <param name="name">Nome do cliente (já validado).</param>
    /// <param name="email">Email do cliente (já validado).</param>
    /// <param name="cpf">CPF do cliente (já validado).</param>
    public Customer(Name name, Email email, Cpf cpf) : base()
    {
        Name = name;
        Email = email;
        Cpf = cpf;
    }

    /// <summary>
    /// Factory method para criar um novo cliente com validação de todos os campos.
    /// </summary>
    /// <param name="name">Nome do cliente (3-100 caracteres).</param>
    /// <param name="email">Email do cliente (formato válido).</param>
    /// <param name="cpf">CPF do cliente (11 dígitos).</param>
    /// <returns>Uma nova instância de Customer com os dados validados.</returns>
    /// <exception cref="CustomerDomainException">Lançada quando algum dos campos é inválido.</exception>
    public static Customer Create(string name, string email, string cpf)
    {
        return new Customer(
            Name.Create(name),
            Email.Create(email),
            Cpf.Create(cpf)
        );
    }

    /// <summary>
    /// Atualiza os dados do cliente que podem ser modificados (nome e email).
    /// O CPF não pode ser alterado após a criação.
    /// </summary>
    /// <param name="name">Novo nome do cliente (3-100 caracteres).</param>
    /// <param name="email">Novo email do cliente (formato válido).</param>
    /// <exception cref="CustomerDomainException">Lançada quando algum dos campos é inválido.</exception>
    public void Update(string name, string email)
    {
        Name = Name.Create(name);
        Email = Email.Create(email);
        SetUpdatedAt();
    }
}
