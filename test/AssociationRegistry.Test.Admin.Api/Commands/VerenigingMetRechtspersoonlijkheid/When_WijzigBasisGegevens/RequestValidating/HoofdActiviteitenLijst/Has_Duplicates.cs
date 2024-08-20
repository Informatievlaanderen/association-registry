<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Commands/FeitelijkeVereniging/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/Has_Duplicates.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.
========
﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Commands/VerenigingMetRechtspersoonlijkheid/When_WijzigBasisGegevens/RequestValidating/HoofdActiviteitenLijst/Has_Duplicates.cs
    HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("ABCD", "ABCD")]
    [InlineData("Test", "tEST")]
    [InlineData("BLABLAbla", "BlAbLaBlA")]
    public void Has_a_validation_error_for_hoofdactiviteitenLijst(string hoofdactivitetiCode1, string hoofdactivitetiCode2)
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var request = new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = new[] { hoofdactivitetiCode1, hoofdactivitetiCode2 },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket)
              .WithErrorMessage("Elke waarde in de hoofdactiviteiten mag slechts 1 maal voorkomen.");
    }
}
