namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Multiple_Hoofdlocaties : ValidatorTest
{
    [Fact]
    public void Has_validation_error__niet_meer_dan_1_hoofdlocatie()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Hoofdlocatie = true,
                },
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Hoofdlocatie = true,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Locaties)}")
            .WithErrorMessage("Er mag maximum één hoofdlocatie opgegeven worden.");
    }
}
