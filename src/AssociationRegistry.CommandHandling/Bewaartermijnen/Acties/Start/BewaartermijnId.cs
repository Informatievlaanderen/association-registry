namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

public record BewaartermijnId
{
    public BewaartermijnId(string VCode, int VertegenwoordigerId)
    {
        if (string.IsNullOrWhiteSpace(VCode))
            throw new ArgumentException(message: "Value cannot be null or whitespace.", nameof(VCode));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(VertegenwoordigerId);

        this.VCode = VCode;
        this.VertegenwoordigerId = VertegenwoordigerId;
    }

    public static implicit operator string(BewaartermijnId bewaartermijnId)
        => $"{bewaartermijnId.VCode}-{bewaartermijnId.VertegenwoordigerId}";

    public string VCode { get; }
    public int VertegenwoordigerId { get; }

    public void Deconstruct(out string vCode, out int vertegenwoordigerId)
    {
        vCode = VCode;
        vertegenwoordigerId = VertegenwoordigerId;
    }
}
