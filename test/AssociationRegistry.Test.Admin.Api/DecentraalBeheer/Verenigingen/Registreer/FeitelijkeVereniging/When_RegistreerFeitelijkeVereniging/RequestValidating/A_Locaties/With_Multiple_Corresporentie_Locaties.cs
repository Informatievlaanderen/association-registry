namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class With_Multiple_Corresporentie_Locaties : ValidatorTest
{
    [Fact]
    public void Has_validation_error__niet_meer_dan_1_corresporentie_locatie()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Correspondentie,
                },
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Correspondentie,
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}")
              .WithErrorMessage("Er mag maximum één correspondentie locatie opgegeven worden.");
    }
}
