namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Exceptions;
using Framework;
using Vereniging;

/// <summary>
///     The koepel of a relatieaanvraag: a KBO-vereniging identified as koepel. The messages are the same whoever asks.
/// </summary>
public sealed class BeoogdeKoepel
{
    private BeoogdeKoepel(VCode vCode, string naam)
    {
        VCode = vCode;
        Naam = naam;
    }

    public VCode VCode { get; }

    /// <summary>The naam of the koepel, kept in the relatie on the vereniging when the relatieaanvraag is confirmed.</summary>
    public string Naam { get; }

    public static BeoogdeKoepel Create(VerenigingState koepel)
    {
        Throw<BeoogdeKoepelMoetKboVerenigingZijn>.IfNot(Verenigingstype.IsKboVereniging(koepel.Verenigingstype.Code));
        Throw<BeoogdeKoepelIsNietAlsKoepelGeidentificeerd>.IfNot(koepel.Koepelgegevens.Koepelstatus.IsKoepel);

        return new BeoogdeKoepel(koepel.VCode, koepel.Naam);
    }
}
