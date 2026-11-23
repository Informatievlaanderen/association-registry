namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class BeoogdeKoepelIsNietAlsKoepelGeidentificeerd : DomainException
{
    public BeoogdeKoepelIsNietAlsKoepelGeidentificeerd()
        : base(ExceptionMessages.BeoogdeKoepelIsNietAlsKoepelGeidentificeerd) { }

    protected BeoogdeKoepelIsNietAlsKoepelGeidentificeerd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
