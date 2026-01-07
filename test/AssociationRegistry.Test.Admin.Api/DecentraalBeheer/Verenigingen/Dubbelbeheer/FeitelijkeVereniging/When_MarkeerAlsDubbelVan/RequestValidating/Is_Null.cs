namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error_IsDubbelVan_is_verplicht()
    {
        var validator = new MarkeerAlsDubbelVanValidator();

        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = null,
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.IsDubbelVan)
              .WithErrorMessage($"'isDubbelVan' is verplicht.");
    }
}
