namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_Multiple_Corresporentie_Locaties : ValidatorTest
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
