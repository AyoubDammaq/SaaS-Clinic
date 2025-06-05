
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AddDoctor
{
    public class AddDoctorCommandHandler : IRequestHandler<AddDoctorCommand>
    {
        private readonly IMedecinRepository _medecinRepository;

        public AddDoctorCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }

        public async Task Handle(AddDoctorCommand request, CancellationToken cancellationToken)
        {
            if (request.medecin == null)
            {
                throw new ArgumentNullException(nameof(request.medecin), "Le médecin ne peut pas être nul.");
            }

            request.medecin.AddDoctorEvent();

            await _medecinRepository.AddAsync(request.medecin);
        }
    }
}
