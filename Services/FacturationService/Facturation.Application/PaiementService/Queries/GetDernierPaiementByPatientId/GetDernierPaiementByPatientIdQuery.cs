using Facturation.Application.DTOs;
using Facturation.Domain.Entities;
using MediatR;

namespace Facturation.Application.PaiementService.Queries.GetDernierPaiementByPatientId
{
    public record GetDernierPaiementByPatientIdQuery(Guid PatientId) : IRequest<RecentPaiementDto?>;
}
