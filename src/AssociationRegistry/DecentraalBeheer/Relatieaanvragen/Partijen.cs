namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Exceptions;
using Framework;
using Vereniging;

/// <summary>
///     The koepel and the vereniging of a relatieaanvraag, and the aanvrager from vr-namens: the koepel or the vereniging.
///     The request alone tells whether they are valid, so they are checked before the verenigingen are loaded.
/// </summary>
public sealed record Partijen
{
    private Partijen(VCode koepelVCode, VCode verenigingVCode, VCode aangevraagdDoor)
    {
        KoepelVCode = koepelVCode;
        VerenigingVCode = verenigingVCode;
        AangevraagdDoor = aangevraagdDoor;
    }

    public VCode KoepelVCode { get; }
    public VCode VerenigingVCode { get; }
    public VCode AangevraagdDoor { get; }

    public static Partijen Create(VCode koepelVCode, VCode verenigingVCode, VCode aangevraagdDoor)
    {
        Throw<AanvragerIsGeenPartijBijRelatieaanvraag>.If(
            aangevraagdDoor != koepelVCode && aangevraagdDoor != verenigingVCode
        );

        Throw<VerenigingKanGeenRelatieMetZichzelfAanvragen>.If(koepelVCode == verenigingVCode);

        return new Partijen(koepelVCode, verenigingVCode, aangevraagdDoor);
    }
}
