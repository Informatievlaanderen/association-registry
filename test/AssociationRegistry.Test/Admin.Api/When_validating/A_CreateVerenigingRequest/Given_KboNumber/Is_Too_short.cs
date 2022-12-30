namespace AssociationRegistry.Test.Admin.Api.When_Validating.A_CreateVerenigingRequest.Given_KboNumber;

using FluentValidation.TestHelper;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Too_short
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
