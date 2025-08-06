namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden : DomainException
{
    public AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden() : base(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden)
    {
    }

    protected AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
