namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using FluentAssertions;
using Xunit;

public class Given_Any_Value
{
    [Theory]
    [InlineData(null, "email", null, null, null, false)]
    [InlineData(null, null, "tel", null, null, true)]
    [InlineData(null, null, null, "website", null, false)]
    [InlineData(null, null, null, null, "social", true)]
    [InlineData("een naam", "een email", "een telefoon", "een website", "een social", false)]
    [InlineData("een mix", null, "0125487", null, "@me", true)]
    public void Then_it_returns_a_ContactInfo(string? contactnaam, string? email, string? telefoon, string? website, string? socialMedia, bool primairContactInfo)
    {
        var contactinfo = ContactInfo.CreateInstance(contactnaam, email, telefoon, website, socialMedia, primairContactInfo);
        contactinfo.Contactnaam.Should().Be(contactnaam);
        contactinfo.SocialMedia.Should().Be(socialMedia);
        contactinfo.Telefoon.Should().Be(telefoon);
        contactinfo.Email.Should().Be(email);
        contactinfo.Website.Should().Be(website);
        contactinfo.PrimairContactInfo.Should().Be(primairContactInfo);
    }
}
