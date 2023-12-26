namespace PlayerCommands.Command;

public static class HomeCommand
{
    public static void Home(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("home"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            if (DataManager.PlayerDataDict[sender.PlayerId].homes.TryGetValue("default", out var location))
            {
                sender.Teleport(location);
                sender.SendMessage("[FFAA00]已回到家");
            }
            else
            {
                sender.SendMessage("[FF5555]未设置家");
            }
        }
        else
        {
            if (DataManager.PlayerDataDict[sender.PlayerId].homes.TryGetValue(args[0], out var location))
            {
                sender.Teleport(location);
                sender.SendMessage($"[FFAA00]已回到家:{args[0]}");
            }
            else
            {
                sender.SendMessage($"[FF5555]未设置家:{args[0]}");
            }
        }
    }

    public static void SetHome(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("sethome"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes["default"] = sender.Location;
            sender.SendMessage("[FFAA00]已设置家");
        }
        else
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes[args[0]] = sender.Location;
            sender.SendMessage($"[FFAA00]已设置家：{args[0]}");
        }
    }

    public static void DelHome(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("delhome"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes.Remove("default");
            sender.SendMessage("[FFAA00]已删除家");
        }
        else
        {
            DataManager.PlayerDataDict[sender.PlayerId].homes.Remove(args[0]);
            sender.SendMessage($"[FFAA00]已删除家：{args[0]}");
        }
    }

    public static void ListHome(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("listhome"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        var homes = DataManager.PlayerDataDict[sender.PlayerId].homes;
        foreach (var keyValuePair in homes)
            sender.SendMessage($"[FFAA00]{keyValuePair.Key}：{keyValuePair.Value.ToPositionString()}");

        sender.SendMessage($"[FFAA00]已列出{homes.Count}个家");
    }
}