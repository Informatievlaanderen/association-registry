namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;

/// <summary>
///     Sent by the job of the scheduled host for each relatieaanvraag whose vervaldatum is over, through a durable
///     Postgres queue. The aanvraagId is a plain string, like the vCode in the erkenningen commands: the handler makes
///     the value object.
/// </summary>
public record VervalRelatieaanvraagCommand(string AanvraagId);
