namespace AssociationRegistry.Admin.Api.Adapters.VCodeGeneration;

using Marten;
using Weasel.Postgresql;

public static class SessionExtensions
{
    // A shorthand for generating the required SQL statement for a sequence value query
    public static int NextInSequence(this IQuerySession session, Sequence sequence)
        => session.Query<int>(sql: "select nextval(?)", sequence.Identifier.QualifiedName)[index: 0];
}
