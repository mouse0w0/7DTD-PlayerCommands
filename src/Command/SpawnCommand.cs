using System;

namespace PlayerCommands.Command;

public static class SpawnCommand
{
    public static void Spawn(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("spawn"))
        {
            return;
        }
        
        if (DataManager.spawn != null)
        {
            sender.Teleport(DataManager.spawn ?? throw new NullReferenceException());
            sender.SendMessage(Message.Get("Spawn.Finish"));
        }
        else
        {
            sender.SendMessage(Message.Get("Spawn.NotSet"));
        }
    }

    public static void SetSpawn(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("setspawn"))
        {
            return;
        }

        DataManager.spawn = sender.Location;
        sender.SendMessage(Message.Get("SetSpawn.Finish"));
    }

    public static void DelSpawn(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("delspawn"))
        {
            return;
        }
        
        DataManager.spawn = null;
        sender.SendMessage(Message.Get("DelSpawn.Finish"));
    }

    public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType type, Vector3i position)
    {
        if (Utility.IsClient()) return;
        if (type is not (RespawnType.NewGame or RespawnType.EnterMultiplayer)) return;
        if (DataManager.spawn == null) return;
        clientInfo.Teleport(DataManager.spawn ?? throw new Exception("Cannot reach"));
    }
}