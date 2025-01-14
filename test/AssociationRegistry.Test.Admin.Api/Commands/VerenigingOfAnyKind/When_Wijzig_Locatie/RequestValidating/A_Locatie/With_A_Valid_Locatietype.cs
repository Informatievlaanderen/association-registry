namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Vereniging;
using Xunit;

public class With_A_Valid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData(nameof(Locatietype.Correspondentie))]
    [InlineData(nameof(Locatietype.Activiteiten))]
    public void Has_NoValidationError_For_Locatietype(string locatietype)
    {
        var validator = new TeWijzigenLocatieValidator();
        var locatie = new Fixture().CustomizeAdminApi().Create<TeWijzigenLocatie>();
        locatie.Locatietype = locatietype;

        var result = validator.TestValidate(locatie);

        result.ShouldNotHaveValidationErrorFor(teWijzigenLocatie => teWijzigenLocatie.Locatietype);
    }
}
