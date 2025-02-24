namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Name_Length : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Naam = new string('A', 128);

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Locatie.Naam);
    }
}
