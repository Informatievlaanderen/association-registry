namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan;
using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new MarkeerAlsDubbelVanValidator();

        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = "V0001001",
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
