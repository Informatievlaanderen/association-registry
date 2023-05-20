﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Contactgegeven.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new ToeTeVoegenContactgegeven
                    {
                        Waarde = "test waarde",
                    },
            });

        result.ShouldNotHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." + nameof(ToeTeVoegenContactgegeven.Waarde));
    }
}
