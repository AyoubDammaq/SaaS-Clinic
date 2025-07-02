using MediatR;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Application.Queries.GetAllConsultations
{
    public record GetAllConsultationsQuery : IRequest<IEnumerable<Consultation>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetAllConsultationsQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
