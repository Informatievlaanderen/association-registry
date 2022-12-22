namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_Validating.A_CreateVerenigingRequest.Given_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_One_Or_More_Values : ValidatorTest
{
    [Theory]
    [InlineData("em@il.com", "0123456", "www.ebsi.ti", "www.socialMedia.com")]
    [InlineData(null, null, "www.other.site", null)]
    [InlineData(null, null, null, "@media")]
    [InlineData(null, "9876543210", null, null)]
    [InlineData("yet@another.mail", null, null, null)]
    public void Then_it_has_no_validation_errors(string? email, string? telefoon, string? website, string? socialMedia)
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = new []{new RegistreerVerenigingRequest.ContactInfo
            {
                Email = email,
                Telefoon = telefoon,
                Website = website,
                SocialMedia = socialMedia,
            }},
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
