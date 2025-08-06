namespace AssociationRegistry.Test.Acm.Api.Validators;

using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
using FluentValidation.TestHelper;
using Xunit;

public class VerenigingenPerInszRequestValidatorTests
{
    private readonly VerenigingenPerInszRequestValidator _validator;

    public VerenigingenPerInszRequestValidatorTests()
    {
        _validator = new VerenigingenPerInszRequestValidator();
    }

    [Fact]
    public void Has_ValidationError_For_Insz_When_Null()
    {
        var request = new VerenigingenPerInszRequest { Insz = null, KboNummers = []};
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Insz)
              .WithErrorMessage("'Insz' moet ingevuld zijn.");
    }

    [Fact]
    public void Has_ValidationError_For_Insz_When_Empty()
    {
        var request = new VerenigingenPerInszRequest { Insz = "", KboNummers = []};
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Insz)
              .WithErrorMessage("'Insz' mag niet leeg zijn.");
    }

    [Fact]
    public void Has_No_ValidationError_For_KboNummers_When_Null()
    {
        var request = new VerenigingenPerInszRequest { Insz = "123" };
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.KboNummers);
    }

    [Fact]
    public void Has_No_ValidationError_For_KboNummers_When_Empty()
    {
        var request = new VerenigingenPerInszRequest { Insz = "123", KboNummers = []};
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.KboNummers);
    }

    [Fact]
    public void Has_ChildValidator_For_KboNummers()
    {
        _validator.ShouldHaveChildValidator(r => r.KboNummers, typeof(KboNummerMetRechtsvormValidator));
    }
}
