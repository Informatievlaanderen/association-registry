namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Locatie: ValidatorTest
{
    [Fact]
    public void Uses_ToeTeVoegenLocatieValidator()
    {
        var validator = new WijzigLocatieValidator();
        validator.ShouldHaveChildValidator(request => request.Locatie, typeof(ToeTeVoegenLocatieValidator));
    }
}
