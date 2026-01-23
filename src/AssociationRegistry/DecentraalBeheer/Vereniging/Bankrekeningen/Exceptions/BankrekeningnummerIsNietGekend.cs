namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class BankrekeningnummerIsNietGekend: DomainException
{
    public BankrekeningnummerIsNietGekend(string bankrekeningnummerId) : base(string.Format(ExceptionMessages.BankrekeningnummerIsNietGekend, bankrekeningnummerId))
    {
    }

    protected BankrekeningnummerIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
