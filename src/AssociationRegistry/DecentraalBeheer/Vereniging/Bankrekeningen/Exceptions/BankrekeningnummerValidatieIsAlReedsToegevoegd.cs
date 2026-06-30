namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class BankrekeningnummerValidatieIsAlReedsToegevoegd : DomainException
{
    public BankrekeningnummerValidatieIsAlReedsToegevoegd(string ovoCode)
        : base(string.Format(ExceptionMessages.BankrekeningnummerValidatieIsAlReedsToegevoegd, ovoCode)) { }

    protected BankrekeningnummerValidatieIsAlReedsToegevoegd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
