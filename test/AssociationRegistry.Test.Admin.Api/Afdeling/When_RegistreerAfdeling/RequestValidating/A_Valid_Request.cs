namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using Fakes;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class A_Valid_Request : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerAfdelingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var result = validator.TestValidate(new RegistreerAfdelingRequest { Naam = "abcd", KboNummerMoedervereniging = "0123456789"});

        result.ShouldNotHaveAnyValidationErrors();
    }
}
