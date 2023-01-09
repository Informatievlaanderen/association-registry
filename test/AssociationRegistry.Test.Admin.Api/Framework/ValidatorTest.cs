namespace AssociationRegistry.Test.Admin.Api.Framework;

using FluentValidation;
using JasperFx.Core;
using Xunit.Categories;

[Category("Validator")]
public abstract class ValidatorTest
{
    protected ValidatorTest()
    {
        // This seems to fix a nullpointerexception caused by running a TestServer and
        // these tests together. Probably through concurrent use of this Global variable.
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name.SplitPascalCase() ?? "";
    }
}
