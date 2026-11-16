namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using Common;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegVertegenwoordigerToeValidator : AbstractValidator<VoegVertegenwoordigerToeRequest>
{
    public VoegVertegenwoordigerToeValidator()
    {
        RuleFor(request => request.Vertegenwoordiger)
            .NotNull()
            .WithVeldIsVerplichtMessage(nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger));

        When(
            predicate: request => request.Vertegenwoordiger is not null,
            action: () =>
                RuleFor(request => request.Vertegenwoordiger).SetValidator(new ToeTeVoegenVertegenwoordigerValidator())
        );
    }
}
