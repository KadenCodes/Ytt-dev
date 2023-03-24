using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Masked.DiscordNet;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Masked.DiscordNet.Extensions;
using System.Text.Json;
using SussyBot.BlobAPI;
using Discord.Interactions;

namespace SussyBot.Commands {
    public class Reg : IDiscordCommand {
        public static readonly Reg GlobalInstance = new(); 
        private static readonly string _filePath = "data.json";
        private static readonly Dictionary<ulong, Player> userIDs = LoadData();
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public static Dictionary<ulong, Player> LoadData() {
            if (!File.Exists(_filePath)) 
                return new();
                
            var json = File.ReadAllText(_filePath);

            if (!json.Contains('{') && !json.Contains('}'))
                return new Dictionary<ulong, Player>();
            return JsonSerializer.Deserialize<Dictionary<ulong, Player>>(json, Shared.JsonSGParsers.Default.DictionaryUInt64Player);
        }

        public static void SaveData() {
            var json = JsonSerializer.Serialize(userIDs, Shared.JsonSGParsers.Default.DictionaryUInt64Player);
            File.WriteAllText(_filePath, json);
        }

        public async Task Run(SocketSlashCommand commandSocket) {
            await commandSocket.DeferAsync();

            long userId = (long)commandSocket.Data.Options.ElementAt(0).Value;

            const string API_URI = "http://api.blobgame.io:888/api/users/info/";
            var responseOfRequest = await Shared.HttpClient.GetAsync(API_URI + userId);
            var json = await responseOfRequest.Content.ReadAsStringAsync();
            if (!VerifyUserId(json)) 
            {
                // Send a message back to the user
                var embed = new EmbedBuilder() {
                    Title = "Invalid Player ID",
                    Description = "Please enter an existing user, we have no record of a user with the id you provided in our database."
                };

                await commandSocket.FollowupAsync(embed: embed.Build());
                return;
            }
            var player = Player.FromJson(json);

            if (!VerifyUserId(json))
            {
                // Send a message back to the user
                var embed = new EmbedBuilder()
                {
                    Title = "Invalid Player ID",
                    Description = "Please enter an existing user, we have no record of a user with the id you provided in our database."
                };

                await commandSocket.FollowupAsync(embed: embed.Build());
                return;
            } 
            userIDs[commandSocket.User.Id] = player;
            SaveData();
            if(player.name == string.Empty)
            {
                var embed = new EmbedBuilder()
                {
                    Title = "Invalid Player ID",
                    Description = "Please enter an existing user, we have no record of a user with the id you provided in our database."
                };
                await commandSocket.FollowupAsync(embed: embed.Build());
            }

            var successEmbed = new EmbedBuilder() {
                Title = "ID Registered",
                Description = $"Your ID is now set to {userId}, hello {player.name}",
                Color = Color.Red,
            };
            var successEmbed2 = new EmbedBuilder()
            {
                Title = $"{player.name}'s stats",
                Description = $"Level: {player.lvl} Weekly Results: {player.week_result} Game Time: {player.game_time} Vip: {player.IsVip}",
                Color = Color.Red,
            };
            if(player.lvl != 0)
            {
                await commandSocket.FollowupAsync(embed: successEmbed.Build());
                await commandSocket.FollowupAsync(embed: successEmbed2.Build());
                //await VerifyCommand();
            }
            else
            {
                var embed = new EmbedBuilder()
                {
                    Title = "Invalid Player ID",
                    Description = "Please enter an existing user, we have no record of a user with the id you provided in our database."
                };
                await commandSocket.FollowupAsync(embed: embed.Build());
            }
        }
/*
        private async Task VerifyCommand(InteractionContext context)
        {
            var user = context.User;
            var username = user.GetGuildUser();
            await username.AddRoleAsync(1086067522173931531);
        }
*/
/*
        async Task VerifyCommand(InteractionContext ctx)
        {
            var user = ctx.User;
            var username = user.GetGuildUser();
            await username.AddRoleAsync(1086067522173931531);
        }
*/
        /// <summary>
        /// Verify the user ID using the Blob.io API.
        /// </summary>
        /// <param name="userId"></param>
        private bool VerifyUserId(string apiResponse) {
            // we fucked up somehow... Or they did...
            return apiResponse != """{"error":{"code":2,"message":"User ID was not found in the database"}}""";
        }

        public SlashCommandProperties Build()
        {
            var builder = new SlashCommandBuilder()
                .WithName("reg")
                .WithDescription("Register your Blob.io ID")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("id")
                    .WithDescription("Your Blob.io ID")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithRequired(true));

            return builder.Build();
        }
    }
}