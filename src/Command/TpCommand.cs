using System;
using System.Collections.Generic;
using UnityEngine.Analytics;

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

        pendingRequest[target.entityId] = new TpRequest(sender.entityId);
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

        pendingRequest[target.entityId] = new TpRequest(sender.entityId, true);
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

        if (request.tpHere)
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
        sender.SendMessage(Message.Get("TpAccept.Finish"));
        Utility.SendMessage(request.requesterEntityId, Message.Get("TpAccept.Finish"));
    }

    private class TpRequest
    {
        public readonly int requesterEntityId;
        public readonly bool tpHere;

        public TpRequest(int requesterEntityId, bool tpHere = false)
        {
            this.requesterEntityId = requesterEntityId;
            this.tpHere = tpHere;
        }
    }
}