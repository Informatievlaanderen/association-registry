namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using AutoFixture;
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

    public class Given_Initiator_Is_Empty_String : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__initiator_mag_niet_leeg_zijn()
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
        public void Then_it_has_validation_error__initiator_is_verplicht()
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

    public class Given_An_Empty_Locaties_Array : ValidatorTest
    {
        [Fact]
        public void Then_it_has_no_validation_errors()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = Array.Empty<RegistreerVerenigingRequest.Locatie>(),
            };
            var result = validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class Given_A_Locaties_Array_With_Two_Identical_Locations : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__idenitiek_locaties_verboden()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var identiekLocatie = new RegistreerVerenigingRequest.Locatie
            {
                LocatieType = LocatieTypes.Activiteiten,
            };
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    identiekLocatie,
                    identiekLocatie,
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(vereniging => vereniging.Locaties)
                .WithErrorMessage("Identieke locaties zijn niet toegelaten.");
        }
    }

    public class Given_A_Locatie_Without_A_LocatieType : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__locatieType_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        Straatnaam = "dezeStraat",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.LocatieType)}")
                .WithErrorMessage("'LocatieType' is verplicht.");
        }
    }

    public class Given_A_Locatie_With_An_Empty_LocatieType : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__locatieType_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = string.Empty,
                        Straatnaam = "dezeStraat",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.LocatieType)}")
                .WithErrorMessage("'LocatieType' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_With_An_Invalid_LocatieType : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__locatieType_moet_juiste_waarde_hebben()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = new Fixture().Create<string>(),
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.LocatieType)}")
                .WithErrorMessage($"'LocatieType' moet een geldige waarde hebben. ({LocatieTypes.Correspondentie}, {LocatieTypes.Activiteiten}");
        }
    }

    public class Given_A_Locatie_With_A_Valid_LocatieType : ValidatorTest
    {
        [Theory]
        [InlineData(LocatieTypes.Correspondentie)]
        [InlineData(LocatieTypes.Activiteiten)]
        public void Then_it_has_no_validation_errors(string locationType)
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = locationType,
                        Straatnaam = "dezeStraat",
                        Huisnummer = "69"
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class Given_A_Locaties_Array_With_Multiple_Corresporentie_Locaties : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__niet_meer_dan_1_corresporentie_locatie()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Correspondentie,
                    },
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Correspondentie,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}")
                .WithErrorMessage("Er mag maximum één coresporentie locatie opgegeven worden.");
        }
    }

    public class Given_A_Locaties_Array_With_Multiple_HoofdLocaties : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__niet_meer_dan_1_hoofdlocatie()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        HoofdLocatie = true,
                    },
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        HoofdLocatie = true,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}")
                .WithErrorMessage("Er mag maximum één hoofdlocatie opgegeven worden.");
        }
    }

    public class Given_A_Locatie_With_An_Empty_Straatnaam : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__straatnaam_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = string.Empty,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Straatnaam)}")
                .WithErrorMessage("'Straatnaam' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_Without_A_Straatnaam : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__straatnaam_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Straatnaam)}")
                .WithErrorMessage("'Straatnaam' is verplicht.");
        }
    }
    public class Given_A_Locatie_With_An_Empty_Huisnummer : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__huisnummer_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Huisnummer = string.Empty,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Huisnummer)}")
                .WithErrorMessage("'Huisnummer' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_Without_A_Huisnummer : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__huisnummer_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Huisnummer)}")
                .WithErrorMessage("'Huisnummer' is verplicht.");
        }
    }
    public class Given_A_Locatie_With_An_Empty_Postcode : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__postcode_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Huisnummer = "69",
                        Postcode = "0123",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Postcode)}")
                .WithErrorMessage("'Postcode' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_Without_A_Postcode : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__huisnummer_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Huisnummer = "69"
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Postcode)}")
                .WithErrorMessage("'Postcode' is verplicht.");
        }
    }
    public class Given_A_Locatie_With_An_Empty_Gemeente : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__gemeente_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Gemeente = string.Empty,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Gemeente)}")
                .WithErrorMessage("'Gemeente' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_Without_A_Gemeente : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__gemeente_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Gemeente)}")
                .WithErrorMessage("'Gemeente' is verplicht.");
        }
    }
    public class Given_A_Locatie_With_An_Empty_Land : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__land_mag_niet_leeg_zijn()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Land = string.Empty,
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Land)}")
                .WithErrorMessage("'Land' mag niet leeg zijn.");
        }
    }

    public class Given_A_Locatie_Without_A_Land : ValidatorTest
    {
        [Fact]
        public void Then_it_has_validation_error__land_is_verplicht()
        {
            var validator = new RegistreerVerenigingRequestValidator();
            var request = new RegistreerVerenigingRequest
            {
                Naam = "abcd",
                Initiator = "OVO000001",
                Locaties = new[]
                {
                    new RegistreerVerenigingRequest.Locatie
                    {
                        LocatieType = LocatieTypes.Activiteiten,
                        Straatnaam = "Dezestraat",
                        Huisnummer = "69",
                        Postcode = "0123",
                        Gemeente = "Hottentot",
                    },
                },
            };
            var result = validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Land)}")
                .WithErrorMessage("'Land' is verplicht.");
        }
    }
}
