namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_Multiple_Primaire_Locaties : ValidatorTest
{
    [Fact]
    public void Has_validation_error__niet_meer_dan_1_primaire_locatie()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Naam = "een",
                    IsPrimair = true,
                },
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Naam = "twee",
                    IsPrimair = true,
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}")
              .WithErrorMessage("Er mag maximum één primaire locatie opgegeven worden.");
    }
}
