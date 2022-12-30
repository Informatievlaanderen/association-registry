namespace AssociationRegistry.Test.Admin.Api.When_Validating.A_CreateVerenigingRequest.Given_Naam;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__naam_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
            .WithErrorMessage("'Naam' mag niet leeg zijn.")
            .Only();
    }
}
