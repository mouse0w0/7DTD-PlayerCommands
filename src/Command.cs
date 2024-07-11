using System.Collections.Generic;
using Newtonsoft.Json;

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

    public List<string> Labels { get; set; }
    public int PermissionLevel { get; set; }
}