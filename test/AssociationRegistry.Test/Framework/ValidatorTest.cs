namespace AssociationRegistry.Test.Framework;

using Baseline;
using FluentValidation;

public abstract class ValidatorTest
{
    public ValidatorTest()
    {
        // This seems to fix a nullpointerexception caused by running a TestServer and
        // these tests together. Probably through concurrent use of this Global variable.
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name.SplitPascalCase();
    }
}
