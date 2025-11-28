namespace CafeMenu.Domain.ValueObjects;

public record CurrencyCode
{
    public string Code { get; init; }

    private CurrencyCode(string code)
    {
        Code = code;
    }

    public static CurrencyCode FromString(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code cannot be null or empty", nameof(code));

        return new CurrencyCode(code.ToUpperInvariant());
    }

    public static CurrencyCode TRY => new("TRY");
    public static CurrencyCode USD => new("USD");
    public static CurrencyCode EUR => new("EUR");
}

