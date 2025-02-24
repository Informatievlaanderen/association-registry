namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Invalid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData("GeenGeldigLocatieType")]
    [InlineData("")]
    public void Has_ValidationError_For_Locatietype(string locatietype)
    {
        var validator = new TeWijzigenLocatieValidator();
        var locatie = new Fixture().CustomizeAdminApi().Create<TeWijzigenLocatie>();
        locatie.Locatietype = locatietype;

        var result = validator.TestValidate(locatie);

        result.ShouldHaveValidationErrorFor(teWijzigenLocatie => teWijzigenLocatie.Locatietype)
              .WithErrorMessage(
                   $"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})");
    }
}
