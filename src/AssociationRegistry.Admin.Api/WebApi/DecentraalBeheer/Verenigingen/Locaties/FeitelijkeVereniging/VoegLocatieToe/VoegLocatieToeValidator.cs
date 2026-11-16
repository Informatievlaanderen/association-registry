namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using Common;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegLocatieToeValidator : AbstractValidator<VoegLocatieToeRequest>
{
    public VoegLocatieToeValidator()
    {
        RuleFor(request => request.Locatie).NotNull().WithVeldIsVerplichtMessage(nameof(VoegLocatieToeRequest.Locatie));

        RuleFor(request => request.Locatie).SetValidator(new ToeTeVoegenLocatieValidator());
    }
}
