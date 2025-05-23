namespace FastFood.Api.Models;

/// <summary>
/// Modelo de resposta para erros na API.
/// </summary>
public class ErrorResponse
{
    public ErrorResponse()
    {
        Errors = new List<string>();
    }

    public ErrorResponse(string error) : this()
    {
        Errors.Add(error);
    }

    public ErrorResponse(IEnumerable<string> errors) : this()
    {
        Errors.AddRange(errors);
    }    /// <summary>
    /// Lista de mensagens de erro.
    /// </summary>
    public List<string> Errors { get; set; }

    /// <summary>
    /// Identificador de rastreamento da requisição.
    /// </summary>
    public string? TraceId { get; set; }
}
