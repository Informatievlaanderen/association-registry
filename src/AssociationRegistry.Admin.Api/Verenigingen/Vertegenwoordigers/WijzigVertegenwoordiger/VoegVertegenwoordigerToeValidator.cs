namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;

using System.Linq;
using FluentValidation;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigVertegenwoordigerValidator : AbstractValidator<WijzigVertegenwoordigerRequest>
{
    public WijzigVertegenwoordigerValidator()
    {
        RuleFor(request => request.Vertegenwoordiger).NotNull()
            .WithMessage("'Vertegenwoordiger' is verplicht.");
        When(
            request => request.Vertegenwoordiger is not null,
            () => RuleFor(request => request.Vertegenwoordiger)
                .Must(HaveAtLeastOneValue)
                .WithMessage("'Vertegenwoordiger' moet ingevuld zijn.")
        );
    }

    private bool HaveAtLeastOneValue(WijzigVertegenwoordigerRequest.TeWijzigenVertegenwoordiger vertegenwoordiger)
        => vertegenwoordiger
            .GetType()
            .GetProperties()
            .Any(property => property.GetValue(vertegenwoordiger) is not null);
}
