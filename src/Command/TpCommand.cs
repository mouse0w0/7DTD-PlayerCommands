using System.Collections.Generic;

namespace PlayerCommands.Command;

public static class TpCommand
{
    private static readonly Dictionary<int, TpRequest> pendingRequest = new();

    public static void Tp(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("tp"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        var target = Utility.FindEntityPlayer(args[0]);
        if (target == null)
        {
            sender.SendMessage(Message.Get("Tp.NotFoundPlayer").Format(args[0]));
            return;
        }

        if (sender.entityId == target.entityId)
        {
            sender.SendMessage(Message.Get("Tp.CannotTeleportToSelf"));
            return;
        }

        pendingRequest[target.entityId] = new TpRequest(sender);
        Utility.SendMessage(target.entityId, Message.Get("Tp.Request").Format(sender.entityName));
        sender.SendMessage(Message.Get("Tp.Finish").Format(target.EntityName));
    }

    public static void TpHere(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("tph"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        var target = Utility.FindEntityPlayer(args[0]);
        if (target == null)
        {
            sender.SendMessage(Message.Get("Tp.NotFoundPlayer").Format(args[0]));
            return;
        }

        if (sender.entityId == target.entityId)
        {
            sender.SendMessage(Message.Get("Tp.CannotTeleportToSelf"));
            return;
        }

        pendingRequest[target.entityId] = new TpRequest(sender, true);
        Utility.SendMessage(target.entityId, Message.Get("Tp.Request.Here").Format(sender.entityName));
        sender.SendMessage(Message.Get("Tp.Finish").Format(target.EntityName));
    }

    public static void TpAccept(CommandSender sender, string[] args)
    {
        if (!pendingRequest.TryGetValue(sender.entityId, out var request))
        {
            sender.SendMessage(Message.Get("TpAccept.NoRequest"));
            return;
        }

        var requester = request.requester;

        if (request.tpHere)
        {
            sender.Teleport(requester.entityPlayer);
        }
        else
        {
            requester.Teleport(sender.entityPlayer);
        }

        pendingRequest.Remove(sender.entityId);
        sender.SendMessage(Message.Get("TpAccept.Finish"));
        requester.SendMessage(Message.Get("TpAccept.Finish"));
    }

    private class TpRequest
    {
        public readonly CommandSender requester;
        public readonly bool tpHere;

        public TpRequest(CommandSender requester, bool tpHere = false)
        {
            this.requester = requester;
            this.tpHere = tpHere;
        }
    }
}