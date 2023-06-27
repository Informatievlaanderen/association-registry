namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;

using Common;
using FluentValidation;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegLocatieToeValidator : AbstractValidator<VoegLocatieToeRequest>
{
    public VoegLocatieToeValidator()
    {
        RuleFor(request => request.Locatie)
            .SetValidator(new ToeTeVoegenLocatieValidator());
    }
}
