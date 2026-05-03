using MediatR;

namespace TechsysLog.Application.Features.Cep.LookupCep;

public record LookupCepQuery(string Cep) : IRequest<LookupCepResponse>;

public record LookupCepResponse(
    string Cep,
    string Street,
    string Neighborhood,
    string City,
    string State);
