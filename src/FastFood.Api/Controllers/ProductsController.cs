using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using FastFood.Domain.Products.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }    /// <summary>
    /// Cria um novo produto no catálogo
    /// </summary>
    /// <param name="createProductDto">Dados do produto a ser criado</param>
    /// <returns>Produto criado com seus detalhes</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos do produto</response>
    [HttpPost]
    [Authorize] // Exige autenticação para criar um produto
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto request)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Images = request.Images
        };

        var result = await _mediator.Send(command);

        var productDto = new ProductDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Category = result.Category,
            CategoryName = result.CategoryName,
            Price = result.Price,
            ImageUrl = result.ImageUrl,
            Images = result.Images,
            CreatedAt = result.CreatedAt,
            UpdatedAt = null
        };

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, productDto);
    }

    /// <summary>
    /// Atualiza as informações de um produto existente
    /// </summary>
    /// <param name="id">ID do produto a ser atualizado</param>
    /// <param name="updateProductDto">Novos dados do produto</param>
    /// <returns>Produto atualizado com seus detalhes</returns>
    /// <response code="200">Produto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos do produto</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpPut("{id}")]
    [Authorize] // Exige autenticação para atualizar um produto
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto request)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Images = request.Images
        };

        var result = await _mediator.Send(command);

        var productDto = new ProductDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Category = result.Category,
            CategoryName = result.CategoryName,
            Price = result.Price,
            ImageUrl = result.ImageUrl,
            Images = result.Images,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };

        return Ok(productDto);
    }

    /// <summary>
    /// Exclui um produto do catálogo
    /// </summary>
    /// <param name="id">ID do produto a ser excluído</param>
    /// <returns>Nenhum conteúdo em caso de sucesso</returns>
    /// <response code="204">Produto excluído com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpDelete("{id}")]
    [Authorize] // Exige autenticação para excluir um produto
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }    /// <summary>
    /// Busca um produto específico pelo seu ID
    /// </summary>
    /// <param name="id">ID do produto a ser buscado</param>
    /// <returns>Detalhes do produto encontrado</returns>
    /// <response code="200">Produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return NotFound(new { message = result.Error });
        }        var productDto = new ProductDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Category = result.Category,
            CategoryName = result.CategoryName,
            Price = result.Price,
            Images = result.Images,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };

        return Ok(productDto);
    }    /// <summary>
    /// Busca produtos por categoria com suporte a paginação
    /// </summary>
    /// <param name="category">Categoria dos produtos (ex: Lanche, Bebida, Sobremesa)</param>
    /// <param name="pageSize">Número de itens por página (padrão: 10)</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <returns>Lista paginada de produtos da categoria especificada</returns>
    /// <response code="200">Lista de produtos encontrada</response>
    /// <response code="400">Categoria inválida</response>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCategory(ProductCategory category, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
    {
        var query = new GetProductsByCategoryQuery
        {
            Category = category,
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        var result = await _mediator.Send(query);        var productDtos = result.Products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            CategoryName = p.CategoryName,
            Price = p.Price,
            Images = p.Images,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        return Ok(productDtos);
    }
}
