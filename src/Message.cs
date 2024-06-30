using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class Message
{
    private static Dictionary<string, object> _messages = new();

    public static void Load()
    {
        var file = Main.Instance.Path + "/Message.json";
        if (!File.Exists(file))
        {
            Log.Error($"[PlayerCommands] Not found message file at {file}");
            return;
        }

        Log.Out("[PlayerCommands] Loading message");
        _messages.Clear();
        foreach (var keyValuePair in JObject.Parse(File.ReadAllText(file)))
        {
            if (keyValuePair.Value == null) continue;
            if (keyValuePair.Value.Type == JTokenType.Array)
            {
                _messages[keyValuePair.Key] = keyValuePair.Value.ToObject<string[]>();
            }
            else
            {
                _messages[keyValuePair.Key] = keyValuePair.Value.ToObject<string>();
            }
        }

        Log.Out("[PlayerCommands] Loaded message");
    }

    public static string Get(string messageKey)
    {
        return (string)_messages.GetValueOrDefault(messageKey, messageKey);
    }

    public static IEnumerable<string> GetArray(string messageKey)
    {
        return (string[])(_messages.TryGetValue(messageKey, out var obj) ? obj : new[] { messageKey });
    }
}