using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Cep.LookupCep;

public class LookupCepQueryHandler : IRequestHandler<LookupCepQuery, LookupCepResponse>
{
    private readonly ICepService _cep;

    public LookupCepQueryHandler(ICepService cep)
    {
        _cep = cep;
    }

    public async Task<LookupCepResponse> Handle(LookupCepQuery request, CancellationToken ct)
    {
        var address = await _cep.LookupAsync(request.Cep, ct)
                       ?? throw new NotFoundException("CEP", request.Cep);

        return new LookupCepResponse(
            address.Cep,
            address.Street,
            address.Neighborhood,
            address.City,
            address.State);
    }
}
