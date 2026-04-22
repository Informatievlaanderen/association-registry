namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;

using DecentraalBeheer.Vereniging.Erkenningen;
using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;
using Resources;
using TeRegistrerenErkenning = RequestModels.TeRegistrerenErkenning;

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
        }
    }
}
