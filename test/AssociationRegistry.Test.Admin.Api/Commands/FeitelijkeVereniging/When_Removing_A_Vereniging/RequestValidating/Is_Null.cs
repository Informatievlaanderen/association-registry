﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Reden_Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_reden()
    {
        var validator = new VerwijderVerenigingRequestValidator();
        var result = validator.TestValidate(new VerwijderVerenigingRequest { Reden = null });

        result.ShouldHaveValidationErrorFor(nameof(VerwijderVerenigingRequest.Reden))
              .WithErrorMessage("'Reden' is verplicht.");
    }
}
