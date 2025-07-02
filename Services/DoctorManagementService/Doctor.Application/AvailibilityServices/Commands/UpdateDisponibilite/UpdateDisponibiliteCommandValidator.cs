using FluentValidation;

namespace Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite
{
    public class UpdateDisponibiliteCommandValidator : AbstractValidator<UpdateDisponibiliteCommand>
    {
        public UpdateDisponibiliteCommandValidator()
        {
            RuleFor(x => x.disponibilite)
                .NotNull()
                .WithMessage("Les données de la disponibilité sont requises.");

            RuleFor(x => x.disponibilite.HeureDebut)
                .LessThan(x => x.disponibilite.HeureFin)
                .WithMessage("L'heure de début doit être inférieure à l'heure de fin.");

            RuleFor(x => x.disponibilite.Jour)
                .IsInEnum()
                .WithMessage("Le jour spécifié n'est pas valide.");

            RuleFor(x => x.disponibilite.MedecinId)
                .NotEmpty()
                .WithMessage("Le médecin est requis.");
        }
    }
}
