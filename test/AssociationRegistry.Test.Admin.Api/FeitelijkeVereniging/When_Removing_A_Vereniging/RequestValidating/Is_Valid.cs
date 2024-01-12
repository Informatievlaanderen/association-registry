namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijderen;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijderen.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Reden_Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_reden()
    {
        var validator = new VerwijderVerenigingRequestValidator();
        var result = validator.TestValidate(new VerwijderVerenigingRequest { Reden = "Eender welke reden is goed!" });

        result.ShouldNotHaveValidationErrorFor(nameof(VerwijderVerenigingRequest.Reden));
    }
}
