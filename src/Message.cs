using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class Message
{
    private static readonly Dictionary<string, object> Messages = new();

    public static void Load()
    {
        var file = Main.Instance.Path + "/Message.json";
        if (!File.Exists(file))
        {
            Log.Error($"[PlayerCommands] Not found message file at {file}");
            return;
        }

        Log.Out("[PlayerCommands] Loading message");
        Messages.Clear();
        foreach (var (key, value) in JObject.Parse(File.ReadAllText(file)))
        {
            if (value == null) continue;
            if (value.Type == JTokenType.Array)
            {
                Messages[key] = value.ToObject<string[]>();
            }
            else
            {
                Messages[key] = value.ToObject<string>();
            }
        }

        Log.Out("[PlayerCommands] Loaded message");
    }

    public static string Get(string messageKey)
    {
        return (string)Messages.GetValueOrDefault(messageKey, messageKey);
    }

    public static IEnumerable<string> GetArray(string messageKey)
    {
        return (string[])(Messages.TryGetValue(messageKey, out var obj) ? obj : new[] { messageKey });
    }
}