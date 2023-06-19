namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_KboNummerMoederVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__KboNummerMoedervereniging_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var result = validator.TestValidate(new RegistreerAfdelingRequest { KboNummerMoedervereniging = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummerMoedervereniging)
            .WithErrorMessage("'KboNummerMoedervereniging' mag niet leeg zijn.")
            .Only();
    }
}
