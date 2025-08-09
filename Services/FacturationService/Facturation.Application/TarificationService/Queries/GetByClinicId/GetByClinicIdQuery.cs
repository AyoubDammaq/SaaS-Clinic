using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetByClinicId
{
    public record GetByClinicIdQuery(Guid cliniqueId) : IRequest<IEnumerable<TarifConsultationDto>>;
}
