using Facturation.Domain.Entities;
using MediatR;

namespace Facturation.Application.FactureService.Commands.ExportToPdf
{
    public record ExportToPdfCommand(Facture facture) : IRequest<byte[]>;
}
