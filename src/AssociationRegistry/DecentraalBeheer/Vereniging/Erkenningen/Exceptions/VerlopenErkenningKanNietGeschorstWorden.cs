namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerlopenErkenningKanNietGeschorstWorden : DomainException
{
    public VerlopenErkenningKanNietGeschorstWorden()
        : base(ExceptionMessages.VerlopenErkenningKanNietGeschorstWorden) { }

    protected VerlopenErkenningKanNietGeschorstWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
