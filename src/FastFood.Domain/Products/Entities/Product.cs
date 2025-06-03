using System;
using FastFood.Domain.Products.Enums;
using FastFood.Domain.Products.Exceptions;
using FastFood.Domain.Shared.Entities;

namespace FastFood.Domain.Products.Entities;

/// <summary>
/// Representa um produto no sistema.
/// </summary>
public class Product : Entity
{
    /// <summary>
    /// Nome do produto.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição do produto.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Categoria do produto.
    /// </summary>
    public ProductCategory Category { get; private set; }

    /// <summary>
    /// Preço do produto.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// URLs das imagens do produto.
    /// </summary>
    public List<string> Images { get; private set; } = new List<string>();/// <summary>
    /// Construtor privado para uso do EF Core.
    /// </summary>
    private Product() : base() 
    {
    }

    /// <summary>
    /// Construtor para criação de um novo produto.
    /// </summary>
    /// <param name="name">Nome do produto.</param>
    /// <param name="description">Descrição do produto.</param>
    /// <param name="category">Categoria do produto.</param>
    /// <param name="price">Preço do produto.</param>
    /// <param name="images">URLs das imagens do produto (opcional).</param>
    public Product(string name, string description, ProductCategory category, decimal price, List<string>? images = null) : base()
    {
        ValidateAndSetName(name);
        ValidateAndSetDescription(description);
        ValidateAndSetCategory(category);
        ValidateAndSetPrice(price);
        if (images != null)
        {
            Images = images;
        }
    }

    /// <summary>
    /// Construtor para criação de um novo produto a partir de uma string de categoria.
    /// </summary>
    /// <param name="name">Nome do produto.</param>
    /// <param name="description">Descrição do produto.</param>
    /// <param name="categoryName">Nome da categoria do produto.</param>
    /// <param name="price">Preço do produto.</param>
    /// <param name="images">URLs das imagens do produto (opcional).</param>
    public Product(string name, string description, string categoryName, decimal price, List<string>? images = null) : base()
    {
        ValidateAndSetName(name);
        ValidateAndSetDescription(description);
        ValidateAndSetCategoryFromString(categoryName);
        ValidateAndSetPrice(price);
        if (images != null)
        {
            Images = images;
        }
    }

    /// <summary>
    /// Atualiza os dados do produto.
    /// </summary>
    /// <param name="name">Novo nome do produto.</param>
    /// <param name="description">Nova descrição do produto.</param>
    /// <param name="category">Nova categoria do produto.</param>
    /// <param name="price">Novo preço do produto.</param>
    /// <param name="images">Novas URLs das imagens do produto.</param>
    public void Update(string name, string description, ProductCategory category, decimal price, List<string>? images = null)
    {
        ValidateAndSetName(name);
        ValidateAndSetDescription(description);
        ValidateAndSetCategory(category);
        ValidateAndSetPrice(price);

        if (images != null)
        {
            Images = images;
        }

        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza os dados do produto usando uma string para a categoria.
    /// </summary>
    /// <param name="name">Novo nome do produto.</param>
    /// <param name="description">Nova descrição do produto.</param>
    /// <param name="categoryName">Nova categoria do produto (string).</param>
    /// <param name="price">Novo preço do produto.</param>
    /// <param name="images">Novas URLs das imagens do produto.</param>
    public void Update(string name, string description, string categoryName, decimal price, List<string>? images = null)
    {
        ValidateAndSetName(name);
        ValidateAndSetDescription(description);
        ValidateAndSetCategoryFromString(categoryName);
        ValidateAndSetPrice(price);

        if (images != null)
        {
            Images = images;
        }

        SetUpdatedAt();
    }

    private void ValidateAndSetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductDomainException("O nome não pode ser vazio");

        if (name.Length < 3)
            throw new ProductDomainException("O nome deve ter pelo menos 3 caracteres");

        if (name.Length > 100)
            throw new ProductDomainException("O nome não pode ter mais de 100 caracteres");

        Name = name;
    }

    private void ValidateAndSetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ProductDomainException("A descrição não pode ser vazia");

        if (description.Length > 500)
            throw new ProductDomainException("A descrição não pode ter mais de 500 caracteres");

        Description = description;
    }

    private void ValidateAndSetCategory(ProductCategory category)
    {
        // O enum já garante que o valor é válido
        Category = category;
    }

    private void ValidateAndSetCategoryFromString(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ProductDomainException("A categoria não pode ser vazia");

        if (!Enum.TryParse(categoryName, true, out ProductCategory category))
            throw new ProductDomainException($"A categoria '{categoryName}' não é válida");

        Category = category;
    }

    private void ValidateAndSetPrice(decimal price)
    {
        if (price <= 0)
            throw new ProductDomainException("O preço deve ser maior que zero");

        Price = price;
    }

    /// <summary>
    /// Adiciona uma URL de imagem ao produto.
    /// </summary>
    /// <param name="imageUrl">URL da imagem a ser adicionada.</param>
    public void AddImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ProductDomainException("A URL da imagem não pode ser vazia");

        Images.Add(imageUrl);
        SetUpdatedAt();
    }

    /// <summary>
    /// Remove uma URL de imagem do produto.
    /// </summary>
    /// <param name="imageUrl">URL da imagem a ser removida.</param>
    /// <returns>True se a imagem foi removida com sucesso; False caso contrário.</returns>
    public bool RemoveImage(string imageUrl)
    {
        var removed = Images.Remove(imageUrl);
        if (removed)
        {
            SetUpdatedAt();
        }
        return removed;
    }
}
