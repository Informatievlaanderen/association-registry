namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

public class With_Multiple_Corresporentie_Locaties : ValidatorTest
{
    [Fact]
    public void Has_validation_error__niet_meer_dan_1_corresporentie_locatie()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
                {
                    Locatietype = Locatietypes.Correspondentie,
                },
                new RegistreerVerenigingRequest.Locatie
                {
                    Locatietype = Locatietypes.Correspondentie,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}")
            .WithErrorMessage("Er mag maximum één coresporentie locatie opgegeven worden.");
    }
}
