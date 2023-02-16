namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
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
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            HoofdactiviteitenVerenigingsLoket = new[] { hoofdactivitetiCode1, hoofdactivitetiCode2 },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsLoket)
            .WithErrorMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");
    }
}
