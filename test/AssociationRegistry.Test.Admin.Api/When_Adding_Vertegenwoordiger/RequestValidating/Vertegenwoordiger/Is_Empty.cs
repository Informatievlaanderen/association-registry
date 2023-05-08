﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__vertegenwoordiger_is_verplicht()
    {
        var validator = new VoegVertegenwoordigerToeValidator();
        var request = new VoegVertegenwoordigerToeRequest();
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Vertegenwoordiger)
            .WithErrorMessage("'Vertegenwoordiger' is verplicht.");
    }
}
