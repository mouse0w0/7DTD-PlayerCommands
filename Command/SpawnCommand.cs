using System;

namespace PlayerCommands.Command;

public static class SpawnCommand
{
    public static void Spawn(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("spawn"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (DataManager.spawn != null)
        {
            sender.Teleport(DataManager.spawn ?? throw new NullReferenceException());
            sender.SendMessage("[FFAA00]已传送至出生点");
        }
        else
        {
            sender.SendMessage("[FF5555]未设置出生点");
        }
    }

    public static void SetSpawn(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("setspawn"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }

        DataManager.spawn = sender.Location;
        sender.SendMessage("[FFAA00]已设置出生点");
    }

    public static void DelSpawn(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("delspawn"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        DataManager.spawn = null;
        sender.SendMessage("[FFAA00]已删除出生点");
    }

    public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType type, Vector3i position)
    {
        if (Utility.IsClient()) return;
        if (type is not (RespawnType.NewGame or RespawnType.EnterMultiplayer)) return;
        if (DataManager.spawn == null) return;
        clientInfo.Teleport(DataManager.spawn ?? throw new NullReferenceException());
    }
}