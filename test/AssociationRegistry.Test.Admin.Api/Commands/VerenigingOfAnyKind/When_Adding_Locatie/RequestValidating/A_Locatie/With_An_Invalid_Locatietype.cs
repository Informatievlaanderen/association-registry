namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Invalid_Locatietype : ValidatorTest
{
    [Fact]
    public void Has_validation_error__locatieType_moet_juiste_waarde_hebben()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Locatietype = new Fixture().Create<string>();
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Locatietype)}")
              .WithErrorMessage(
                   $"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})");
    }
}
