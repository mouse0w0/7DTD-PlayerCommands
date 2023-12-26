namespace PlayerCommands.Command;

public static class HelpCommand
{
    public static void Help(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("help"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        foreach (var s in Config.HelpMessage)
        {
            sender.SendMessage(s);
        }
    }
}