using Facturation.Application.DTOs;
using Facturation.Domain.Enums;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFacturesByFilter
{
    public class GetFacturesByFilterQuery : IRequest<IEnumerable<FactureDto>>
    {
        public Guid? ClinicId { get; set; }
        public Guid? PatientId { get; set; }
        public FactureStatus? Status { get; set; }

        public GetFacturesByFilterQuery(Guid? clinicId, Guid? patientId, FactureStatus? status)
        {
            ClinicId = clinicId;
            PatientId = patientId;
            Status = status;
        }
    }
}
