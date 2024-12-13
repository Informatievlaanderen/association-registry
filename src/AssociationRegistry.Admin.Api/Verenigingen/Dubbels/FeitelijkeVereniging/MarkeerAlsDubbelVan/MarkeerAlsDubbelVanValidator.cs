﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan;

using FluentValidation;
using Infrastructure.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class MarkeerAlsDubbelVanValidator : AbstractValidator<MarkeerAlsDubbelVanRequest>
{
    public MarkeerAlsDubbelVanValidator()
    {
        RuleFor(r => r.IsDubbelVan)
           .NotNull()
           .NotEmpty()
           .WithVeldIsVerplichtMessage(nameof(MarkeerAlsDubbelVanRequest.IsDubbelVan));
    }
}
