namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;

/// <summary>
///     The vr-namens header: the vCode of the vereniging on whose behalf the request is made. Required for the
///     relatieacties (indienen, bevestigen, weigeren, intrekken).
/// </summary>
public static class VrNamensHeader
{
    public static VCode Verplicht(string? waarde)
    {
        if (string.IsNullOrWhiteSpace(waarde))
            throw new VrNamensIsVerplicht();

        return VCode.Create(waarde.Trim());
    }
}

public class VrNamensIsVerplicht : DomainException
{
    public VrNamensIsVerplicht()
        : base($"{WellknownHeaderNames.Namens} is verplicht.") { }
}
