namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst.With_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Only_A_Contactnaam : ValidatorTest
{
    [Fact]
    public void Has_validation_error__minsten_1_waarde_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            ContactInfoLijst = new []{new ContactInfo
            {
                Contactnaam = "iets zinnig",
            }},
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.ContactInfoLijst)
            .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
    }
}
