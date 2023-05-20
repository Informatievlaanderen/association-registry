namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Invalid_Locatietype : ValidatorTest
{
    [Fact]
    public void Has_validation_error__locatieType_moet_juiste_waarde_hebben()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = new Fixture().Create<string>(),
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Locatietype)}")
            .WithErrorMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietypes.Correspondentie}, {Locatietypes.Activiteiten}");
    }
}
