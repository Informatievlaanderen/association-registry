namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_KboNumber;

using FluentValidation.TestHelper;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Valid
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd", KboNummer = "1234567890"});

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
