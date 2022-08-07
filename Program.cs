using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build()
    .Get<AppConfiguration>();

var youtrackWorker = new YourtrackWorker(config.BaseAddress, config.BearerToken);

var issues = await youtrackWorker.GetIssuesByFilter(config.ProjectIds, config.Filter);

Print($"Issues to move to sprint {config.ToSprint}", issues);
Console.WriteLine("Press any key to continue");
Console.ReadKey();
Console.WriteLine("Moving...");

using var youtrackApiWorker = new YoutrackApiWorker(config.BaseAddress, config.BearerToken);
foreach (var issue in issues)
    await youtrackApiWorker.MoveIssueToSprint(issue, config.ToSprint);

issues = await youtrackWorker.GetIssuesByFilter(config.ProjectIds, config.Filter);
Print($"Issues moved:", issues);

void Print(string header, IEnumerable<object> items)
{
    Console.WriteLine(header);
    foreach (var item in items)
        Console.WriteLine($"\t{item}");
}