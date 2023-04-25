﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' is verplicht.");
    }
}
