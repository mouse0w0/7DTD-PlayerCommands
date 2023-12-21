using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class Config
{
    public static Dictionary<string, string> Commands { get; private set; }
    public static HashSet<string> PlayerCommands { get; private set; }
    public static string[] HelpMessage { get; private set; }

    public static void Load()
    {
        var configFile = Main.Instance.Path + "/Config.json";
        if (!File.Exists(configFile))
        {
            Log.Error($"[PlayerCommands] Not found config file at {configFile}");
            return;
        }
        
        Log.Out("[PlayerCommands] Loading config");
        
        var root = JObject.Parse(File.ReadAllText(configFile));
        
        Commands = new Dictionary<string, string>();
        foreach (var commandPair in root.GetValue("Commands")!.ToObject<Dictionary<string, string[]>>())
        {
            var commandName = commandPair.Key;
            foreach (var commandAlias in commandPair.Value)
            {
                if (Commands.ContainsKey(commandAlias))
                {
                    Log.Warning($"[PlayerCommands] Duplicate alias `{commandAlias}` in command `{commandName}`");
                }
                else
                {
                    Commands.Add(commandAlias, commandName);
                }
            }
        }
        PlayerCommands = root.GetValue("PlayerCommands")!.ToObject<HashSet<string>>();
        HelpMessage = root.GetValue("HelpMessage")!.ToObject<string[]>();

        Log.Out("[PlayerCommands] Loaded config");
    }
}