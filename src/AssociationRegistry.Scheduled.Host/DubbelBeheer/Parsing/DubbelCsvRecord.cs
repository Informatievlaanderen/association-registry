namespace AssociationRegistry.Scheduled.Host.DubbelBeheer.Parsing;

using DecentraalBeheer.Vereniging;

public class DubbelCsvRecord
{
    public VCode DubbeleVCode { get; }
    public VCode AuthentiekeVCode { get; }

    public DubbelCsvRecord(string dubbeleVCode, string authentiekeVCode)
    {
        DubbeleVCode = VCode.Create(dubbeleVCode);
        AuthentiekeVCode = VCode.Create(authentiekeVCode);

        if (DubbeleVCode == AuthentiekeVCode)
            throw new ArgumentException($"DubbeleVCode en AuthentiekeVCode zijn gelijk ({DubbeleVCode}).");
    }
}
