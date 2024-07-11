using System;

namespace PlayerCommands.Commands;

public static class SpawnCommand
{
    public static void Spawn(User sender, Command command, string label, string[] args)
    {
        if (WorldData.Spawn != null)
        {
            if (TeleportHandler.Teleport(sender, WorldData.Spawn ?? throw new NullReferenceException()))
            {
                sender.SendMessage(Message.Get("Spawn.Success"));
            }
        }
        else
        {
            sender.SendMessage(Message.Get("Spawn.NotSet"));
        }
    }

    public static void SetSpawn(User sender, Command command, string label, string[] args)
    {
        WorldData.Spawn = sender.Location;
        sender.SendMessage(Message.Get("SetSpawn.Success"));
    }

    public static void DelSpawn(User sender, Command command, string label, string[] args)
    {
        WorldData.Spawn = null;
        sender.SendMessage(Message.Get("DelSpawn.Success"));
    }

    public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType type, Vector3i position)
    {
        if (Utility.IsClient()) return;
        if (type is not (RespawnType.NewGame or RespawnType.EnterMultiplayer)) return;
        if (WorldData.Spawn == null) return;
        clientInfo.Teleport(WorldData.Spawn ?? throw new Exception("Cannot reach"));
    }
}