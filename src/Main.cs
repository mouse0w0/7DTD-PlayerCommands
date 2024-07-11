using HarmonyLib;
using PlayerCommands.Commands;

namespace PlayerCommands;

public class Main : IModApi
{
    public static Mod Instance { get; private set; }

    public void InitMod(Mod _modInstance)
    {
        Instance = _modInstance;

        Config.Load();
        Message.Load();

        new Harmony("com.github.mouse0w0.playercommands").PatchAll();

        Events.WorldCreated.RegisterHandler(WorldData.OnWorldCreated);
        Events.WorldUnloading.RegisterHandler(WorldData.OnWorldUnloading);
        Events.WorldUnloading.RegisterHandler(UserManager.OnWorldUnloading);
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(UserManager.OnPlayerSpawnedInWorld);
        ModEvents.PlayerDisconnected.RegisterHandler(UserManager.OnPlayerDisconnected);
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(SpawnCommand.OnPlayerSpawnedInWorld);
        ModEvents.EntityKilled.RegisterHandler(BackCommand.OnEntityKilled);
    }
}