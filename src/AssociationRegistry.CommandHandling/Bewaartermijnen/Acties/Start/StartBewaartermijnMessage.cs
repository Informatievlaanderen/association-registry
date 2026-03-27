namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

public record StartBewaartermijnMessage(string VCode, string PersoonsgegevensType, int EntityId, string Reden);
