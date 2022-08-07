public record AppConfiguration
{
    public string BaseAddress { get; init; } = null!;
    public string BearerToken { get; init; } = null!;
    public string[] ProjectIds { get; init; } = null!;
    public string? Filter { get; init; }
    public string ToSprint { get; init; } = null!;
    public int MaxDegreeOfParallelism { get; init; } = 1;
};