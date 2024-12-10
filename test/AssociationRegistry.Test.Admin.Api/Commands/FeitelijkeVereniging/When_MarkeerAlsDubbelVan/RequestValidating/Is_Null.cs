namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan;
using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using FluentValidation.TestHelper;
using Framework;
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
              .WithErrorMessage($"'{nameof(MarkeerAlsDubbelVanRequest.IsDubbelVan)}' is verplicht.");
    }
}
