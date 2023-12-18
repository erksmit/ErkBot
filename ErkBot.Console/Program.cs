using ErkBot;

Console.WriteLine("Hello, World!");
// SEE https://github.com/spectreconsole/spectre.console

var configuration = Configuration.LoadConfiguration();
var client = new Client(configuration);

await client.Start();
Console.ReadLine();