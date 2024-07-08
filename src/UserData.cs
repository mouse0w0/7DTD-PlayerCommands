using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PlayerCommands;

public class UserData
{
    [JsonIgnore]
    private readonly string _userDataFile;

    public UserData(string userDataFile)
    {
        _userDataFile = userDataFile;
    }

    public string LastPlayerName { get; set; }
    public Dictionary<string, Location> Homes { get; } = new();
    public bool TeleportEnabled { get; set; } = true;
    public bool AutoTeleportEnabled { get; set; }

    public void Load()
    {
        if (File.Exists(_userDataFile))
        {
            JsonConvert.PopulateObject(File.ReadAllText(_userDataFile), this);
        }
    }

    public void Save()
    {
        Utility.WriteAllText(_userDataFile, JsonConvert.SerializeObject(this));
    }
}