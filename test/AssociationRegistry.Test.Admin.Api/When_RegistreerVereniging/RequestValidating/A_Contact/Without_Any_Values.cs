namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Any_Values : ValidatorTest
{
    [Fact]
    public void Has_validation_error__minsten_1_waarde_is_verplicht()
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
