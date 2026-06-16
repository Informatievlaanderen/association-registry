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
           .WithMessage(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn);

        RuleFor(request => request.RedenVanWijziging)
           .NotEmpty()
           .WithMessage(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }

    private static bool HaveAtLeastOneTeWijzigenValue(WijzigErkenningRequest request)
    {
        var startdatumHasValue = !request.Startdatum.IsNull;
        var einddatumHasValue = !request.Einddatum.IsNull;

        return startdatumHasValue
            || einddatumHasValue
            || request.Hernieuwingsdatum.HasValue
            || request.HernieuwingsUrl is not null;
    }
}
