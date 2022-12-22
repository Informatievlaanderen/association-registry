namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Verenigingen;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Contact_With_Only_A_Contactnaam
{
    [Fact]
    public void Then_it_has_validation_error__minsten_1_waarde_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Contacten = new []{new RegistreerVerenigingRequest.ContactInfo
            {
                Contactnaam = "iets zinnig",
            }},
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Contacten)
            .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
    }
}
