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
           .Must(HasRedenVanWijzigingIsVerplicht)
           .OverridePropertyName("redenVanWijziging")
           .WithMessage(ExceptionMessages.RedenVanWijzigingIsVerplicht);

        RuleFor(request => request)
           .Must(HaveAtLeastOneTeWijzigenValue)
           .OverridePropertyName("request")
           .WithMessage(ExceptionMessages.MinstensEenVeldMoetIngevuldZijn);
    }

    private static bool HasRedenVanWijzigingIsVerplicht(WijzigErkenningRequest request) =>
         !string.IsNullOrWhiteSpace(request.HernieuwingsUrl);

    private static bool HaveAtLeastOneTeWijzigenValue(WijzigErkenningRequest request) =>
        request.Startdatum.HasValue
     || request.Einddatum.HasValue
     || request.Hernieuwingsdatum.HasValue
     || request.HernieuwingsUrl is not null;
}
