using Masked.DiscordNet;

namespace SussyBot.Commands;

internal static class CommandLoader
{
    public static CommandHelper LoadCommands(CommandHelper cmdHelper)
    {
        cmdHelper.AddToCommandList(Ping.GlobalInstance);
        cmdHelper.AddToCommandList(Ban.GlobalInstance);
        cmdHelper.AddToCommandList(Reg.GlobalInstance);
        return cmdHelper;
    }

    public static Task LoadDataAsync() {
        Reg.LoadData();
        return Task.CompletedTask;
    }

    public static Task SaveDataAsync() {
        Reg.SaveData();
        return Task.CompletedTask;
    }
}
