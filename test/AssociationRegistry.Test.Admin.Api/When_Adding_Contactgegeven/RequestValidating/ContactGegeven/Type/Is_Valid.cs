﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Vereniging;
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
                Contactgegeven = new VoegContactgegevenToeRequest.RequestContactgegeven
                    {
                        Type = ContactgegevenType.Email,
                    },
            });

        result.ShouldNotHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." + nameof(VoegContactgegevenToeRequest.RequestContactgegeven.Type));
    }
}
