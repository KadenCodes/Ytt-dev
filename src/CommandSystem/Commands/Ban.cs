using Discord;
using Discord.WebSocket;
using Masked.DiscordNet;
using Masked.DiscordNet.Extensions;

namespace SussyBot.Commands;

public sealed class Ban : IDiscordCommand
{
    public readonly static Ban GlobalInstance = new();

    public async Task Run(SocketSlashCommand commandSocket)
    {
        // Defer the request to avoid time out.
        await commandSocket.DeferAsync();

        SocketUser moderator = commandSocket.User;
        SocketGuildUser user = ((SocketUser)commandSocket.Data.Options.ElementAt(0).Value).GetGuildUser();
        long purgeUserMessages = (long)commandSocket.Data.Options.ElementAt(1).Value > 7 ? 7 : (long)commandSocket.Data.Options.ElementAt(1).Value;
        string reason = "";
        string purgeString = "";
        if (commandSocket.Data.Options.Count == 3)
            reason = (string)commandSocket.Data.Options.ElementAt(2).Value;

        reason = reason.Length == 0 ? "*No reason provided*" : reason += '.';

        if (purgeUserMessages != 0)
            purgeString = $"\nAll the messages of the user from the past {purgeUserMessages} days have been purged.";

        EmbedBuilder embed = new()
        {
            Title = "User Banned!",
            Description = $"User {user.Username} has been banned!\nReason: {reason}\nAction taken by: {moderator.Username} ({moderator.Id}).{purgeString}"
        };
        Console.WriteLine(reason);
        await user.Guild.AddBanAsync(user, (int)purgeUserMessages, reason);

        await commandSocket.FollowupAsync(embed: embed.Build());
    }
    public SlashCommandProperties Build()
    {
        SlashCommandBuilder builder = new()
        {
            Name = "ban",
            Description = "Bans a user from the server",
            DefaultMemberPermissions = GuildPermission.BanMembers
        };
        builder.AddOptions(
            new SlashCommandOptionBuilder[] {
                new()
                {
                    Name = "user",
                    Type = ApplicationCommandOptionType.User,
                    Description = "The user that should be banned",
                    IsRequired = true
                },
                new()
                {
                    Name = "purgemessage",
                    Type = ApplicationCommandOptionType.Integer,
                    Description = "The age of the messages that should be purged 0 - 7",
                    IsRequired = true
                },
                new()
                {
                    Name = "reason",
                    Type = ApplicationCommandOptionType.String,
                    Description = "The reason to why the user was banned",
                    IsRequired = false
                }
            }
        );
        return builder.Build();
    }
}