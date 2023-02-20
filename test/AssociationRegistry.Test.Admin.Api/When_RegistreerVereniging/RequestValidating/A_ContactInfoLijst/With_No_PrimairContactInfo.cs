namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class With_No_PrimairContactInfo
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
            {
                new()
                {
                    Email = "master@outlook.com",
                    Telefoon = "9876543210",
                    Website = "www.master.lara",
                    SocialMedia = "#ScrumMaster",
                },
                new()
                {
                    Email = "scrum@gmail.com",
                    Telefoon = "0000111222",
                    Website = "www.scrum.google",
                    SocialMedia = "#GoogleScrums",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(v=>v.ContactInfoLijst);
    }
}
