namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OnbekendIpdcProductNummer : DomainException
{
    public OnbekendIpdcProductNummer(string ipdcProductNummer)
        : base(string.Format(ExceptionMessages.OnbekendIpdcProductNummer, ipdcProductNummer)) { }

    protected OnbekendIpdcProductNummer(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
