<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Commands/FeitelijkeVereniging/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/IsNull.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.
========
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Commands/VerenigingMetRechtspersoonlijkheid/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/IsNull.cs
    HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class IsNull : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_hoofdactiviteitenLijst()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var request = new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket);
    }
}
