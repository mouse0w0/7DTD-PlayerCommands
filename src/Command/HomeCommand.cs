namespace PlayerCommands.Command;

public static class HomeCommand
{
    public static void Home(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("home"))
        {
            return;
        }

        if (args.Length == 0)
        {
            if (sender.UserData.Homes.TryGetValue("default", out var location))
            {
                if (TeleportHandler.Teleport(sender, location))
                {
                    sender.SendMessage(Message.Get("Home.Success.Default"));
                }
            }
            else
            {
                sender.SendMessage(Message.Get("Home.NotSet.Default"));
            }
        }
        else
        {
            if (sender.UserData.Homes.TryGetValue(args[0], out var location))
            {
                if (TeleportHandler.Teleport(sender, location))
                {
                    sender.SendMessage(Message.Get("Home.Success").Format(args[0]));
                }
            }
            else
            {
                sender.SendMessage(Message.Get("Home.NotSet").Format(args[0]));
            }
        }
    }

    public static void SetHome(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("sethome"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.UserData.Homes["default"] = sender.Location;
            sender.SendMessage(Message.Get("SetHome.Success.Default"));
        }
        else
        {
            sender.UserData.Homes[args[0]] = sender.Location;
            sender.SendMessage(Message.Get("SetHome.Success").Format(args[0]));
        }
    }

    public static void DelHome(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("delhome"))
        {
            return;
        }

        if (args.Length == 0)
        {
            sender.UserData.Homes.Remove("default");
            sender.SendMessage(Message.Get("DelHome.Success.Default"));
        }
        else
        {
            sender.UserData.Homes.Remove(args[0]);
            sender.SendMessage(Message.Get("DelHome.Success").Format(args[0]));
        }
    }

    public static void ListHome(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("listhome"))
        {
            return;
        }

        var homes = sender.UserData.Homes;
        foreach (var (key, value) in homes)
        {
            sender.SendMessage(Message.Get("ListHome.Item").Format(key, value.ToPositionString()));
        }

        sender.SendMessage(Message.Get("ListHome.Success").Format(homes.Count));
    }
}