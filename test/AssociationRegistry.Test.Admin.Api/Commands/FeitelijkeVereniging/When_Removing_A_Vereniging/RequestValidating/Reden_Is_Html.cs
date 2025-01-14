﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Resources;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Reden_Is_Html : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_reden()
    {
        var validator = new VerwijderVerenigingRequestValidator();
        var result = validator.TestValidate(new VerwijderVerenigingRequest { Reden = "<p>Injecteer html</p>" });

        result.ShouldHaveValidationErrorFor(nameof(VerwijderVerenigingRequest.Reden))
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
