namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

public record StartBewaartermijnMessage(string VCode, string BewaartermijnType, int RecordId, string Reden);
