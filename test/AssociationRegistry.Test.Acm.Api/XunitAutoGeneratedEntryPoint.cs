

using AssociationRegistry.Test.Acm.Api;

[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class XunitAutoGeneratedEntryPoint
{
    public static int Main(string[] args)
    {
        if (global::System.Linq.Enumerable.Any(args, arg => arg == "-automated" || arg == "@@"))
            return global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run(args).GetAwaiter().GetResult();
        else
            return global::Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.RunAsync(args, SelfRegisteredExtensions.AddSelfRegisteredExtensions).GetAwaiter().GetResult();
    }
}
