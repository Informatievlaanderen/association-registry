namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_No_PrimairContactInfo:ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
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

        result.ShouldNotHaveValidationErrorFor(v=>v.ContactInfoLijst);
    }
}
