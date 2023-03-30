﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Contactgegeven;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = Array.Empty<ContactInfo>(),
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
