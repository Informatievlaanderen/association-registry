namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;

using FluentValidation;
using RequestModels;
using Resources;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigErkenningValidator : AbstractValidator<WijzigErkenningRequest>
{
    public WijzigErkenningValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneTeWijzigenValue)
           .OverridePropertyName("request")
           .WithMessage(ExceptionMessages.MinstensEenVeldMoetIngevuldZijn);

        RuleFor(request => request.RedenVanWijziging)
           .NotNull()
           .NotEmpty()
           .WithMessage(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }

    private static bool HaveAtLeastOneTeWijzigenValue(WijzigErkenningRequest request) =>
        request.Startdatum.HasValue
     || request.Einddatum.HasValue
     || request.Hernieuwingsdatum.HasValue
     || request.HernieuwingsUrl is not null;
}
