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

        TeleportCooldown = TimeSpan.FromSeconds(root["TeleportCooldown"]?.ToObject<double>() ?? 0);
        TeleportRequestTimeout = TimeSpan.FromSeconds(root["TeleportRequestTimeout"]?.ToObject<double>() ?? 0);
        TeleportMaxRequests = root["TeleportMaxRequests"]?.ToObject<int>() ?? 0;

        Log.Out("[PlayerCommands] Loaded config");
    }
}