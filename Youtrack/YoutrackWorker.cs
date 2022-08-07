using YouTrackSharp;

public class YourtrackWorker
{
    private readonly BearerTokenConnection _connection;

    public YourtrackWorker(string baseAddress, string bearerToken)
    {
        _connection = new BearerTokenConnection(baseAddress, bearerToken);
    }

    public async Task<IEnumerable<Issue>> GetIssuesByFilter(string[] projectIds, string? filter)
    {
        var result = new List<Issue>();
        var issueService = _connection.CreateIssuesService();
        foreach (var projectId in projectIds)
        {
            var issues = await issueService.GetIssuesInProject(projectId, filter);
            var set = issues.Select(i =>
                {
                    var sprints = i.Fields
                        .Where(f => f.Name?.Equals("Sprints", StringComparison.CurrentCultureIgnoreCase) ?? false)
                        .FirstOrDefault()?
                        .Value as IEnumerable<string>;
                    if (sprints is null || !sprints!.Any())
                    {
                        sprints = Enumerable.Empty<string>();
                    }
                    return new Issue(i.Id, i.Summary, sprints!.Select(s => new SprintValue { Name = s }));
                });
            result.AddRange(set);
        }
        return result;
    }
}