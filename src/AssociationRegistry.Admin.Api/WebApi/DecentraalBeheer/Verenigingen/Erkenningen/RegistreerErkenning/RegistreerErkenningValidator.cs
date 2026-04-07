namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentValidation;
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
            this.RequireNotNullOrEmpty(erkenning => erkenning.IpdcProductNummer);
         //   this.RequireNotEmpty(erkenning => erkenning.Startdatum);
          //this.RequireNotNullOrEmpty(erkenning => erkenning.Einddatum);
          //  this.RequireNotNullOrEmpty(erkenning => erkenning.Hernieuwingsdatum);
            this.RequireNotNullOrEmpty(erkenning => erkenning.HernieuwingsUrl);


        }
    }
}
