namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Reacties.VerwijderVertegenwoordigerPersoonsgegevens;

using NodaTime;

public record VerwijderVertegenwoordigerPersoonsgegevensMessage(
    string BewaartermijnId,
    string VCode,
    int VertegenwoordigerId,
    Instant Vervaldag,
    string reden);
