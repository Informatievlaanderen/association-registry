﻿namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestValidating.A_ContactInfoLijst.With_A_Contact;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Test.Admin.Api.Framework;
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
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = new []{new ContactInfo
            {
                Email = email,
                Telefoon = telefoon,
                Website = website,
                SocialMedia = socialMedia,
            }},
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(request.ContactInfoLijst)}[0]");
    }
}
