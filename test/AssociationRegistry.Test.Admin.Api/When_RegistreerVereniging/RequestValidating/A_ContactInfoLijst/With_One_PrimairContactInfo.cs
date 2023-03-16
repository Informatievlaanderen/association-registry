namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_One_PrimairContactInfo
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            ContactInfoLijst = new ContactInfo[]
            {
                new()
                {
                    Contactnaam = "Algemeen",
                    Email = "master@outlook.com",
                    Telefoon = "9876543210",
                    Website = "www.master.lara",
                    SocialMedia = "#ScrumMaster",
                    PrimairContactInfo = true,
                },
                new()
                {
                    Contactnaam = "Scrum",
                    Email = "scrum@gmail.com",
                    Telefoon = "0000111222",
                    Website = "www.scrum.google",
                    SocialMedia = "#GoogleScrums",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(v => v.ContactInfoLijst);
    }
}
