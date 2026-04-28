using RequestModels_TeRegistrerenErkenning =
    AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels.
    TeRegistrerenErkenning;

namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class SchorsErkenningValidator : AbstractValidator<SchorsErkenningRequest>
{
    public SchorsErkenningValidator()
    {
        RuleFor(request => request.Erkenning)
           .NotNull()
           .WithVeldIsVerplichtMessage(nameof(RegistreerErkenningRequest.Erkenning));

        When(
            predicate: request => request.Erkenning is not null,
            action: () => RuleFor(request => request.Erkenning).SetValidator(new ErkenningValidator())
        );
    }

    public class ErkenningValidator : AbstractValidator<TeSchorsenErkenning>
    {
        public ErkenningValidator()
        {
            this.RequireNotNullOrEmpty(erkenning => erkenning.RedenSchorsing);
        }
    }
}
