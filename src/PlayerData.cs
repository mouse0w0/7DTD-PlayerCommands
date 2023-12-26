using System.Collections.Generic;

namespace PlayerCommands;

public class PlayerData
{
    public string lastPlayerName;
    public Dictionary<string, Location> homes = new();
}