using FluentValidation;

namespace RDV.Application.Commands.CreateRendezVous
{
    public class CreateRendezVousCommandValidator : AbstractValidator<CreateRendezVousCommand>
    {
        public CreateRendezVousCommandValidator()
        {
            RuleFor(x => x.rendezVous)
                .NotNull().WithMessage("Le rendez-vous ne peut pas être nul.");

            RuleFor(x => x.rendezVous.PatientId)
                .NotEmpty().WithMessage("Le PatientId est obligatoire.");

            RuleFor(x => x.rendezVous.MedecinId)
                .NotEmpty().WithMessage("Le MedecinId est obligatoire.");

            RuleFor(x => x.rendezVous.DateHeure)
                .GreaterThan(DateTime.Now).WithMessage("La date du rendez-vous doit être ultérieure à maintenant.");
        }
    }
}
