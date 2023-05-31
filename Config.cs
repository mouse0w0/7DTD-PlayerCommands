using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class Config
{
    public static HashSet<string> PlayerCommands { get; private set; }

    public static void Load()
    {
        Log.Out("[PlayerCommands] Loading config");

        var configFile = Main.Instance.Path + "/Config.json";
        var root = JObject.Parse(Utility.ReadAllText(configFile) ?? "{}");

        PlayerCommands = root.GetValue("PlayerCommands").ToObject<HashSet<string>>();

        Log.Out("[PlayerCommands] Loaded config");
    }
}