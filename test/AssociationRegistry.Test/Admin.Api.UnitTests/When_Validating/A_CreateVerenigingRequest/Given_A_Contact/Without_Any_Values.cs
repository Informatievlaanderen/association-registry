namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_Validating.A_CreateVerenigingRequest.Given_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class Without_Any_Values
{
    [Fact]
    public void Then_it_has_validation_error__minsten_1_waarde_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = new []{new RegistreerVerenigingRequest.ContactInfo()},
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.ContactInfoLijst)
            .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
    }
}
