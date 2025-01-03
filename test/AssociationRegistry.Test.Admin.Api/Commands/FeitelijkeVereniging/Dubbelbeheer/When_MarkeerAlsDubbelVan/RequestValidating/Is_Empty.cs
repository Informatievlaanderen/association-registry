namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.Dubbelbeheer.When_MarkeerAlsDubbelVan.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan;
using AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error_IsDubbelVan_is_verplicht()
    {
        var validator = new MarkeerAlsDubbelVanValidator();

        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = "",
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.IsDubbelVan)
              .WithErrorMessage($"'{nameof(MarkeerAlsDubbelVanRequest.IsDubbelVan)}' mag niet leeg zijn.");
    }
}
