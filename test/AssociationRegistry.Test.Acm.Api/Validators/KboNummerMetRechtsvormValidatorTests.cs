namespace AssociationRegistry.Test.Acm.Api.Validators;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using FluentValidation.TestHelper;
using Xunit;

public class KboNummerMetRechtsvormValidatorTests
{
    private readonly KboNummerMetRechtsvormValidator _validator;

    public KboNummerMetRechtsvormValidatorTests()
    {
        _validator = new KboNummerMetRechtsvormValidator();
    }

    [Fact]
    public void Has_ValidationError_For_KboNummer_When_Null()
    {
        var request = new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest {
            KboNummer = null,
            Rechtsvorm = "VZW",
        };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.KboNummer)
              .WithErrorMessage("'KboNummer' moet ingevuld zijn.");
    }

    [Fact]
    public void Has_ValidationError_For_KboNummer_When_Empty()
    {
        var request = new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest {
            KboNummer = "",
            Rechtsvorm = "VZW",
        };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.KboNummer)
              .WithErrorMessage("'KboNummer' mag niet leeg zijn.");
    }

    [Fact]
    public void Has_ValidationError_For_Rechtsvorm_When_Null()
    {
        var request = new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest {
            KboNummer = "012345",
            Rechtsvorm = null
        };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Rechtsvorm)
              .WithErrorMessage("'Rechtsvorm' moet ingevuld zijn.");
    }

    [Fact]
    public void Has_ValidationError_For_Rechtsvorm_When_Empty()
    {
        var request = new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest {
            KboNummer = "012345",
            Rechtsvorm = "",
        };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Rechtsvorm)
              .WithErrorMessage("'Rechtsvorm' mag niet leeg zijn.");
    }
}
