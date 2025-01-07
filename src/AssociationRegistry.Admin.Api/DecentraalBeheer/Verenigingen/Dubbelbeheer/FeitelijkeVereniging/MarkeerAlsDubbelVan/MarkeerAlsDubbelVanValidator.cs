namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class MarkeerAlsDubbelVanValidator : AbstractValidator<MarkeerAlsDubbelVanRequest>
{
    public MarkeerAlsDubbelVanValidator()
    {
        this.RequireNotNullOrEmpty(request => request.IsDubbelVan);
    }
}
