namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst.With_A_Contact.With_A_Contactnaam;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            ContactInfoLijst = new []{new ContactInfo
            {
                Contactnaam = null!,
            }},
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.ContactInfoLijst)}[0]" +
                                            $".{nameof(ContactInfo.Contactnaam)}")
            .WithErrorMessage("'Contactnaam' is verplicht.");
    }
}
