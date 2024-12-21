using HarmonyLib;

namespace PlayerCommands;

[HarmonyPatch]
public static class Events
{
    public static readonly ModEvent WorldCreated = new();

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.createWorld))]
    [HarmonyPostfix]
    public static void GameManager_CreateWorld_Postfix()
    {
        if (Utility.IsClient()) return;
        WorldCreated.Invoke();
    }

    public static readonly ModEvent WorldUnloading = new();

    [HarmonyPatch(typeof(World), nameof(World.UnloadWorld))]
    [HarmonyPrefix]
    public static void World_UnloadWorld_Prefix()
    {
        if (Utility.IsClient()) return;
        WorldUnloading.Invoke();
    }

    public static readonly ModEvent<ClientInfo, RespawnType, Vector3i> PlayerSpawnedInWorld = new();

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerSpawnedInWorld))]
    [HarmonyPrefix]
    public static void GameManager_PlayerSpawnedInWorld_Prefix(ClientInfo _cInfo, RespawnType _respawnReason,
        Vector3i _pos)
    {
        if (Utility.IsClient()) return;
        PlayerSpawnedInWorld.Invoke(_cInfo, _respawnReason, _pos);
    }
}