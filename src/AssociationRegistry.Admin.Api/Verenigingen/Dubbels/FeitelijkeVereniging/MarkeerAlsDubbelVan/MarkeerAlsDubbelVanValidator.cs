namespace AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan;

using FluentValidation;
using Infrastructure.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class MarkeerAlsDubbelVanValidator : AbstractValidator<MarkeerAlsDubbelVanRequest>
{
    public MarkeerAlsDubbelVanValidator()
    {
        this.RequireNotNullOrEmpty(request => request.IsDubbelVan);
    }
}
