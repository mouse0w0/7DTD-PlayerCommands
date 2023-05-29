using System.Collections.Generic;
using HarmonyLib;

namespace PlayerCommands.Command;

[HarmonyPatch]
public static class BackCommand
{
    private static readonly Dictionary<int, Location> PrevLocation = new();

    public static void Back(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("back"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        if (!PrevLocation.TryGetValue(sender.entityId, out var location))
        {
            sender.SendMessage("[FF5555]不存在上一地点");
            return;
        }

        sender.Teleport(location);
        sender.SendMessage("[FFAA00]已回到上一地点");
    }

    public static void OnEntityKilled(Entity entity, Entity killer)
    {
        if (Utility.IsClient()) return;
        if (entity is not EntityPlayer) return;
        PrevLocation[entity.entityId] = entity.GetLocation();
    }

    [HarmonyPatch(typeof(ClientInfo), nameof(ClientInfo.SendPackage))]
    [HarmonyPrefix]
    public static void ClientInfo_SendPackage_Prefix(ClientInfo __instance, NetPackage _package)
    {
        if (_package is not NetPackageTeleportPlayer) return;
        var entityPlayer = __instance.GetEntityPlayer();
        PrevLocation[entityPlayer.entityId] = entityPlayer.GetLocation();
    }

    [HarmonyPatch(typeof(NetPackageTeleportPlayer), nameof(NetPackageTeleportPlayer.ProcessPackage))]
    [HarmonyPrefix]
    public static void NetPackageTeleportPlayer_ProcessPackage_Prefix(NetPackageTeleportPlayer __instance, World _world,
        GameManager _callbacks)
    {
        if (Utility.IsClient()) return;
        if (_world == null) return;
        var entityPlayer = _world.GetPrimaryPlayer();
        if (entityPlayer == null) return;
        PrevLocation[entityPlayer.entityId] = entityPlayer.GetLocation();
    }
}