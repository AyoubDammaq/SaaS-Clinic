using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.LinkUserToDoctor
{
    public class LinkUserToDoctorCommandHandler : IRequestHandler<LinkUserToDoctorCommand, bool>
    {
        private readonly IMedecinRepository _medecinRepository;
        public LinkUserToDoctorCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<bool> Handle(LinkUserToDoctorCommand request, CancellationToken cancellationToken)
        {
            return await _medecinRepository.LinkUserToDoctorEntityAsync(request.LinkDTO.DoctorId, request.LinkDTO.UserId);
        }
    }
}
