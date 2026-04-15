namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;

using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class RegistreerErkenningValidator : AbstractValidator<RegistreerErkenningRequest>
{
    public RegistreerErkenningValidator()
    {
        RuleFor(request => request.Erkenning)
           .NotNull()
           .WithVeldIsVerplichtMessage(nameof(RegistreerErkenningRequest.Erkenning));

        When(
            predicate: request => request.Erkenning is not null,
            action: () => RuleFor(request => request.Erkenning).SetValidator(new ErkenningValidator())
        );
    }

    public class ErkenningValidator : AbstractValidator<TeRegistrerenErkenning>
    {
        public ErkenningValidator()
        {
            RuleFor(erkenning => erkenning.IpdcProductNummer)
               .Must(nummer => !string.IsNullOrEmpty(nummer))
               .WithMessage("Er was een probleem met de doorgestuurde waarden");

            RuleFor(erkenning => erkenning)
               .Must(erkenning => erkenning.HernieuwingsUrl!.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                               || erkenning.HernieuwingsUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
               .When(erkenning => !string.IsNullOrEmpty(erkenning.HernieuwingsUrl))
               .WithMessage("Url moet starten met 'http://' of 'https://.'");

            RuleFor(erkenning => erkenning)
               .Must(e => e.Startdatum <= e.Hernieuwingsdatum && e.Hernieuwingsdatum <= e.Einddatum)
               .WithMessage("Hernieuwingsdatum moet tussen startdatum en einddatum liggen.");
        }
    }
}
