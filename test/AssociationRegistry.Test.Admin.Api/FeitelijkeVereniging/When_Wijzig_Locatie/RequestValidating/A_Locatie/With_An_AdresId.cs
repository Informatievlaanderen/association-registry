﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_AdresId : ValidatorTest
{

    [Fact]
    public void Uses_Child_Validator()
    {
        var validator = new TeWijzigenLocatieValidator();

        validator.ShouldHaveChildValidator(request => request.AdresId, typeof(AdresIdValidator));
    }
}
