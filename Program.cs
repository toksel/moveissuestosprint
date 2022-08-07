using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build()
    .Get<AppConfiguration>();

var cts = new CancellationTokenSource();

AppDomain.CurrentDomain.ProcessExit += (sender, e) => cts.Cancel();

var youtrackWorker = new YourtrackWorker(config.BaseAddress, config.BearerToken);

var issues = await youtrackWorker.GetIssuesByFilter(config.ProjectIds, config.Filter);

Print($"Issues to move to sprint {config.ToSprint}", issues);
Console.WriteLine("Press any key to continue");
Console.ReadKey();
Console.WriteLine("Moving...");

using var youtrackApiWorker = new YoutrackApiWorker(config.BaseAddress, config.BearerToken);

var flowOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = config.MaxDegreeOfParallelism };

var actionBlock = new ActionBlock<Issue>(issue => 
    youtrackApiWorker.MoveIssueToSprint(issue, config.ToSprint, cts.Token),
    flowOptions);

foreach (var issue in issues)
    actionBlock.Post(issue);

actionBlock.Complete();
await actionBlock.Completion.WaitAsync(cts.Token);

issues = await youtrackWorker.GetIssuesByFilter(config.ProjectIds, config.Filter);
Print($"Issues moved:", issues);

void Print(string header, IEnumerable<object> items)
{
    Console.WriteLine(header);
    foreach (var item in items)
        Console.WriteLine($"\t{item}");
}