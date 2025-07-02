using FluentValidation;

namespace Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite
{
    public class AjouterDisponibiliteCommandValidator : AbstractValidator<AjouterDisponibiliteCommand>
    {
        public AjouterDisponibiliteCommandValidator()
        {
            RuleFor(x => x.nouvelleDispo)
                .NotNull()
                .WithMessage("Les données de la disponibilité sont requises.");

            RuleFor(x => x.nouvelleDispo.HeureDebut)
                .LessThan(x => x.nouvelleDispo.HeureFin)
                .WithMessage("L'heure de début doit être inférieure à l'heure de fin.");

            RuleFor(x => x.nouvelleDispo.Jour)
                .IsInEnum()
                .WithMessage("Le jour spécifié n'est pas valide.");

            RuleFor(x => x.nouvelleDispo.MedecinId)
                .NotEmpty()
                .WithMessage("Le médecin est requis.");
        }
    }
}
