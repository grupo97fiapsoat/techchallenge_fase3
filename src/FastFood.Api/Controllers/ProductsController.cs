using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using FastFood.Domain.Products.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

/// <summary>
/// Controlador para gerenciamento do catálogo de produtos
/// 
/// **Finalidade:** Gerencia o catálogo completo de produtos do restaurante FastFood.
/// 
/// **Categorias disponíveis:**
/// - **Lanche (0)**: Hambúrguers, sanduíches, wraps
/// - **Acompanhamento (1)**: Batatas fritas, anéis de cebola, nuggets
/// - **Bebida (2)**: Refrigerantes, sucos, águas, cafés
/// - **Sobremesa (3)**: Sorvetes, tortas, cookies, milk-shakes
/// 
/// **Níveis de acesso:**
/// - **Consulta (Público)**: Qualquer pessoa pode visualizar produtos
/// - **Gestão (Protegido)**: Apenas usuários autenticados podem criar, editar ou excluir produtos
/// 
/// **Fluxo típico para clientes:**
/// 1. Consulte produtos por categoria usando `GET /category/{category}`
/// 2. Veja detalhes de um produto específico usando `GET /{id}`
/// 3. Use os IDs dos produtos para criar um pedido
/// </summary>
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }    /// <summary>    /// Cria um novo produto no catálogo
    /// 
    /// **Requer autenticação:** Apenas usuários autenticados podem criar produtos.
    /// 
    /// **Como usar:**
    /// 1. Autentique-se primeiro usando `/api/v1/auth/login`
    /// 2. Envie os dados do produto no formato JSON
    /// 3. O produto será criado e ficará disponível no catálogo
    /// 
    /// **Campos obrigatórios:**
    /// - Name: Nome do produto (ex: "Big Mac")
    /// - Description: Descrição detalhada
    /// - Category: Categoria (0=Lanche, 1=Acompanhamento, 2=Bebida, 3=Sobremesa)
    /// - Price: Preço em decimal (ex: 25.90)
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "name": "Hambúrguer Especial",
    ///   "description": "Hambúrguer artesanal com carne 180g, queijo cheddar e molho especial",
    ///   "category": 0,
    ///   "price": 28.90,
    ///   "imageUrl": "https://exemplo.com/imagem.jpg"
    /// }
    /// ```
    /// </summary>
    /// <param name="request">Dados do produto a ser criado</param>
    /// <returns>Produto criado com seus detalhes, incluindo ID gerado</returns>
    /// <response code="201">Produto criado com sucesso - Disponível no catálogo</response>
    /// <response code="400">Dados inválidos - Verifique os campos obrigatórios</response>
    /// <response code="401">Não autorizado - Faça login para criar produtos</response>
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
    }    /// <summary>    /// Atualiza as informações de um produto existente
    /// 
    /// **Requer autenticação:** Apenas usuários autenticados podem atualizar produtos.
    /// 
    /// **Como usar:**
    /// 1. Autentique-se primeiro usando `/api/v1/auth/login`
    /// 2. Forneça o ID do produto que deseja atualizar na URL
    /// 3. Envie os novos dados do produto no formato JSON
    /// 4. Apenas os campos enviados serão atualizados
    /// 
    /// **Dica:** Use `GET /{id}` primeiro para ver os dados atuais do produto.
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "name": "Hambúrguer Especial Premium",
    ///   "description": "Versão premium com ingredientes selecionados",
    ///   "price": 32.90
    /// }
    /// ```
    /// </summary>
    /// <param name="id">ID do produto a ser atualizado</param>
    /// <param name="request">Novos dados do produto</param>
    /// <returns>Produto atualizado com seus detalhes</returns>
    /// <response code="200">Produto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos - Verifique o formato dos dados</response>
    /// <response code="404">Produto não encontrado - Verifique se o ID está correto</response>
    /// <response code="401">Não autorizado - Faça login para atualizar produtos</response>
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
    }    /// <summary>    /// Exclui um produto do catálogo permanentemente
    /// 
    /// **Requer autenticação:** Apenas usuários autenticados podem excluir produtos.
    /// **Ação irreversível:** Esta operação não pode ser desfeita.
    /// 
    /// **Como usar:**
    /// 1. Autentique-se primeiro usando `/api/v1/auth/login`
    /// 2. Forneça o ID do produto que deseja excluir na URL
    /// 3. Confirme que realmente deseja excluir (operação irreversível)
    /// 
    /// **Importante:** Se o produto estiver sendo usado em pedidos existentes, 
    /// considere desativá-lo ao invés de excluí-lo para manter a integridade histórica.
    /// </summary>
    /// <param name="id">ID do produto a ser excluído permanentemente</param>
    /// <returns>Nenhum conteúdo em caso de sucesso</returns>
    /// <response code="204">Produto excluído com sucesso</response>
    /// <response code="404">Produto não encontrado - Verifique se o ID está correto</response>
    /// <response code="401">Não autorizado - Faça login para excluir produtos</response>
    [HttpDelete("{id}")]
    [Authorize] // Exige autenticação para excluir um produto
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }    /// <summary>    /// Busca um produto específico pelo seu ID
    /// 
    /// **Endpoint público:** Não requer autenticação.
    /// 
    /// **Como usar:**
    /// 1. Forneça o ID do produto na URL
    /// 2. Receba todos os detalhes do produto, incluindo preço, descrição e imagens
    /// 3. Use essas informações para exibir o produto ao cliente
    /// 
    /// **Informações retornadas:**
    /// - Dados completos do produto (nome, descrição, preço)
    /// - Categoria e nome da categoria
    /// - URLs de imagens
    /// - Datas de criação e última atualização
    /// 
    /// **Dica:** Use este endpoint para obter detalhes antes de adicionar um produto ao pedido.
    /// </summary>
    /// <param name="id">ID único do produto a ser buscado</param>
    /// <returns>Detalhes completos do produto encontrado</returns>
    /// <response code="200">Produto encontrado - Retorna todos os detalhes</response>
    /// <response code="404">Produto não encontrado - Verifique se o ID está correto</response>
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
    }    /// <summary>    /// Busca produtos por categoria com suporte a paginação
    /// 
    /// **Endpoint público:** Não requer autenticação.
    /// **Perfeito para montar o cardápio:** Use este endpoint para exibir produtos organizados por categoria.
    /// 
    /// **Categorias disponíveis:**
    /// - **0 = Lanche**: Hambúrguers, sanduíches, wraps
    /// - **1 = Acompanhamento**: Batatas fritas, anéis de cebola, nuggets
    /// - **2 = Bebida**: Refrigerantes, sucos, águas, cafés
    /// - **3 = Sobremesa**: Sorvetes, tortas, cookies, milk-shakes
    /// 
    /// **Como usar:**
    /// 1. Escolha a categoria desejada (0, 1, 2 ou 3)
    /// 2. Opcionalmente, configure a paginação usando pageSize e pageNumber
    /// 3. Receba a lista de produtos da categoria
    /// 
    /// **Exemplos de uso:**
    /// - `/category/0` - Todos os lanches (primeira página)
    /// - `/category/2?pageSize=5` - Bebidas, 5 por página
    /// - `/category/1?pageSize=20&pageNumber=2` - Acompanhamentos, página 2
    /// 
    /// **Dicas:**
    /// - Use pageSize pequeno (5-10) para interfaces móveis
    /// - Use pageSize maior (20-50) para interfaces desktop
    /// - Produtos são ordenados por data de criação (mais novos primeiro)
    /// </summary>
    /// <param name="category">Categoria dos produtos (0=Lanche, 1=Acompanhamento, 2=Bebida, 3=Sobremesa)</param>
    /// <param name="pageSize">Número de itens por página (padrão: 10, máximo recomendado: 50)</param>
    /// <param name="pageNumber">Número da página (padrão: 1, começa em 1)</param>
    /// <returns>Lista paginada de produtos da categoria especificada</returns>
    /// <response code="200">Lista de produtos encontrada - Pode estar vazia se a categoria não tiver produtos</response>
    /// <response code="400">Categoria inválida - Use valores de 0 a 3</response>
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
