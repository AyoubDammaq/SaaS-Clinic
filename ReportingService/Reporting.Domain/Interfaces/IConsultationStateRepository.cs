using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;

namespace Reporting.Domain.Interfaces
{
    public interface IConsultationStateRepository
    {
        Task<List<ConsultationDataModel>> GetAllConsultationsAsync(Guid? cliniqueId);
    }

}

