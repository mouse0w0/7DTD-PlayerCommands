namespace PlayerCommands.Command;

public static class HomeCommand
{
    public static void Home(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("home"))
        {
            return;
        }

        if (args.Length == 0)
        {
            if (DataManager.PlayerDataDict[sender.PlayerId].homes.TryGetValue("default", out var location))
            {
                sender.Teleport(location);
                sender.SendMessage(Message.Get("Home.Finish.Default"));
            }
            else
            {
                sender.SendMessage(Message.Get("Home.NotSet.Default"));
            }
        }
        else
        {
            if (DataManager.PlayerDataDict[sender.PlayerId].homes.TryGetValue(args[0], out var location))
            {
                sender.Teleport(location);
                sender.SendMessage(Message.Get("Home.Finish").Format(args[0]));
            }
            else
            {
                sender.SendMessage(Message.Get("Home.NotSet").Format(args[0]));
            }
        }
    }

    public static void SetHome(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("sethome"))
        {
            return;
        }

        if (args.Length == 0)
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes["default"] = sender.Location;
            sender.SendMessage(Message.Get("SetHome.Finish.Default"));
        }
        else
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes[args[0]] = sender.Location;
            sender.SendMessage(Message.Get("SetHome.Finish").Format(args[0]));
        }
    }

    public static void DelHome(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("delhome"))
        {
            return;
        }

        if (args.Length == 0)
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes.Remove("default");
            sender.SendMessage(Message.Get("DelHome.Finish.Default"));
        }
        else
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes.Remove(args[0]);
            sender.SendMessage(Message.Get("DelHome.Finish").Format(args[0]));
        }
    }

    public static void ListHome(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("listhome"))
        {
            return;
        }

        var homes = DataManager.PlayerDataDict[sender.PlayerId].homes;
        foreach (var keyValuePair in homes)
        {
            sender.SendMessage(
                Message.Get("ListHome.Item").Format(keyValuePair.Key, keyValuePair.Value.ToPositionString()));
        }

        sender.SendMessage(Message.Get("ListHome.Finish").Format(homes.Count));
    }
}