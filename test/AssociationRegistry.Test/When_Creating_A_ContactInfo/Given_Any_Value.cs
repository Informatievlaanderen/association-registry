namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using FluentAssertions;
using Xunit;

public class Given_Any_Value
{
    [Theory]
    [InlineData("em@a.il", null, null, null, false)]
    [InlineData(null, "-123456", null, null, true)]
    [InlineData(null, null, "http://web.site", null, false)]
    [InlineData(null, null, null, "http://so.cial", true)]
    [InlineData("eenem@a.il", "44/1/44", "https://een.website", "https://een.social", false)]
    [InlineData(null, "0125487", null, "http://@.me", true)]
    public void Then_it_returns_a_ContactInfo(string? email, string? telefoon, string? website, string? socialMedia, bool primairContactInfo)
    {
        const string contactnaam = "een naam";
        var contactinfo = ContactInfo.CreateInstance(contactnaam, email, telefoon, website, socialMedia, primairContactInfo);
        contactinfo.Contactnaam.Should().Be(contactnaam);
        contactinfo.SocialMedia?.ToString().Should().Be(socialMedia);
        contactinfo.Telefoon?.ToString().Should().Be(telefoon);
        contactinfo.Email?.ToString().Should().Be(email);
        contactinfo.Website?.ToString().Should().Be(website);
        contactinfo.PrimairContactInfo.Should().Be(primairContactInfo);
    }
}
