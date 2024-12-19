namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
