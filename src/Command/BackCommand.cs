using HarmonyLib;

namespace PlayerCommands.Command;

[HarmonyPatch]
public static class BackCommand
{
    public static void Back(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("back"))
        {
            return;
        }

        if (sender.PrevLocation is {} prevLocation)
        {
            if (TeleportHandler.Teleport(sender, prevLocation))
            {
                sender.SendMessage(Message.Get("Back.Success"));
            }
        }
        else
        {
            sender.SendMessage(Message.Get("Back.NotFound"));
        }
    }

    public static void OnEntityKilled(Entity entity, Entity killer)
    {
        if (Utility.IsClient()) return;
        if (entity is not EntityPlayer entityPlayer) return;
        var user = entityPlayer.ToUser();
        user.PrevLocation = user.Location;
    }

    [HarmonyPatch(typeof(ClientInfo), nameof(ClientInfo.SendPackage))]
    [HarmonyPrefix]
    public static void ClientInfo_SendPackage_Prefix(ClientInfo __instance, NetPackage _package)
    {
        if (_package is not NetPackageTeleportPlayer) return;
        var user = __instance.ToUser();
        user.PrevLocation = user.Location;
    }

    [HarmonyPatch(typeof(NetPackageTeleportPlayer), nameof(NetPackageTeleportPlayer.ProcessPackage))]
    [HarmonyPrefix]
    public static void NetPackageTeleportPlayer_ProcessPackage_Prefix(NetPackageTeleportPlayer __instance, World _world,
        GameManager _callbacks)
    {
        if (Utility.IsClient()) return;
        if (_world == null) return;
        var user = UserManager.GetPrimaryUser();
        user.PrevLocation = user.Location;
    }
}