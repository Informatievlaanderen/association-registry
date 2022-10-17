namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using Marten;
using Weasel.Postgresql;

public static class SessionExtensions
{
    // A shorthand for generating the required SQL statement for a sequence value query
    public static int NextInSequence(this IQuerySession session, Sequence sequence)
        => session.Query<int>("select nextval(?)", sequence.Identifier.QualifiedName)[0];
}