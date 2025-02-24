namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.
    A_HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("ABCD", "ABCD")]
    [InlineData("Test", "tEST")]
    [InlineData("BLABLAbla", "BlAbLaBlA")]
    public void Has_a_validation_error_for_hoofdactiviteitenLijst(string hoofdactivitetiCode1, string hoofdactivitetiCode2)
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            HoofdactiviteitenVerenigingsloket = new[] { hoofdactivitetiCode1, hoofdactivitetiCode2 },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket)
              .WithErrorMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");
    }
}
