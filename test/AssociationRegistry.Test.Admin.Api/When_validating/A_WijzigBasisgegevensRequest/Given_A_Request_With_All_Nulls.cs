namespace AssociationRegistry.Test.Admin.Api.When_validating.A_WijzigBasisgegevensRequest;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Request_With_All_Nulls
{
    [Fact]
    public void Then_it_should_have_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { Naam = null, KorteNaam = null, Initiator = ""});

        result.ShouldHaveValidationErrorFor("request")
            .WithErrorMessage("Een request mag niet leeg zijn.");
    }
}
