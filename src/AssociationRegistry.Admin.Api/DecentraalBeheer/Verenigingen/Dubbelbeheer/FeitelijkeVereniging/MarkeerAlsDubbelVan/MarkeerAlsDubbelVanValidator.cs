namespace AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan;

using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class MarkeerAlsDubbelVanValidator : AbstractValidator<MarkeerAlsDubbelVanRequest>
{
    public MarkeerAlsDubbelVanValidator()
    {
        this.RequireNotNullOrEmpty(request => request.IsDubbelVan);
    }
}
