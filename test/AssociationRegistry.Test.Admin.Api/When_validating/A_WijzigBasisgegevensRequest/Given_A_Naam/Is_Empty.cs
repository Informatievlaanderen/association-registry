namespace AssociationRegistry.Test.Admin.Api.When_validating.A_WijzigBasisgegevensRequest.Given_A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Then_it_has__validation_errors__naam_kan_niet_leeg_zijn()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { Naam = "" });

        result.ShouldHaveValidationErrorFor(request => request.Naam)
            .WithErrorMessage("'Naam' mag niet leeg zijn.");
    }
}
