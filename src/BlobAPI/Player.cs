using System.Text.Json;

namespace SussyBot.BlobAPI; 

public partial class Player
{
    public int with_fb_boost { get; set; }
    public int next_lvl_exp { get; set; }
    public int id { get; set; }
    public string reg_time { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
    public string photo_url { get; set; }
    public int cur_exp { get; set; }
    public int acum_exp { get; set; }
    public int lvl { get; set; }
    public string last_seen { get; set; }
    public int game_time { get; set; }
    public int vip { get; set; }
    public int role { get; set; }
    public int prv { get; set; }
    public string sid { get; set; }
    public int week_result { get; set; }
    public int del_status { get; set; }
    public object del_time { get; set; }
}
public partial class Player {


    /// <summary>
    /// Verifies if the player is a VIP player.
    /// </summary>
    public bool IsVip => vip == 1;

    /// <summary>
    /// Parses a player from a JSON.
    /// </summary>
    /// <param name="json">The JSON representing the Player class.</param>
    /// <returns>An instance that represents the string as a player class.</returns>
    public static Player? FromJson(string json) {
        return JsonSerializer.Deserialize(json, Shared.JsonSGParsers.Default.Player);
    }
}