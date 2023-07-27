namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class A_Doelgroep
{
    [Fact]
    public void Uses_DoelgroepValidator()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        validator.ShouldHaveChildValidator(request => request.Doelgroep, typeof(DoelgroepRequestValidator));
    }
}
