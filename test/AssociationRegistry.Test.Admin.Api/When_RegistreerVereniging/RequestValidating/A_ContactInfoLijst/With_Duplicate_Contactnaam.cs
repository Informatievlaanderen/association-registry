namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Duplicate_Contactnaam
{
    [Fact]
    public void Has_validation_error_for_ContactInfoLijst()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            ContactInfoLijst = new ContactInfo[]
            {
                new()
                {
                    Contactnaam = "Algemeen",
                },
                new()
                {
                    Contactnaam = "Algemeen",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(v=>v.ContactInfoLijst)
            .WithErrorMessage("Een contactnaam moet uniek zijn.");
    }
}
