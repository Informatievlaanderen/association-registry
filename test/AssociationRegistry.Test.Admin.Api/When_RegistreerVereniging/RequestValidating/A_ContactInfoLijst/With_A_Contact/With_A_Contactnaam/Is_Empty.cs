namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst.With_A_Contact.With_A_Contactnaam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = new []{new RegistreerVerenigingRequest.ContactInfo
            {
                Contactnaam = string.Empty,
                Email = "info@something.be",
            }},
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.ContactInfoLijst)}[0]" +
                                            $".{nameof(RegistreerVerenigingRequest.ContactInfo.Contactnaam)}")
            .WithErrorMessage("'Contactnaam' mag niet leeg zijn.");
    }
}
