namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using FluentAssertions;
using Xunit;
public class Given_Any_Value
{
    [Theory]
    [InlineData(null, "email", null, null, null)]
    [InlineData(null, null, "tel", null, null)]
    [InlineData(null, null, null, "website", null)]
    [InlineData(null, null, null, null, "social")]
    [InlineData("een naam", "een email", "een telefoon", "een website", "een social")]
    [InlineData("een mix", null, "0125487", null, "@me")]
    public void Then_it_returns_a_ContactInfo(string? contactnaam, string? email, string? telefoon, string? website, string? socialMedia)
    {
        var contactinfo = ContactInfo.CreateInstance(contactnaam, email, telefoon, website, socialMedia);
        contactinfo.Contactnaam.Should().Be(contactnaam);
        contactinfo.SocialMedia.Should().Be(socialMedia);
        contactinfo.Telefoon.Should().Be(telefoon);
        contactinfo.Email.Should().Be(email);
        contactinfo.Website.Should().Be(website);
    }
}
