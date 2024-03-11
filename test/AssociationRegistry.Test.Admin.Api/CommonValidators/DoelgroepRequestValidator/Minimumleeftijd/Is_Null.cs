﻿namespace AssociationRegistry.Test.Admin.Api.CommonValidators.DoelgroepRequestValidator.Minimumleeftijd;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_errors()
    {
        var validator = new DoelgroepRequestValidator();

        var request = new DoelgroepRequest
        {
            Minimumleeftijd = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Minimumleeftijd);
    }
}
