namespace PlayerCommands.Command;

public static class WarpCommand
{
    public static void Warp(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("warp"))
        {
            return;
        }
        
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (DataManager.warps.TryGetValue(args[0], out var location))
        {
            sender.Teleport(location);
            sender.SendMessage(Message.Get("Warp.Finish").Format(args[0]));
        }
        else
        {
            sender.SendMessage(Message.Get("Warp.NotSet").Format(args[0]));
        }
    }

    public static void SetWarp(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("setwarp"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (DataManager.warps.ContainsKey(args[0]))
        {
            sender.SendMessage(Message.Get("SetWarp.Exists").Format(args[0]));
        }
        else
        {
            DataManager.warps.Add(args[0], sender.Location);
            sender.SendMessage(Message.Get("SetWrap.Finish").Format(args[0]));
        }
    }

    public static void DelWarp(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("delwarp"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (DataManager.warps.Remove(args[0]))
        {
            sender.SendMessage(Message.Get("DelWarp.Finish").Format(args[0]));
        }
        else
        {
            sender.SendMessage(Message.Get("DelWarp.NotSet").Format(args[0]));
        }
    }

    public static void ListWarp(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("listwarp"))
        {
            return;
        }

        foreach (var keyValuePair in DataManager.warps)
        {
            sender.SendMessage(
                Message.Get("ListWarp.Item").Format(keyValuePair.Key, keyValuePair.Value.ToPositionString()));
        }

        sender.SendMessage(Message.Get("ListWarp.Finish").Format(DataManager.warps.Count));
    }
}