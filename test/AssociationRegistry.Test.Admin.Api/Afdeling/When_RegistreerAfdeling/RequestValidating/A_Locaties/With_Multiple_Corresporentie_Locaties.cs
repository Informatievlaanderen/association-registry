namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Multiple_Corresporentie_Locaties : ValidatorTest
{
    [Fact]
    public void Has_validation_error__niet_meer_dan_1_corresporentie_locatie()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Correspondentie,
                },
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Correspondentie,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}")
            .WithErrorMessage("Er mag maximum één correspondentie locatie opgegeven worden.");
    }
}
