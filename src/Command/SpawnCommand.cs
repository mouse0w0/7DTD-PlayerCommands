using System;

namespace PlayerCommands.Command;

public static class SpawnCommand
{
    public static void Spawn(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("spawn"))
        {
            return;
        }

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

    public static void SetSpawn(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("setspawn"))
        {
            return;
        }

        WorldData.Spawn = sender.Location;
        sender.SendMessage(Message.Get("SetSpawn.Success"));
    }

    public static void DelSpawn(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("delspawn"))
        {
            return;
        }

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