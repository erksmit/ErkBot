using ErkBot;
using ErkBot.Discord;

Console.WriteLine("Hello, World!");
// SEE https://github.com/spectreconsole/spectre.console

var configuration = Configuration.LoadConfiguration();
var client = new BotClient(configuration);

await client.Start();
Console.ReadLine();