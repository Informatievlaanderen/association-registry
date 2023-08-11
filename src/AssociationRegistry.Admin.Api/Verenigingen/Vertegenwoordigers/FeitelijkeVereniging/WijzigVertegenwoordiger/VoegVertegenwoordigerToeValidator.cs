namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;

using System.Linq;
using FluentValidation;
using RequestModels;

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

    private bool HaveAtLeastOneValue(TeWijzigenVertegenwoordiger vertegenwoordiger)
        => vertegenwoordiger
            .GetType()
            .GetProperties()
            .Any(property => property.GetValue(vertegenwoordiger) is not null);
}
