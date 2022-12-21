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
            var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd", Initiator = "OVO000001" });

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class Given_An_Empty_Contacten_Array : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Contacten = Array.Empty<RegistreerVerenigingRequest.ContactInfo>(),
            };
            var result = validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class Given_A_Contact_Without_Any_Values
    {
        [Fact]
        public void Then_it_has_validation_error__minsten_1_waarde_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Contacten = new []{new RegistreerVerenigingRequest.ContactInfo()},
            };

            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Contacten)
                .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
        }
    }

    public class Given_A_Contact_With_Only_A_Contactnaam
    {
        [Fact]
        public void Then_it_has_validation_error__minsten_1_waarde_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Contacten = new []{new RegistreerVerenigingRequest.ContactInfo
                {
                    Contactnaam = "iets zinnig",
                }},
            };

            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Contacten)
                .WithErrorMessage("Een contact moet minstens één waarde bevatten.");
        }
    }

    public class Given_An_Contact_With_One_Or_More_Values : ValidatorTest
    {
        [Theory]
        [InlineData("em@il.com", "0123456", "www.ebsi.ti", "www.socialMedia.com")]
        [InlineData(null, null, "www.other.site", null)]
        [InlineData(null, null, null, "@media")]
        [InlineData(null, "9876543210", null, null)]
        [InlineData("yet@another.mail", null, null, null)]
        public void Then_it_has_no_validation_errors(string? email, string? telefoon, string? website, string? socialMedia)
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Contacten = new []{new RegistreerVerenigingRequest.ContactInfo
                {
                    Email = email,
                    Telefoon = telefoon,
                    Website = website,
                    SocialMedia = socialMedia,
                }},
            };
            var result = validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

}
