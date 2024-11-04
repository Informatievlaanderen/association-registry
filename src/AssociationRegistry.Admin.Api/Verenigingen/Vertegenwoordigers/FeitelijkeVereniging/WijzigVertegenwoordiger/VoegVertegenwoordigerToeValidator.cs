namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;

using FluentValidation;
using Infrastructure.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigVertegenwoordigerValidator : AbstractValidator<WijzigVertegenwoordigerRequest>
{
    public WijzigVertegenwoordigerValidator()
    {
        RuleFor(request => request.Vertegenwoordiger).NotNull()
                                                     .WithVeldIsVerplichtMessage(nameof(WijzigVertegenwoordigerRequest.Vertegenwoordiger));
        When(
            predicate: request => request.Vertegenwoordiger is not null,
            action: () => RuleFor(request => request.Vertegenwoordiger)
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
