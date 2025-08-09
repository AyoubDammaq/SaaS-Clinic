using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetAll
{
    public record GetAllQuery() : IRequest<IEnumerable<TarifConsultationDto>>;
}
