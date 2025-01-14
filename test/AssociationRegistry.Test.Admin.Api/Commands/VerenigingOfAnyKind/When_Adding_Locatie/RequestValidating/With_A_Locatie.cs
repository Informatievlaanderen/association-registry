﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Locatie : ValidatorTest
{
    [Fact]
    public void Uses_ToeTeVoegenLocatieValidator()
    {
        var validator = new VoegLocatieToeValidator();
        validator.ShouldHaveChildValidator(expression: request => request.Locatie, typeof(ToeTeVoegenLocatieValidator));
    }
}
