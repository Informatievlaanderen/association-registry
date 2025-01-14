﻿namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;

using Common;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegLocatieToeValidator : AbstractValidator<VoegLocatieToeRequest>
{
    public VoegLocatieToeValidator()
    {
        RuleFor(request => request.Locatie)
           .SetValidator(new ToeTeVoegenLocatieValidator());
    }
}
