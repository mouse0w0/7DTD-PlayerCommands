using HarmonyLib;
using PlayerCommands.Command;

namespace PlayerCommands;

public class Main : IModApi
{
    public static Mod Instance { get; private set; }

    public void InitMod(Mod _modInstance)
    {
        Instance = _modInstance;
        
        Config.Load();

        new Harmony("com.github.mouse0w0.essentials").PatchAll();
        
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(DataManager.OnPlayerSpawnedInWorld);
        ModEvents.PlayerDisconnected.RegisterHandler(DataManager.OnPlayerDisconnected);
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(SpawnCommand.OnPlayerSpawnedInWorld);
        ModEvents.EntityKilled.RegisterHandler(BackCommand.OnEntityKilled);
    }
}