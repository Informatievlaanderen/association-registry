﻿namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_Validating.A_CreateVerenigingRequest.Given_A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = Array.Empty<RegistreerVerenigingRequest.ContactInfo>(),
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
