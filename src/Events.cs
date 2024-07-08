using HarmonyLib;

namespace PlayerCommands;

[HarmonyPatch]
public static class Events
{
    public static readonly ModEvent WorldCreated = new();

    public static readonly ModEvent WorldUnloading = new();

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.createWorld))]
    [HarmonyPostfix]
    public static void GameManager_CreateWorld_Postfix()
    {
        WorldCreated.Invoke();
    }

    [HarmonyPatch(typeof(World), nameof(World.UnloadWorld))]
    [HarmonyPrefix]
    public static void World_UnloadWorld_Prefix()
    {
        WorldUnloading.Invoke();
    }
}