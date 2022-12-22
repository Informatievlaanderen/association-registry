namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

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