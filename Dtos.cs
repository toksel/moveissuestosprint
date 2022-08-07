using Newtonsoft.Json;

public record SprintValue()
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = null!;
    [JsonProperty(PropertyName = "$type")]
    public string Type { get; }= "VersionBundleElement";

    public override string ToString() => Name;
};

public record Sprint()
{
    [JsonProperty(PropertyName = "value")]
    public SprintValue[]? Values { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; } = "Sprints";
    [JsonProperty(PropertyName = "$type")]
    public string Type { get; } = "MultiVersionIssueCustomField";
};

public record CustomFields()
{
    [JsonProperty(PropertyName = "customFields")]
    public Sprint[]? Values { get; set; }
};

public record Issue(string Id, string Name, IEnumerable<SprintValue> Sprints)
{
    public override string ToString() =>
        $"Id: {Id}; Name: {Name}; Sprints: [{string.Join(", ", Sprints)}]";
}