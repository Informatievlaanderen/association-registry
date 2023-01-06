namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_Initiator;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__initiator_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' is verplicht.");
    }
}
