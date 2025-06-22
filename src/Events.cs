using HarmonyLib;

namespace PlayerCommands;

[HarmonyPatch]
public static class Events
{
    public static readonly ModEvents.ModEvent<SWorldCreatedData> WorldCreated = new();

    public struct SWorldCreatedData;

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.createWorld))]
    [HarmonyPostfix]
    public static void GameManager_CreateWorld_Postfix()
    {
        if (Utility.IsClient()) return;
        SWorldCreatedData eventData;
        WorldCreated.Invoke(ref eventData);
    }

    public static readonly ModEvents.ModEvent<SWorldUnloadingData> WorldUnloading = new();

    public struct SWorldUnloadingData;

    [HarmonyPatch(typeof(World), nameof(World.UnloadWorld))]
    [HarmonyPrefix]
    public static void World_UnloadWorld_Prefix()
    {
        if (Utility.IsClient()) return;
        SWorldUnloadingData eventData;
        WorldUnloading.Invoke(ref eventData);
    }

    public static readonly ModEvents.ModEvent<SPlayerSpawnedInWorldData> PlayerSpawnedInWorld = new();
    
    public struct SPlayerSpawnedInWorldData(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
    {
        public readonly ClientInfo ClientInfo = clientInfo;
        public readonly RespawnType RespawnType = respawnType;
        public readonly Vector3i Position = position;
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerSpawnedInWorld))]
    [HarmonyPrefix]
    public static void GameManager_PlayerSpawnedInWorld_Prefix(ClientInfo _cInfo, RespawnType _respawnReason,
        Vector3i _pos)
    {
        if (Utility.IsClient()) return;
        var eventData = new SPlayerSpawnedInWorldData(_cInfo, _respawnReason, _pos);
        PlayerSpawnedInWorld.Invoke(ref eventData);
    }
}