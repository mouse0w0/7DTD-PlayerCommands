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

    public static void SaveSpawn()
    {
        Utility.WriteAllText(GetGlobalDataPath("Spawn.json"), JsonConvert.SerializeObject(Spawn));
    }

    public static void SaveWraps()
    {
        Utility.WriteAllText(GetGlobalDataPath("Warps.json"), JsonConvert.SerializeObject(Warps));
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