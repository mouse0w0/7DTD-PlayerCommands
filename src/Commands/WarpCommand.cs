namespace PlayerCommands.Commands;

public static class WarpCommand
{
    public static void Warp(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (WorldData.Warps.TryGetValue(args[0], out var location))
        {
            if (TeleportHandler.Teleport(sender, location))
            {
                sender.SendMessage(Message.Get("Warp.Success").Format(args[0]));
            }
        }
        else
        {
            sender.SendMessage(Message.Get("Warp.NotSet").Format(args[0]));
        }
    }

    public static void SetWarp(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (WorldData.Warps.ContainsKey(args[0]))
        {
            sender.SendMessage(Message.Get("SetWarp.Exists").Format(args[0]));
        }
        else
        {
            WorldData.Warps.Add(args[0], sender.Location);
            sender.SendMessage(Message.Get("SetWrap.Success").Format(args[0]));
        }
    }

    public static void DelWarp(User sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            sender.SendMessage(Message.Get("NoEnoughParam"));
            return;
        }

        if (WorldData.Warps.Remove(args[0]))
        {
            sender.SendMessage(Message.Get("DelWarp.Success").Format(args[0]));
        }
        else
        {
            sender.SendMessage(Message.Get("DelWarp.NotSet").Format(args[0]));
        }
    }

    public static void ListWarp(User sender, Command command, string label, string[] args)
    {
        foreach (var (key, value) in WorldData.Warps)
        {
            sender.SendMessage(Message.Get("ListWarp.Item").Format(key, value.ToPositionString()));
        }

        sender.SendMessage(Message.Get("ListWarp.Success").Format(WorldData.Warps.Count));
    }
}