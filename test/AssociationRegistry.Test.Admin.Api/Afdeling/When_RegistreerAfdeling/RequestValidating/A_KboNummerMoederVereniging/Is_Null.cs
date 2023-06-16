namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_KboNummerMoederVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__KboNummerMoedervereniging_is_verplicht()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var result = validator.TestValidate(new RegistreerAfdelingRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummerMoedervereniging)
            .WithErrorMessage("'KboNummerMoedervereniging' is verplicht.");
    }
}
