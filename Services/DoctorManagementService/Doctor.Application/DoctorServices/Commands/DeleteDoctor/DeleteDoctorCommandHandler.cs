using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.DeleteDoctor
{
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand>
    {
        private readonly IMedecinRepository _medecinRepository;
        public DeleteDoctorCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.id));
            }
            await _medecinRepository.DeleteAsync(request.id);
        }
    }
}
