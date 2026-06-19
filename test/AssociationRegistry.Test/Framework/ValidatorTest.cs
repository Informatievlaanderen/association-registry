namespace AssociationRegistry.Test.Framework;

using FluentValidation;

public abstract class ValidatorTest
{
    protected ValidatorTest()
    {
        // This seems to fix a nullpointerexception caused by running a TestServer and
        // these tests together. Probably through concurrent use of this Global variable.
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member is null ? null : SplitPascalCase(member.Name);
    }

    private static string SplitPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = new System.Text.StringBuilder(value.Length);
        result.Append(value[0]);

        for (var i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]) && !char.IsWhiteSpace(value[i - 1]))
                result.Append(' ');

            result.Append(value[i]);
        }

        return result.ToString();
    }
}
