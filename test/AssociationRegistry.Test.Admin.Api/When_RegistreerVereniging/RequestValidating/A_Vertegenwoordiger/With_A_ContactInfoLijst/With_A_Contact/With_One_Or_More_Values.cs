namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_A_ContactInfoLijst.With_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_One_Or_More_Values : ValidatorTest
{
    [Theory]
    [InlineData("em@il.com", "0123456", "www.ebsi.ti", "www.socialMedia.com")]
    [InlineData(null, null, "www.other.site", null)]
    [InlineData(null, null, null, "@media")]
    [InlineData(null, "9876543210", null, null)]
    [InlineData("yet@another.mail", null, null, null)]
    public void Has_no_validation_errors(string? email, string? telefoon, string? website, string? socialMedia)
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
                            Email = email,
                            Telefoon = telefoon,
                            Website = website,
                            SocialMedia = socialMedia,
                        },
                    },
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(nameof(request.Vertegenwoordigers) + "[0]." + nameof(RegistreerVerenigingRequest.Vertegenwoordiger.ContactInfoLijst));
    }
}
