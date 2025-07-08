using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Commands.LinkUserToClinic
{
    public class LinkUserToClinicCommandHandler : IRequestHandler<LinkUserToClinicCommand, bool>
    {
        private readonly ICliniqueRepository _cliniqueRepository;
        public LinkUserToClinicCommandHandler(ICliniqueRepository cliniqueRepository)
        {
            _cliniqueRepository = cliniqueRepository;
        }
        public async Task<bool> Handle(LinkUserToClinicCommand request, CancellationToken cancellationToken)
        {
            var link = request.Link;
            return await _cliniqueRepository.LinkUserToClinicEntityAsync(link.ClinicId, link.UserId);
        }
    }
}
