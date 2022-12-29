namespace AssociationRegistry.Test.Admin.Api.When_Validating.A_CreateVerenigingRequest.Given_Locaties;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Constants;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class With_Multiple_HoofdLocaties : ValidatorTest
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
