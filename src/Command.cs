using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public delegate void CommandExecutor(User sender, Command command, string label, string[] args);

public class Command
{
    [JsonIgnore] public string Name { get; }
    [JsonIgnore] public CommandExecutor Executor { get; }

    public Command(string name, CommandExecutor executor)
    {
        Name = name;
        Executor = executor;
    }

    public List<string> Labels { get; private set; }
    public int PermissionLevel { get; private set; }
    public TimeSpan Cooldown { get; private set; }

    internal void Load(JToken jToken)
    {
        Labels = jToken["Labels"]?.ToObject<List<string>>() ?? new List<string>();
        PermissionLevel = jToken["PermissionLevel"]?.ToObject<int>() ?? 0;
        Cooldown = TimeSpan.FromSeconds(jToken["Cooldown"]?.ToObject<double>() ?? 0);
    }
}