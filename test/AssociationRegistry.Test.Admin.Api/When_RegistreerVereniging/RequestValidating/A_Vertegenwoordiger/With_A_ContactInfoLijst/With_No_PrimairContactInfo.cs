namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
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
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    ContactInfoLijst = new ContactInfo[]
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
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(v => v.ContactInfoLijst);
    }
}
