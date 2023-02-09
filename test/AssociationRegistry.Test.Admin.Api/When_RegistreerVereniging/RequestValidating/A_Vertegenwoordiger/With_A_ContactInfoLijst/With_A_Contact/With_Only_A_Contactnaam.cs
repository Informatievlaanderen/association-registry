namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_A_ContactInfoLijst.With_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
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
            Naam = "abcd",
            Initiator = "OVO000001",
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    ContactInfoLijst = new[]
                    {
                        new RegistreerVerenigingRequest.ContactInfo
                        {
                            Contactnaam = "iets zinnig",
                        },
                    },
                },
            },
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor($"{nameof(request.Vertegenwoordigers)}[0].{nameof(RegistreerVerenigingRequest.Vertegenwoordiger.ContactInfoLijst)}[0]")
            .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
    }
}
