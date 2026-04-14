namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class OnbekendIpdcProductNummer : DomainException
{
    public OnbekendIpdcProductNummer(string ipdcProductNummer)
        : base(string.Format(ExceptionMessages.IpdcProductNummerRequired, ipdcProductNummer)) { }

    protected OnbekendIpdcProductNummer(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
