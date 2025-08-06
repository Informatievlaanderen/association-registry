namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_A_Locatie : ValidatorTest
{
    [Fact]
    public void Uses_ToeTeVoegenLocatieValidator()
    {
        var validator = new VoegLocatieToeValidator();
        validator.ShouldHaveChildValidator(expression: request => request.Locatie, typeof(ToeTeVoegenLocatieValidator));
    }
}
