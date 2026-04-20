namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OngeldigIpdcProductNummer : DomainException
{
    public OngeldigIpdcProductNummer(string ipdcProductNummer)
        : base(string.Format(ExceptionMessages.OngeldigIpdcProductNummer, ipdcProductNummer)) { }

    protected OngeldigIpdcProductNummer(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
