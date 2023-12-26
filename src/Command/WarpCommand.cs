namespace PlayerCommands.Command;

public static class WarpCommand
{
    public static void Warp(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("warp"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        if (args.Length == 0)
        {
            sender.SendMessage("[FF5555]缺少参数");
            return;
        }

        if (!DataManager.warps.TryGetValue(args[0], out var location))
        {
            sender.SendMessage($"[FF5555]未找到地标：{args[0]}");
            return;
        }

        sender.Teleport(location);
        sender.SendMessage($"[FFAA00]已传送至地标：{args[0]}");
    }

    public static void SetWarp(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("setwarp"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage("[FF5555]缺少参数");
            return;
        }

        if (DataManager.warps.ContainsKey(args[0]))
        {
            sender.SendMessage($"[FF5555]已存在地标：{args[0]}");
        }
        else
        {
            DataManager.warps.Add(args[0], sender.Location);
            sender.SendMessage($"[FFAA00]已设置地标：{args[0]}");
        }
    }

    public static void DelWarp(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("delwarp"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }

        if (args.Length == 0)
        {
            sender.SendMessage("[FF5555]缺少参数");
            return;
        }

        if (DataManager.warps.Remove(args[0]))
            sender.SendMessage($"[FFAA00]已删除地标：{args[0]}");
        else
            sender.SendMessage($"[FF5555]未找到地标：{args[0]}");
    }

    public static void ListWarp(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("listwarp"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        foreach (var keyValuePair in DataManager.warps)
            sender.SendMessage($"[FFAA00]{keyValuePair.Key}：{keyValuePair.Value.ToPositionString()}");

        sender.SendMessage($"[FFAA00]已列出{DataManager.warps.Count}个地标");
    }
}