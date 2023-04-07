﻿namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_moet_een_waarde_bevatten()
    {
        var validator = new WijzigContactgegevenValidator();
        var request = new WijzigContactgegevenRequest()
        {
            Contactgegeven = new WijzigContactgegevenRequest.RequestContactgegeven(),
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven)
            .WithErrorMessage("'Contactgegeven' moet ingevuld zijn.");
    }
}
