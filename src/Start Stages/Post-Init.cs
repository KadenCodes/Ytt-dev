using Discord;
using Spectre.Console;
using SussyBot.Commands;

namespace SussyBot;

public sealed partial class StartStage
{
    public async Task PostInitialization()
    {
        AnsiConsole.MarkupLine("[maroon][[INFO]] Running [yellow]Post-Initialization[/].[/]");

        // TODO: Auto Completition of certain commands which might need it.
    }
}