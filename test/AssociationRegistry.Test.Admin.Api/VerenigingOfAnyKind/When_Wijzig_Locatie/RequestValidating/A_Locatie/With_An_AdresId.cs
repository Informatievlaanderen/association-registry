﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_AdresId : ValidatorTest
{
    [Fact]
    public void Uses_Child_Validator()
    {
        var validator = new TeWijzigenLocatieValidator();

        validator.ShouldHaveChildValidator(expression: request => request.AdresId, typeof(AdresIdValidator));
    }
}
