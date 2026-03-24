namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Verlopen;

using NodaTime;

public record VerloopBewaartermijnCommand(
    string VCode,
    int VertegenwoordigerId,
    string Reden,
    Instant Vervaldag);
