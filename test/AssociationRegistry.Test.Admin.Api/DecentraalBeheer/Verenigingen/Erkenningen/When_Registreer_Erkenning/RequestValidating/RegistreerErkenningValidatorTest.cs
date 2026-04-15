namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.
    RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class RegistreerErkenningValidatorTest : ValidatorTest
{
    [Fact]
    public void With_Erkenning_Null_Then_Has_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest()
        {
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
              .WithErrorMessage("'Erkenning' is verplicht.");
    }

    [Fact]
    public void With_HernieuwingsUrl_Start_With_XYZ_Null_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest()
        {
            Erkenning = new()
            {
                HernieuwingsUrl = "xyz.vlaanderen.be",
                IpdcProductNummer = "2144",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
              .WithErrorMessage("Url moet starten met 'http://' of 'https://.'");
    }

    [Fact]
    public void With_IpdcProductnummer_Null_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest()
        {
            Erkenning = new()
            {
                IpdcProductNummer = null,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning.IpdcProductNummer)
              .WithErrorMessage("Er was een probleem met de doorgestuurde waarden");
    }

    [Fact]
    public void With_Hernieuwingsdatum_Before_Start_And_EindDatum_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var request = new RegistreerErkenningRequest()
        {
            Erkenning = new()
            {
                Hernieuwingsdatum = today.AddDays(-20),
                Startdatum = today.AddDays(-5),
                Einddatum = today.AddDays(10),
                IpdcProductNummer = "8274",

            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
              .WithErrorMessage("Hernieuwingsdatum moet tussen startdatum en einddatum liggen.");
    }

    [Fact]
    public void With_Hernieuwingsdatum_After_Start_And_EindDatum_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var request = new RegistreerErkenningRequest()
        {
            Erkenning = new()
            {
                Hernieuwingsdatum = today.AddDays(-20),
                Startdatum = today.AddDays(1),
                Einddatum = today.AddDays(10),
                IpdcProductNummer = "8274",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
              .WithErrorMessage("Hernieuwingsdatum moet tussen startdatum en einddatum liggen.");
    }
}

// namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;
//
// using FluentValidation;
// using Infrastructure.WebApi.Validation;
// using RequestModels;
//
// // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// public class RegistreerErkenningValidator : AbstractValidator<RegistreerErkenningRequest>
// {
//     public RegistreerErkenningValidator()
//     {
//         RuleFor(request => request.Erkenning)
//            .NotNull()
//            .WithVeldIsVerplichtMessage(nameof(RegistreerErkenningRequest.Erkenning));
//
//         When(
//             predicate: request => request.Erkenning is not null,
//             action: () => RuleFor(request => request.Erkenning).SetValidator(new ErkenningValidator())
//         );
//     }
//
//     public class ErkenningValidator : AbstractValidator<TeRegistrerenErkenning>
//     {
//         public ErkenningValidator()
//         {
//             this.RequireNotNullOrEmpty(erkenning => erkenning.IpdcProductNummer);
//
//             RuleFor(erkenning => erkenning.HernieuwingsUrl)
//                .Must(url => url!.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
//                          || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
//                .When(erkenning => string.IsNullOrEmpty(erkenning.HernieuwingsUrl))
//                .WithMessage("Url moet starten met 'http://' of 'https://.'");
//
//             RuleFor(erkenning => erkenning)
//                .Must(e => e.Startdatum <= e.Hernieuwingsdatum && e.Hernieuwingsdatum <= e.Einddatum)
//                .WithMessage("Hernieuwingsdatum moet tussen startdatum en einddatum liggen.");
//         }
//     }
// }
