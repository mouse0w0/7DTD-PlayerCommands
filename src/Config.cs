using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class Config
{
    public static TimeSpan TeleportCooldown { get; private set; }
    public static TimeSpan TeleportRequestTimeout { get; private set; }
    public static int TeleportMaxRequests { get; private set; }

    public static void Load()
    {
        var file = Main.Instance.Path + "/Config.jsonc";
        if (!File.Exists(file))
        {
            Log.Error($"[PlayerCommands] Not found config file at {file}");
            return;
        }

        Log.Out("[PlayerCommands] Loading config");

        var root = JObject.Parse(File.ReadAllText(file));

        CommandManager.LoadConfig(root["Commands"]!);

        TeleportCooldown = TimeSpan.FromSeconds(root["TeleportCooldown"]!.ToObject<double>());
        TeleportRequestTimeout = TimeSpan.FromSeconds(root["TeleportRequestTimeout"]!.ToObject<double>());
        TeleportMaxRequests = root["TeleportMaxRequests"]!.ToObject<int>();

        Log.Out("[PlayerCommands] Loaded config");
    }
}