namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Verwijder;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijder.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
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
