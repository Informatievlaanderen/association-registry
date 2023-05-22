﻿namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__vertegenwoordiger_is_verplicht()
    {
        var validator = new WijzigVertegenwoordigerValidator();
        var request = new WijzigVertegenwoordigerRequest()
        {
            Vertegenwoordiger = new WijzigVertegenwoordigerRequest.TeWijzigenVertegenwoordiger(),
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Vertegenwoordiger)
            .WithErrorMessage("'Vertegenwoordiger' moet ingevuld zijn.");
    }
}
