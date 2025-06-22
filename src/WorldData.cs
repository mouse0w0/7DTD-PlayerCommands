using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlayerCommands;

public static class WorldData
{
    public static Location? Spawn { get; set; }
    public static Dictionary<string, Location> Warps { get; private set; }

    public static void OnWorldCreated(ref Events.SWorldCreatedData data)
    {
        Load();
    }

    public static void OnWorldUnloading(ref Events.SWorldUnloadingData data)
    {
        Save();
        Cleanup();
    }

    private static void Load()
    {
        Log.Out("[PlayerCommands] Loading world data.");
        Spawn = JsonConvert.DeserializeObject<Location?>(
            Utility.ReadAllText(GetGlobalDataPath("Spawn.json")) ?? "null");
        Warps = JsonConvert.DeserializeObject<Dictionary<string, Location>>(
            Utility.ReadAllText(GetGlobalDataPath("Warps.json")) ?? "{}");
        Log.Out("[PlayerCommands] Loaded world data.");
    }

    private static void Save()
    {
        Log.Out("[PlayerCommands] Saving world data.");
        Utility.WriteAllText(GetGlobalDataPath("Spawn.json"), JsonConvert.SerializeObject(Spawn));
        Utility.WriteAllText(GetGlobalDataPath("Warps.json"), JsonConvert.SerializeObject(Warps));
        Log.Out("[PlayerCommands] Saved world data.");
    }

    private static void Cleanup()
    {
        Spawn = null;
        Warps = null;
    }

    private static string GetGlobalDataPath(string fileName)
    {
        return GameIO.GetSaveGameDir() + "/PlayerCommands/" + fileName;
    }
}