namespace AssociationRegistry.Hosts;

public static class ProgramArguments
{
    public static bool IsCodeGen(string[] args)
        => args.Contains("codegen", StringComparer.InvariantCultureIgnoreCase);
}
