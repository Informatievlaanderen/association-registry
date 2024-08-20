<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Commands/VerenigingMetRechtspersoonlijkheid/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/Is_Valid.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    HoofdActiviteitenLijst;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.HoofdActiviteitenLijst;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Commands/FeitelijkeVereniging/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/Is_Valid.cs

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = new[] { "abcd" },
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.KorteBeschrijving);
    }
}
