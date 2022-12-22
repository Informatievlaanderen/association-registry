namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.ContactInfo;

using AssociationRegistry.ContactInfo;
using AssociationRegistry.ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_ContactInfo
{
    public class Given_All_Null
    {
        [Fact]
        public void Then_it_throws_an_NoContactInfoException()
        {
            var factory = () => ContactInfo.CreateInstance(null, null, null, null, null);
            factory.Should().Throw<NoContactInfo>();
        }
    }

    public class Given_Only_A_Name
    {
        [Fact]
        public void Then_it_throws_an_NoContactInfoException()
        {
            var factory = () => ContactInfo.CreateInstance(null, null, null, null, null);
            factory.Should().Throw<NoContactInfo>();
        }
    }

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
}
