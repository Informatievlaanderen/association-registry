namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Too_short : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__naam_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd", KboNummer = "1234"});

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummer)
            .WithErrorMessage("KboNummer moet 10 cijfers bevatten.");
    }
}
