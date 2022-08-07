using System.Text;
using Newtonsoft.Json;

public class YoutrackApiWorker : IDisposable
{
    private readonly HttpClient _httpClient;

    public YoutrackApiWorker(string baseAddress, string bearerToken)
    {
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(baseAddress);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    public async Task MoveIssueToSprint(Issue issue, string sprint)
    {
        var endpoint = @$"/api/issues/{issue.Id}?fields=customFields(name,value(name))";

        var sprints = new CustomFields
        {
            Values = new Sprint[]
            {
                new Sprint
                {
                    Values = issue.Sprints
                        .Union(new[] { new SprintValue { Name = sprint } })
                        .ToArray()
                }
            }
        };

        var body = JsonConvert.SerializeObject(sprints);

        var data = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, data);
    }
}