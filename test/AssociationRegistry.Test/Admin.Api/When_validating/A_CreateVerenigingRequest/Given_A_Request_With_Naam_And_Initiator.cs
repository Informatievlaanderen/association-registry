namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Given_A_Valid_Request : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd", Initiator = "OVO000001" });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
