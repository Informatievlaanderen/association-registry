namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ValidatieBankrekeningnummerIsNietGekend : DomainException
{
    public ValidatieBankrekeningnummerIsNietGekend(string ovoCode)
        : base(string.Format(ExceptionMessages.ValidatieBankrekeningnummerIsNietGekend, ovoCode))
    {
    }

    protected ValidatieBankrekeningnummerIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
