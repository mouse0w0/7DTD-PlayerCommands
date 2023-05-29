using System.Collections.Generic;

namespace PlayerCommands.Command;

public static class TpCommand
{
    private static readonly Dictionary<int, TpaRequest> pendingRequest = new();

    public static void Tp(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("tp"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            sender.SendMessage("[FF5555]缺少参数");
            return;
        }

        var target = Utility.FindEntityPlayer(args[0]);
        if (target == null)
        {
            sender.SendMessage($"[FF5555]找不到玩家：{args[0]}");
            return;
        }

        if (sender.entityId == target.entityId)
        {
            sender.SendMessage("[FF5555]不能传送到自己");
            return;
        }

        pendingRequest[target.entityId] = new TpaRequest(sender.entityId);
        Utility.SendMessage(target.entityId, $"[FFAA00]“{sender.entityName}”请求传送到你，输入/ok接受");
        sender.SendMessage($"[FFAA00]已发送传送请求到“{target.EntityName}”");
    }

    public static void TpHere(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("tph"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            sender.SendMessage("[FF5555]缺少参数");
            return;
        }

        var target = Utility.FindEntityPlayer(args[0]);
        if (target == null)
        {
            sender.SendMessage($"[FF5555]找不到玩家：{args[0]}");
            return;
        }

        if (sender.entityId == target.entityId)
        {
            sender.SendMessage("[FF5555]不能传送到自己");
            return;
        }

        pendingRequest[target.entityId] = new TpaRequest(sender.entityId, true);
        Utility.SendMessage(target.entityId, $"[FFAA00]“{sender.entityName}”请求你传送到他，输入/ok接受");
        sender.SendMessage($"[FFAA00]已发送传送请求到“{target.EntityName}”");
    }

    public static void Ok(CommandSender sender, string[] args)
    {
        if (!pendingRequest.TryGetValue(sender.entityId, out var request))
        {
            sender.SendMessage("[FF5555]没有待处理的传送请求");
            return;
        }

        if (request.tpahere)
        {
            var target = Utility.GetEntityPlayer(request.requesterEntityId);
            Utility.GetClientInfo(sender.entityId).Teleport(target.position, target.rotation);
        }
        else
        {
            var target = Utility.GetEntityPlayer(sender.entityId);
            Utility.GetClientInfo(request.requesterEntityId).Teleport(target.position, target.rotation);
        }

        pendingRequest.Remove(sender.entityId);
        sender.SendMessage("[FFAA00]传送完毕");
        Utility.SendMessage(request.requesterEntityId, "[FFAA00]传送完毕");
    }

    private class TpaRequest
    {
        public readonly int requesterEntityId;
        public readonly bool tpahere;

        public TpaRequest(int requesterEntityId, bool tpahere = false)
        {
            this.requesterEntityId = requesterEntityId;
            this.tpahere = tpahere;
        }
    }
}