namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VertegenwoordigerIsNietGekend : DomainException
{
    public VertegenwoordigerIsNietGekend(
        string vertegenwoordigerId) : base(
        $"Vertegenwoordiger met vertegenwoordigerId '{vertegenwoordigerId}' is niet gekend.")
    {
    }

    protected VertegenwoordigerIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
