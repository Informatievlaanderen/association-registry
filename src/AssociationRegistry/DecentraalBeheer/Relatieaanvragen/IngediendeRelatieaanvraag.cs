namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;

/// <summary>
///     A relatieaanvraag that its aanvrager can still withdraw: it is Aangevraagd and its vervaldatum is not over. On the
///     vervaldatum itself it is still possible.
/// </summary>
public sealed class IngediendeRelatieaanvraag
{
    private readonly RelatieaanvraagState _relatieaanvraag;

    private IngediendeRelatieaanvraag(RelatieaanvraagState relatieaanvraag)
    {
        _relatieaanvraag = relatieaanvraag;
    }

    /// <param name="namens">The vCode from vr-namens: must be the aanvrager.</param>
    public static IngediendeRelatieaanvraag Create(RelatieaanvraagState relatieaanvraag, VCode namens, DateOnly today)
    {
        Throw<NamensIsNietDeAanvrager>.If(namens != relatieaanvraag.AangevraagdDoor);
        relatieaanvraag.ThrowIfNotOpen(today);

        return new IngediendeRelatieaanvraag(relatieaanvraag);
    }

    /// <summary>
    ///     Withdrawing checks no more than <see cref="Create" />: not the koepel, not the rules of the type. Nothing changes
    ///     on the verenigingen. The caller saves the event.
    /// </summary>
    /// <param name="reden">Optional: empty when none was given.</param>
    public RelatieaanvraagWerdIngetrokken TrekIn(string reden, DateOnly today) =>
        new(_relatieaanvraag.AanvraagId.Value, today, reden);
}
