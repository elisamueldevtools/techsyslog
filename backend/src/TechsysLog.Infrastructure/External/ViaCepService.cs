using System.Net.Http.Json;
using System.Text.Json.Serialization;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.External;

public class ViaCepService : ICepService
{
    public const string HttpClientName = "ViaCep";

    private readonly IHttpClientFactory _factory;

    public ViaCepService(IHttpClientFactory factory) { _factory = factory; }

    public async Task<Address?> LookupAsync(string cep, CancellationToken ct)
    {
        var sanitized = new string(cep.Where(char.IsDigit).ToArray());
        if (sanitized.Length != 8) return null;

        var http = _factory.CreateClient(HttpClientName);
        var response = await http.GetAsync($"{sanitized}/json/", ct);
        if (!response.IsSuccessStatusCode) return null;

        var dto = await response.Content.ReadFromJsonAsync<ViaCepResponse>(cancellationToken: ct);
        if (dto is null || dto.Erro) return null;

        return new Address
        {
            Cep = sanitized,
            Street = dto.Logradouro ?? string.Empty,
            Neighborhood = dto.Bairro ?? string.Empty,
            City = dto.Localidade ?? string.Empty,
            State = dto.Uf ?? string.Empty
        };
    }

    private sealed class ViaCepResponse
    {
        [JsonPropertyName("logradouro")] public string? Logradouro { get; set; }
        [JsonPropertyName("bairro")]     public string? Bairro     { get; set; }
        [JsonPropertyName("localidade")] public string? Localidade { get; set; }
        [JsonPropertyName("uf")]         public string? Uf         { get; set; }
        [JsonPropertyName("erro")]       public bool   Erro        { get; set; }
    }
}
