using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.TarificationService.Queries.GetById
{
    public record GetByIdQuery(Guid Id) : IRequest<TarifConsultationDto?>;
}
