namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators;

using AssociationRegistry.Admin.Api.Verenigingen;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class When_Validating_A_RegistreerVerenigingRequest
{
    // NAAM
    public class Given_Name_Is_Empty_String : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__naam_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "" });

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
                .WithErrorMessage("'Naam' mag niet leeg zijn.")
                .Only();
        }
    }

    public class Given_Name_Is_Null : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__naam_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "OVO000001" });

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
                .WithErrorMessage("'Naam' is verplicht.");
        }
    }

    public class Given_A_Valid_Naam : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd" });

            result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
        }
    }

    // Initiator
    public class Given_Initiator_Is_Empty_String : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__naam_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "" });

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
                .WithErrorMessage("'Initiator' mag niet leeg zijn.")
                .Only();
        }
    }

    public class Given_Initiator_Is_Null : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__naam_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest());

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
                .WithErrorMessage("'Initiator' is verplicht.");
        }
    }

    public class Given_A_Valid_Initiator : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "abcd" });

            result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Initiator);
        }
    }

    // Valid
    public class Given_A_Valid_Request : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd", Initiator = "OVO000001"});

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
