namespace PlayerCommands.Commands;

public static class TpCommand
{
    public static void Tp(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("Command.NoEnoughParam"));
            return;
        }

        var target = UserManager.FindUserByPlayerName(args[0]);
        if (target == null)
        {
            sender.SendMessage(Message.Get("Command.NotFoundPlayer").Format(args[0]));
            return;
        }

        if (sender == target)
        {
            sender.SendMessage(Message.Get("Tp.CannotTeleportToSelf"));
            return;
        }

        var targetUserData = target.UserData;

        if (!targetUserData.TeleportEnabled)
        {
            sender.SendMessage(Message.Get("Tp.Disabled.Requester").Format(target.PlayerName));
            return;
        }

        if (targetUserData.AutoTeleportEnabled)
        {
            TeleportHandler.Teleport(sender, target);
            target.SendMessage(Message.Get("Tp.Accepted.Auto").Format(sender.PlayerName));
            sender.SendMessage(Message.Get("Tp.Teleported.Auto").Format(target.EntityName));
        }
        else
        {
            target.TeleportRequestList.Push(new TeleportRequest(sender));
            target.SendMessage(Message.Get("Tp.Request").Format(sender.PlayerName));
            sender.SendMessage(Message.Get("Tp.Sent").Format(target.EntityName));
        }
    }

    public static void TpHere(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("Command.NoEnoughParam"));
            return;
        }

        var target = UserManager.FindUserByPlayerName(args[0]);
        if (target == null)
        {
            sender.SendMessage(Message.Get("Command.NotFoundPlayer").Format(args[0]));
            return;
        }

        if (sender == target)
        {
            sender.SendMessage(Message.Get("Tp.CannotTeleportToSelf"));
            return;
        }

        var targetUserData = target.UserData;

        if (!targetUserData.TeleportEnabled)
        {
            sender.SendMessage(Message.Get("Tp.Disabled.Requester").Format(target.PlayerName));
            return;
        }

        if (targetUserData.AutoTeleportEnabled)
        {
            TeleportHandler.Teleport(target, sender);
            target.SendMessage(Message.Get("Tp.Accepted.Auto").Format(sender.PlayerName));
            sender.SendMessage(Message.Get("Tp.Teleported.Auto").Format(target.EntityName));
        }
        else
        {
            target.TeleportRequestList.Push(new TeleportRequest(sender, true));
            target.SendMessage(Message.Get("Tp.Request.Here").Format(sender.PlayerName));
            sender.SendMessage(Message.Get("Tp.Sent").Format(target.EntityName));
        }
    }

    public static void TpAll(User sender, Command command, string label, string[] args)
    {
        foreach (var target in UserManager.GetUsers())
        {
            if (target == sender) continue;
            var userData = target.UserData;
            if (!userData.TeleportEnabled) continue;
            if (userData.AutoTeleportEnabled)
            {
                TeleportHandler.Teleport(target, sender);
                target.SendMessage(Message.Get("Tp.Accepted.Auto").Format(sender.PlayerName));
                sender.SendMessage(Message.Get("Tp.Teleported.Auto").Format(target.EntityName));
            }
            else
            {
                target.TeleportRequestList.Push(new TeleportRequest(sender, true));
                target.SendMessage(Message.Get("Tp.Request.Here").Format(sender.PlayerName));
            }
        }

        sender.SendMessage(Message.Get("Tp.Sent.All"));
    }

    public static void TpCancel(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("Command.NoEnoughParam"));
            return;
        }

        var target = UserManager.FindUserByPlayerName(args[0]);
        if (target == null)
        {
            sender.SendMessage(Message.Get("Command.NotFoundPlayer").Format(args[0]));
            return;
        }

        if (sender == target)
        {
            sender.SendMessage(Message.Get("TpCancel.CannotBeSelf"));
            return;
        }

        if (target.TeleportRequestList.Pop(sender.PlayerName, out _))
        {
            sender.SendMessage(Message.Get("TpCancel.Success"));
            target.SendMessage(Message.Get("TpCancel.Cancelled").Format(sender.PlayerName));
        }
        else
        {
            sender.SendMessage(Message.Get("TpCancel.NotFound"));
        }
    }

    public static void TpAccept(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            if (sender.TeleportRequestList.Pop(out var request))
            {
                sender.SendMessage(Message.Get("Tp.Accepted").Format(request.Requester.PlayerName));
                AcceptRequest(sender, request);
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoRequest"));
            }
        }
        else if (args[0] == "*")
        {
            var requestList = sender.TeleportRequestList;
            if (requestList.Count != 0)
            {
                sender.SendMessage(Message.Get("Tp.Accepted.All"));
                while (requestList.Pop(out var request))
                {
                    AcceptRequest(sender, request);
                }
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoRequest"));
            }
        }
        else
        {
            if (sender.TeleportRequestList.PopFuzzily(args[0], out var request))
            {
                sender.SendMessage(Message.Get("Tp.Accepted").Format(request.Requester.PlayerName));
                AcceptRequest(sender, request);
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoFoundRequest").Format(args[0]));
            }
        }
    }

    private static void AcceptRequest(User sender, TeleportRequest request)
    {
        var requester = request.Requester;

        if (request.Here)
        {
            TeleportHandler.Teleport(sender, requester);
        }
        else
        {
            if (TeleportHandler.Teleport(requester, sender))
            {
                requester.SendMessage(Message.Get("Tp.Teleported").Format(sender.PlayerName));
            }
        }
    }

    public static void TpDeny(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            if (sender.TeleportRequestList.Pop(out var request))
            {
                sender.SendMessage(Message.Get("Tp.Denied").Format(request.Requester.PlayerName));
                DenyRequest(sender, request);
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoRequest"));
            }
        }
        else if (args[0] == "*")
        {
            var requestList = sender.TeleportRequestList;
            if (requestList.Count != 0)
            {
                sender.SendMessage(Message.Get("Tp.Denied.All"));
                while (requestList.Pop(out var request))
                {
                    DenyRequest(sender, request);
                }
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoRequest"));
            }
        }
        else
        {
            if (sender.TeleportRequestList.PopFuzzily(args[0], out var request))
            {
                sender.SendMessage(Message.Get("Tp.Denied").Format(request.Requester.PlayerName));
                DenyRequest(sender, request);
            }
            else
            {
                sender.SendMessage(Message.Get("Tp.NoFoundRequest").Format(args[0]));
            }
        }
    }

    private static void DenyRequest(User sender, TeleportRequest request)
    {
        request.Requester.SendMessage(Message.Get("Tp.Denied.Requester").Format(sender.PlayerName));
    }

    public static void TpToggle(User sender, Command command, string label, string[] args)
    {
        var userData = sender.UserData;
        var enabled = args.Length == 0 ? !userData.TeleportEnabled : bool.Parse(args[0]);
        userData.TeleportEnabled = enabled;
        sender.SendMessage(Message.Get(enabled ? "Tp.Enabled" : "Tp.Disabled"));
    }

    public static void TpAuto(User sender, Command command, string label, string[] args)
    {
        var userData = sender.UserData;
        var enabled = args.Length == 0 ? !userData.AutoTeleportEnabled : bool.Parse(args[0]);
        userData.AutoTeleportEnabled = enabled;
        sender.SendMessage(Message.Get(enabled ? "TpAuto.Enabled" : "TpAuto.Disabled"));
        if (enabled && !userData.TeleportEnabled)
        {
            sender.SendMessage(Message.Get("TpAuto.TpDisabledWarning"));
        }
    }
}