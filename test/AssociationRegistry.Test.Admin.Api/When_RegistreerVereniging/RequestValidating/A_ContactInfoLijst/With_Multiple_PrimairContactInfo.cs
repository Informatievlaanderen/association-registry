namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class With_Multiple_PrimairContactInfo
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
                    Email = "master@outlook.com",
                    Telefoon = "9876543210",
                    Website = "www.master.lara",
                    SocialMedia = "#ScrumMaster",
                    PrimairContactInfo = true,
                },
                new()
                {
                    Email = "scrum@gmail.com",
                    Telefoon = "0000111222",
                    Website = "www.scrum.google",
                    SocialMedia = "#GoogleScrums",
                    PrimairContactInfo = true,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(v=>v.ContactInfoLijst)
            .WithErrorMessage("Er mag maximum één primair contactinfo opgegeven worden.");
    }
}
